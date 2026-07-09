#!/bin/bash
set -euo pipefail

# Install Foundation on a Raspberry Pi — Kestrel serving directly, no nginx.
# Intended for internal-network deployments (plain HTTP on port 80).
# Run as root: sudo bash install-pi-web.sh
#
# Expects to be run from the extracted tarball directory, or
# pass the tarball path: sudo bash install-pi-web.sh /tmp/foundation-pi-deploy.tar.gz
#
# Environment variables (optional):
#   PORT=80                    — app listen port (default 80)
#   REBUILD_DB=1               — delete and recreate the database

APP_DIR="/opt/foundation"
SERVICE_USER="admin"
APP_PORT="${PORT:-80}"
REBUILD_DB="${REBUILD_DB:-0}"

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
# 1. Install required packages
#--------------------------------------------------
echo "==> Installing required packages..."
apt-get update -qq
apt-get install -y -qq rsync curl libfontconfig1 fonts-dejavu-core > /dev/null
echo "  rsync, curl, libfontconfig1 installed."

# Create service user if it doesn't exist
if ! id "$SERVICE_USER" &>/dev/null; then
  useradd -r -m -s /bin/bash "$SERVICE_USER"
  echo "  Created user: $SERVICE_USER"
fi

# Open firewall ports (Pi OS ships with no firewall, but a site-hardened
# image may have ufw or iptables rules that would block the app)
echo "==> Configuring firewall..."
if command -v ufw &>/dev/null && ufw status | grep -q "active"; then
  ufw allow 22/tcp > /dev/null
  ufw allow "$APP_PORT"/tcp > /dev/null
  echo "  Firewall: ufw — ports 22 and $APP_PORT opened."
fi

if command -v iptables &>/dev/null; then
  # Only add if not already present
  iptables -C INPUT -p tcp --dport "$APP_PORT" -j ACCEPT 2>/dev/null || \
    iptables -I INPUT -p tcp --dport "$APP_PORT" -j ACCEPT
  echo "  Firewall: iptables — port $APP_PORT opened."

  # Persist iptables rules across reboots
  if command -v netfilter-persistent &>/dev/null; then
    netfilter-persistent save 2>/dev/null || true
  elif command -v iptables-save &>/dev/null; then
    mkdir -p /etc/iptables
    iptables-save > /etc/iptables/rules.v4 2>/dev/null || true
  fi
  echo "  Firewall: rules saved for reboot persistence."
fi

#--------------------------------------------------
# 2. Stop existing service
#--------------------------------------------------
echo "==> Stopping service (if running)..."
systemctl stop foundation 2>/dev/null || true

# Migrate a pre-rename "Basic Weigh" install: same app, old name.
if [[ -d /opt/basicweigh && ! -d "$APP_DIR" ]]; then
  echo "==> Migrating existing Basic Weigh install to Foundation..."
  systemctl stop basicweigh 2>/dev/null || true
  systemctl disable basicweigh 2>/dev/null || true
  rm -f /etc/systemd/system/basicweigh.service
  systemctl daemon-reload
  mv /opt/basicweigh "$APP_DIR"
  if [[ -f "$APP_DIR/BasicWeigh.db" && ! -f "$APP_DIR/Foundation.db" ]]; then
    mv "$APP_DIR/BasicWeigh.db" "$APP_DIR/Foundation.db"
    echo "  Database renamed: BasicWeigh.db -> Foundation.db"
  fi
  echo "  Migration complete."
fi

#--------------------------------------------------
# 3. Install Foundation web app
#--------------------------------------------------
echo "==> Installing Foundation.Web to $APP_DIR..."
mkdir -p "$APP_DIR"

# Preserve database and reports if they exist (unless rebuild requested)
if [[ "$REBUILD_DB" == "1" ]]; then
  echo "  --rebuild-db: Database will be recreated from scratch."
  rm -f "$APP_DIR/Foundation.db" /tmp/Foundation.db.bak
else
  if [[ -f "$APP_DIR/Foundation.db" ]]; then
    cp "$APP_DIR/Foundation.db" /tmp/Foundation.db.bak
    echo "  Database backed up to /tmp/Foundation.db.bak"
  fi
fi
if [[ -d "$APP_DIR/Reports" ]]; then
  cp -r "$APP_DIR/Reports" /tmp/Reports.bak
  echo "  Reports backed up to /tmp/Reports.bak/"
fi

# Copy new files. Exclude wwwroot/images/tickets/ from the --delete sweep
# so captured ticket photos (runtime uploads, not part of the build) survive
# every deploy. Without this, --delete wipes the entire directory.
rsync -a --delete --exclude='wwwroot/images/tickets/' foundation/ "$APP_DIR/"

# Make sure the tickets directory exists for fresh installs and that the
# service user can write to it.
mkdir -p "$APP_DIR/wwwroot/images/tickets"

# Restore database and reports
if [[ "$REBUILD_DB" != "1" ]] && [[ -f /tmp/Foundation.db.bak ]]; then
  cp /tmp/Foundation.db.bak "$APP_DIR/Foundation.db"
  echo "  Database restored."
fi
if [[ -d /tmp/Reports.bak ]]; then
  cp -r /tmp/Reports.bak/* "$APP_DIR/Reports/" 2>/dev/null || true
  echo "  Reports restored."
fi

chmod +x "$APP_DIR/Foundation.Web"
chown -R "$SERVICE_USER:$SERVICE_USER" "$APP_DIR"

#--------------------------------------------------
# 4. Install systemd service (Kestrel binds the port itself)
#--------------------------------------------------
echo "==> Installing systemd service (port $APP_PORT)..."
sed "s|http://0.0.0.0:80|http://0.0.0.0:$APP_PORT|" foundation-pi.service \
  > /etc/systemd/system/foundation.service
systemctl daemon-reload

#--------------------------------------------------
# 5. Enable and start service
#--------------------------------------------------
echo "==> Enabling and starting Foundation..."
systemctl enable foundation
systemctl start foundation
echo "  Foundation: $(systemctl is-active foundation)"

#--------------------------------------------------
# 6. Done
#--------------------------------------------------
PI_IP="$(hostname -I | awk '{print $1}')"
echo ""
echo "=========================================="
echo "  Installation complete!"
echo "=========================================="
echo "  Web App: $APP_DIR"
echo "  Service: systemctl status foundation"
echo "  Logs:    journalctl -u foundation -f"
if [[ "$APP_PORT" == "80" ]]; then
echo "  URL:     http://$PI_IP"
else
echo "  URL:     http://$PI_IP:$APP_PORT"
fi
echo ""
echo "  NOTE: Kestrel serves plain HTTP directly (no nginx, no SSL)."
echo "        Intended for trusted internal networks only."
echo "=========================================="
