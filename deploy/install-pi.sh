#!/bin/bash
set -euo pipefail

# Install Kiosk Print Agent on a Raspberry Pi
# Run as root: sudo bash install-pi.sh
#
# Expects to be run from the extracted tarball directory, or
# pass the tarball path: sudo bash install-pi.sh /tmp/kioskprint-deploy.tar.gz

PRINT_DIR="/opt/kioskprint"
SERVICE_USER="pi"

#--------------------------------------------------
# 0. If a tarball was passed, extract it first
#--------------------------------------------------
if [[ "${1:-}" == *.tar.gz ]] && [[ -f "${1:-}" ]]; then
  echo "==> Extracting tarball..."
  WORK_DIR="$(mktemp -d)"
  tar -xzf "$1" -C "$WORK_DIR"
  cd "$WORK_DIR"
fi

#--------------------------------------------------
# 1. Install required packages (CUPS for printing)
#--------------------------------------------------
echo "==> Installing required packages..."
apt-get update -qq
apt-get install -y -qq cups rsync > /dev/null
echo "  cups, rsync installed."

# Add pi user to lpadmin group for printer access
usermod -aG lpadmin "$SERVICE_USER" 2>/dev/null || true

#--------------------------------------------------
# 2. Stop existing service
#--------------------------------------------------
echo "==> Stopping service (if running)..."
systemctl stop kioskprint 2>/dev/null || true

#--------------------------------------------------
# 3. Install Kiosk Print Agent
#--------------------------------------------------
echo "==> Installing KioskPrintAgent to $PRINT_DIR..."
mkdir -p "$PRINT_DIR"

# Preserve existing config
if [[ -f "$PRINT_DIR/appsettings.json" ]]; then
  cp "$PRINT_DIR/appsettings.json" /tmp/kioskprint-appsettings.json.bak
  echo "  Config backed up."
fi

rsync -a --delete kioskprint/ "$PRINT_DIR/"

# Restore config
if [[ -f /tmp/kioskprint-appsettings.json.bak ]]; then
  cp /tmp/kioskprint-appsettings.json.bak "$PRINT_DIR/appsettings.json"
  echo "  Config restored."
fi

chmod +x "$PRINT_DIR/KioskPrintAgent"
chown -R "$SERVICE_USER:$SERVICE_USER" "$PRINT_DIR"

#--------------------------------------------------
# 4. Install systemd service
#--------------------------------------------------
echo "==> Installing systemd service..."
cp kioskprint.service /etc/systemd/system/kioskprint.service
systemctl daemon-reload

#--------------------------------------------------
# 5. Enable and start service
#--------------------------------------------------
echo "==> Enabling and starting KioskPrintAgent..."
systemctl enable kioskprint
systemctl start kioskprint
echo "  KioskPrint: $(systemctl is-active kioskprint)"

#--------------------------------------------------
# 6. Done
#--------------------------------------------------
echo ""
echo "=========================================="
echo "  Pi Installation complete!"
echo "=========================================="
echo "  Print Agent: $PRINT_DIR"
echo "  Service:     systemctl status kioskprint"
echo "  Logs:        journalctl -u kioskprint -f"
echo ""
echo "  Configure: $PRINT_DIR/appsettings.json"
echo "    ServerUrl:   https://<server-address>"
echo "    PrinterName: (run 'lpstat -p' to list printers)"
echo "    PrinterId:   1 = Inbound, 2 = Outbound"
echo "=========================================="
