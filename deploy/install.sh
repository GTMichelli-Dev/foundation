#!/bin/bash
set -euo pipefail

# Install Basic Weigh on a Debian server
# Run as root: sudo bash install.sh
#
# Expects to be run from the extracted tarball directory, or
# pass the tarball path: sudo bash install.sh /tmp/basicweigh-deploy.tar.gz

APP_DIR="/opt/basicweigh"
PRINT_DIR="/opt/kioskprint"
SERVICE_USER="admin"

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
# 1. Stop existing services
#--------------------------------------------------
echo "==> Stopping services (if running)..."
systemctl stop basicweigh 2>/dev/null || true
systemctl stop kioskprint 2>/dev/null || true

#--------------------------------------------------
# 2. Install Basic Weigh web app
#--------------------------------------------------
echo "==> Installing BasicWeigh.Web to $APP_DIR..."
mkdir -p "$APP_DIR"

# Preserve database and reports if they exist
if [[ -f "$APP_DIR/BasicWeigh.db" ]]; then
  cp "$APP_DIR/BasicWeigh.db" /tmp/BasicWeigh.db.bak
  echo "  Database backed up to /tmp/BasicWeigh.db.bak"
fi
if [[ -d "$APP_DIR/Reports" ]]; then
  cp -r "$APP_DIR/Reports" /tmp/Reports.bak
  echo "  Reports backed up to /tmp/Reports.bak/"
fi

# Copy new files
rsync -a --delete basicweigh/ "$APP_DIR/"

# Restore database and reports
if [[ -f /tmp/BasicWeigh.db.bak ]]; then
  cp /tmp/BasicWeigh.db.bak "$APP_DIR/BasicWeigh.db"
  echo "  Database restored."
fi
if [[ -d /tmp/Reports.bak ]]; then
  cp -r /tmp/Reports.bak/* "$APP_DIR/Reports/" 2>/dev/null || true
  echo "  Reports restored."
fi

chmod +x "$APP_DIR/BasicWeigh.Web"
chown -R "$SERVICE_USER:$SERVICE_USER" "$APP_DIR"

#--------------------------------------------------
# 3. Install Kiosk Print Agent
#--------------------------------------------------
if [[ -d "kioskprint" ]]; then
  echo "==> Installing KioskPrintAgent to $PRINT_DIR..."
  mkdir -p "$PRINT_DIR"

  # Preserve existing config
  if [[ -f "$PRINT_DIR/appsettings.json" ]]; then
    cp "$PRINT_DIR/appsettings.json" /tmp/kioskprint-appsettings.json.bak
  fi

  rsync -a --delete kioskprint/ "$PRINT_DIR/"

  # Restore config
  if [[ -f /tmp/kioskprint-appsettings.json.bak ]]; then
    cp /tmp/kioskprint-appsettings.json.bak "$PRINT_DIR/appsettings.json"
    echo "  Print agent config restored."
  fi

  chmod +x "$PRINT_DIR/KioskPrintAgent"
  chown -R "$SERVICE_USER:$SERVICE_USER" "$PRINT_DIR"
fi

#--------------------------------------------------
# 4. Install systemd services
#--------------------------------------------------
echo "==> Installing systemd services..."
cp basicweigh.service /etc/systemd/system/basicweigh.service
cp kioskprint.service /etc/systemd/system/kioskprint.service
systemctl daemon-reload

#--------------------------------------------------
# 5. Update Nginx to reverse proxy to Kestrel
#--------------------------------------------------
if command -v nginx &>/dev/null; then
  echo "==> Configuring Nginx reverse proxy..."

  # Only update if not already configured for basicweigh
  if ! grep -q "proxy_pass http://127.0.0.1:5110" /etc/nginx/sites-available/default 2>/dev/null; then
    # Get the domain from existing config
    DOMAIN=$(grep -oP 'server_name\s+\K[^;]+' /etc/nginx/sites-available/default 2>/dev/null | head -1 || echo "localhost")

    cat > /etc/nginx/sites-available/default <<NGINX
# HTTP — redirect to HTTPS
server {
    listen 80 default_server;
    listen [::]:80 default_server;
    server_name $DOMAIN;
    return 301 https://\$host\$request_uri;
}

# HTTPS — reverse proxy to Basic Weigh
server {
    listen 443 ssl default_server;
    listen [::]:443 ssl default_server;
    server_name $DOMAIN;

    ssl_certificate     /etc/nginx/ssl/nginx.crt;
    ssl_certificate_key /etc/nginx/ssl/nginx.key;
    ssl_protocols       TLSv1.2 TLSv1.3;
    ssl_ciphers         HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Content-Type-Options nosniff always;

    location / {
        proxy_pass         http://127.0.0.1:5110;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade \$http_upgrade;
        proxy_set_header   Connection "upgrade";
        proxy_set_header   Host \$host;
        proxy_set_header   X-Real-IP \$remote_addr;
        proxy_set_header   X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;

        # WebSocket support (SignalR)
        proxy_read_timeout 86400;
    }
}
NGINX

    nginx -t && systemctl reload nginx
    echo "  Nginx configured as reverse proxy."
  else
    echo "  Nginx already configured — skipping."
  fi
fi

#--------------------------------------------------
# 6. Enable and start services
#--------------------------------------------------
echo "==> Enabling and starting services..."
systemctl enable basicweigh
systemctl start basicweigh
echo "  BasicWeigh: $(systemctl is-active basicweigh)"

# Only start print agent if kiosk printing is needed
# Uncomment the lines below to enable:
# systemctl enable kioskprint
# systemctl start kioskprint
# echo "  KioskPrint: $(systemctl is-active kioskprint)"

#--------------------------------------------------
# 7. Done
#--------------------------------------------------
echo ""
echo "=========================================="
echo "  Installation complete!"
echo "=========================================="
echo "  Web App:     $APP_DIR"
echo "  Print Agent: $PRINT_DIR"
echo "  Service:     systemctl status basicweigh"
echo "  Logs:        journalctl -u basicweigh -f"
echo ""
echo "  To enable kiosk printing:"
echo "    1. Edit $PRINT_DIR/appsettings.json"
echo "       Set ServerUrl, PrinterName, PrinterId"
echo "    2. systemctl enable kioskprint --now"
echo "=========================================="
