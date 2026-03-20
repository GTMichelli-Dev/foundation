#!/bin/bash
set -euo pipefail

# Deploy Basic Weigh to a remote Debian server
# Usage: ./deploy.sh <user@host> [options]
#
# Options:
#   --domain <domain>    Domain name for Let's Encrypt SSL (e.g. scale.example.com)
#   --email <email>      Email for Let's Encrypt notifications
#   --port <port>        App listen port (default 5110)
#   --key <ssh-key>      SSH key file
#
# Examples:
#   ./deploy.sh admin@192.168.1.100
#   ./deploy.sh admin@192.168.1.100 --domain scale.example.com --email admin@example.com
#   ./deploy.sh admin@10.0.0.5 --port 8080 --key ~/.ssh/id_rsa

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARBALL="$SCRIPT_DIR/basicweigh-deploy.tar.gz"

# Parse arguments
REMOTE=""
DOMAIN=""
EMAIL=""
APP_PORT="5110"
SSH_KEY=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --domain)  DOMAIN="$2";   shift 2 ;;
    --email)   EMAIL="$2";    shift 2 ;;
    --port)    APP_PORT="$2"; shift 2 ;;
    --key)     SSH_KEY="$2";  shift 2 ;;
    -*)        echo "Unknown option: $1"; exit 1 ;;
    *)         REMOTE="$1";   shift ;;
  esac
done

if [[ -z "$REMOTE" ]]; then
  echo "Usage: ./deploy.sh <user@host> [options]"
  echo ""
  echo "Options:"
  echo "  --domain <domain>    Domain for Let's Encrypt SSL"
  echo "  --email <email>      Email for Let's Encrypt"
  echo "  --port <port>        App port (default 5110)"
  echo "  --key <ssh-key>      SSH key file"
  echo ""
  echo "Examples:"
  echo "  ./deploy.sh admin@192.168.1.100"
  echo "  ./deploy.sh admin@192.168.1.100 --domain scale.example.com --email admin@example.com"
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
  echo "==> Tarball not found. Running publish first..."
  bash "$SCRIPT_DIR/publish.sh"
fi

echo "==> Uploading to $REMOTE..."
scp $SCP_OPTS "$TARBALL" "$REMOTE:/tmp/basicweigh-deploy.tar.gz"

# Build install command with parameters
INSTALL_CMD="cd /tmp"
INSTALL_CMD="$INSTALL_CMD && mkdir -p /tmp/basicweigh-install"
INSTALL_CMD="$INSTALL_CMD && tar -xzf /tmp/basicweigh-deploy.tar.gz -C /tmp/basicweigh-install"
INSTALL_CMD="$INSTALL_CMD && cd /tmp/basicweigh-install"
INSTALL_CMD="$INSTALL_CMD && sed -i 's/\r$//' install.sh"
INSTALL_CMD="$INSTALL_CMD && sudo DOMAIN='$DOMAIN' EMAIL='$EMAIL' PORT='$APP_PORT' bash install.sh"
INSTALL_CMD="$INSTALL_CMD && rm -rf /tmp/basicweigh-install /tmp/basicweigh-deploy.tar.gz"

echo "==> Installing on remote server..."
ssh $SSH_OPTS "$REMOTE" "$INSTALL_CMD"

echo ""
echo "=========================================="
echo "  Deploy complete!"
echo "=========================================="
echo "  Server: $REMOTE"
if [[ -n "$DOMAIN" ]]; then
echo "  URL:    https://$DOMAIN"
else
echo "  URL:    https://${REMOTE#*@}"
fi
echo "  Check:  ssh $REMOTE 'systemctl status basicweigh'"
echo "=========================================="
