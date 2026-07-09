#!/bin/bash
set -euo pipefail

# Deploy Kiosk Print Agent to a Raspberry Pi
# Usage: ./deploy-pi.sh <user@host> [options]
#
# Options:
#   --server <url>       Foundation server URL (e.g. https://scale.example.com)
#   --printer <name>     CUPS printer name (run 'lpstat -p' on Pi to list)
#   --printer-id <1|2>   1 = Inbound, 2 = Outbound (default 1)
#   --key <ssh-key>      SSH key file
#
# Examples:
#   ./deploy-pi.sh pi@192.168.1.50
#   ./deploy-pi.sh pi@192.168.1.50 --server https://scale.example.com --printer Zebra --printer-id 1

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARBALL="$SCRIPT_DIR/kioskprint-deploy.tar.gz"

# Parse arguments
REMOTE=""
SERVER_URL=""
PRINTER_NAME=""
PRINTER_ID=""
SSH_KEY=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --server)      SERVER_URL="$2";   shift 2 ;;
    --printer)     PRINTER_NAME="$2"; shift 2 ;;
    --printer-id)  PRINTER_ID="$2";   shift 2 ;;
    --key)         SSH_KEY="$2";      shift 2 ;;
    -*)            echo "Unknown option: $1"; exit 1 ;;
    *)             REMOTE="$1";       shift ;;
  esac
done

if [[ -z "$REMOTE" ]]; then
  echo "Usage: ./deploy-pi.sh <user@host> [options]"
  echo ""
  echo "Options:"
  echo "  --server <url>       Foundation server URL"
  echo "  --printer <name>     CUPS printer name"
  echo "  --printer-id <1|2>   1=Inbound, 2=Outbound (default 1)"
  echo "  --key <ssh-key>      SSH key file"
  echo ""
  echo "Examples:"
  echo "  ./deploy-pi.sh pi@192.168.1.50"
  echo "  ./deploy-pi.sh pi@192.168.1.50 --server https://scale.example.com --printer Zebra --printer-id 1"
  exit 1
fi

# Build SSH options
SSH_OPTS="-o StrictHostKeyChecking=no"
SCP_OPTS="-o StrictHostKeyChecking=no"
if [[ -n "$SSH_KEY" ]]; then
  SSH_OPTS="$SSH_OPTS -i $SSH_KEY"
  SCP_OPTS="$SCP_OPTS -i $SSH_KEY"
fi

# Check if tarball exists, if not run publish first
if [[ ! -f "$TARBALL" ]]; then
  echo "==> Tarball not found. Running publish-pi first..."
  bash "$SCRIPT_DIR/publish-pi.sh"
fi

echo "==> Uploading to $REMOTE..."
scp $SCP_OPTS "$TARBALL" "$REMOTE:/tmp/kioskprint-deploy.tar.gz"

echo "==> Installing on Pi..."
ssh $SSH_OPTS "$REMOTE" "
  cd /tmp && \
  mkdir -p /tmp/kioskprint-install && \
  tar -xzf /tmp/kioskprint-deploy.tar.gz -C /tmp/kioskprint-install && \
  cd /tmp/kioskprint-install && \
  sudo bash install-pi.sh && \
  rm -rf /tmp/kioskprint-install /tmp/kioskprint-deploy.tar.gz
"

# Update appsettings.json if parameters were provided
if [[ -n "$SERVER_URL" || -n "$PRINTER_NAME" || -n "$PRINTER_ID" ]]; then
  echo "==> Updating print agent config..."
  CONFIG_CMD=""
  if [[ -n "$SERVER_URL" ]]; then
    CONFIG_CMD="$CONFIG_CMD sed -i 's|\"ServerUrl\":.*|\"ServerUrl\": \"$SERVER_URL\",|' /opt/kioskprint/appsettings.json &&"
  fi
  if [[ -n "$PRINTER_NAME" ]]; then
    CONFIG_CMD="$CONFIG_CMD sed -i 's|\"PrinterName\":.*|\"PrinterName\": \"$PRINTER_NAME\",|' /opt/kioskprint/appsettings.json &&"
  fi
  if [[ -n "$PRINTER_ID" ]]; then
    CONFIG_CMD="$CONFIG_CMD sed -i 's|\"PrinterId\":.*|\"PrinterId\": $PRINTER_ID|' /opt/kioskprint/appsettings.json &&"
  fi
  CONFIG_CMD="$CONFIG_CMD sudo systemctl restart kioskprint"
  ssh $SSH_OPTS "$REMOTE" "$CONFIG_CMD"
  echo "  Config updated and service restarted."
fi

echo ""
echo "=========================================="
echo "  Pi Deploy complete!"
echo "=========================================="
echo "  Pi: $REMOTE"
if [[ -n "$SERVER_URL" ]]; then
echo "  Server: $SERVER_URL"
fi
echo "  Check: ssh $REMOTE 'systemctl status kioskprint'"
echo "=========================================="
