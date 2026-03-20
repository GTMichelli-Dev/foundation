# Debian Server Setup Script

This script:
- Creates an `admin` user with sudo privileges
- Installs and configures **Nginx** with HTTP (port 80) and HTTPS (port 443)

> ⚠️ **Security Note:** The password is hardcoded for convenience. Change it after first login with `passwd admin`, and replace the self-signed certificate with a real one (e.g., Let's Encrypt) before going to production.

---

## Script: `setup.sh`

```bash
#!/bin/bash
set -euo pipefail

#--------------------------------------------------
# 0. Parameters
#--------------------------------------------------
# Usage: ./setup.sh <domain> <root_password>
# Example: ./setup.sh example.com MyR00tPass!

DOMAIN="${1:-}"
ROOT_PASS="${2:-}"

if [[ -z "$DOMAIN" ]]; then
  echo "ERROR: No domain name provided." >&2
  echo "Usage: ./setup.sh <domain> <root_password>" >&2
  echo "Example: ./setup.sh example.com MyR00tPass!" >&2
  exit 1
fi

if [[ -z "$ROOT_PASS" ]]; then
  echo "ERROR: No root password provided." >&2
  echo "Usage: ./setup.sh <domain> <root_password>" >&2
  echo "Example: ./setup.sh example.com MyR00tPass!" >&2
  exit 1
fi

echo "==> Domain set to: $DOMAIN"

#--------------------------------------------------
# 1. Elevate to root using provided password if needed
#--------------------------------------------------
if [[ $EUID -ne 0 ]]; then
  echo "==> Not running as root — elevating with provided password..."
  exec echo "$ROOT_PASS" | sudo -S bash "$0" "$DOMAIN" "$ROOT_PASS"
  exit $?
fi

echo "==> Running as root. Starting Debian server setup..."

#--------------------------------------------------
# 1. Create admin user with password and sudo rights
#--------------------------------------------------
USERNAME="admin"
PASSWORD="Scale_Us3r"

if id "$USERNAME" &>/dev/null; then
  echo "==> User '$USERNAME' already exists — skipping creation."
else
  echo "==> Creating user '$USERNAME'..."
  useradd -m -s /bin/bash "$USERNAME"
  echo "$USERNAME:$PASSWORD" | chpasswd
  echo "==> User created."
fi

# Add to sudo group (Debian standard)
usermod -aG sudo "$USERNAME"
echo "==> '$USERNAME' added to sudo group."

#--------------------------------------------------
# 2. Update package list and install Nginx
#--------------------------------------------------
echo "==> Updating packages..."
apt-get update -qq

echo "==> Installing Nginx and OpenSSL..."
apt-get install -y nginx openssl

#--------------------------------------------------
# 3. Generate a self-signed SSL certificate
#--------------------------------------------------
SSL_DIR="/etc/nginx/ssl"
mkdir -p "$SSL_DIR"

if [[ ! -f "$SSL_DIR/nginx.crt" ]]; then
  echo "==> Generating self-signed SSL certificate (valid 365 days)..."
  openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout "$SSL_DIR/nginx.key" \
    -out    "$SSL_DIR/nginx.crt" \
    -subj "/C=US/ST=State/L=City/O=Organization/CN=$DOMAIN"
  chmod 600 "$SSL_DIR/nginx.key"
  echo "==> Certificate created at $SSL_DIR/"
else
  echo "==> SSL certificate already exists — skipping generation."
fi

#--------------------------------------------------
# 4. Write Nginx configuration (HTTP + HTTPS)
#--------------------------------------------------
NGINX_CONF="/etc/nginx/sites-available/default"

echo "==> Writing Nginx configuration..."
cat > "$NGINX_CONF" <<EOF
# --------------------------------------------------
# HTTP — redirect all traffic to HTTPS
# --------------------------------------------------
server {
    listen 80 default_server;
    listen [::]:80 default_server;

    server_name $DOMAIN www.$DOMAIN;

    # Redirect to HTTPS
    return 301 https://\$host\$request_uri;
}

# --------------------------------------------------
# HTTPS — main server block
# --------------------------------------------------
server {
    listen 443 ssl default_server;
    listen [::]:443 ssl default_server;

    server_name $DOMAIN www.$DOMAIN;

    # SSL certificate paths
    ssl_certificate     /etc/nginx/ssl/nginx.crt;
    ssl_certificate_key /etc/nginx/ssl/nginx.key;

    # Modern SSL settings
    ssl_protocols       TLSv1.2 TLSv1.3;
    ssl_ciphers         HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    # Security headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Content-Type-Options nosniff always;
    add_header X-Frame-Options DENY always;

    root /var/www/html;
    index index.html index.htm;

    location / {
        try_files \$uri \$uri/ =404;
    }
}
EOF

#--------------------------------------------------
# 5. Enable site, test config, and restart Nginx
#--------------------------------------------------
ln -sf /etc/nginx/sites-available/default /etc/nginx/sites-enabled/default

echo "==> Testing Nginx configuration..."
nginx -t

echo "==> Enabling and restarting Nginx..."
systemctl enable nginx
systemctl restart nginx

#--------------------------------------------------
# 6. Open ports via UFW (if installed)
#--------------------------------------------------
if command -v ufw &>/dev/null; then
  echo "==> Configuring UFW firewall..."
  ufw allow 80/tcp   comment "HTTP"
  ufw allow 443/tcp  comment "HTTPS"
  ufw allow 22/tcp   comment "SSH"
  ufw --force enable
  echo "==> UFW rules applied."
else
  echo "==> UFW not found — skipping firewall configuration."
  echo "    Manually open ports 80 and 443 if you have a firewall in place."
fi

#--------------------------------------------------
# 7. Done
#--------------------------------------------------
echo ""
echo "=========================================="
echo "  Setup complete!"
echo "=========================================="
echo "  User     : $USERNAME"
echo "  Password  : $PASSWORD  <-- CHANGE THIS"
echo "  Domain    : $DOMAIN"
echo "  HTTP      : port 80  (redirects to HTTPS)"
echo "  HTTPS     : port 443 (self-signed cert)"
echo "  Nginx     : $(systemctl is-active nginx)"
echo "=========================================="
echo "  Next steps:"
echo "  1. Change the admin password:  passwd admin"
echo "  2. Replace the self-signed cert with Let's Encrypt:"
echo "     apt install certbot python3-certbot-nginx"
echo "     certbot --nginx -d $DOMAIN -d www.$DOMAIN"
echo "=========================================="
```

---

## Usage

```bash
# 1. Save the script
nano setup.sh

# 2. Make it executable
chmod +x setup.sh

# 3. Run with domain and root password — no sudo prefix needed
./setup.sh example.com MyR00tPass!
```

---

## What the Script Does — Step by Step

| Step | Action |
|------|--------|
| 0 | Validates `<domain>` and `<root_password>` args; self-elevates via `sudo -S` if not already root |
| 1 | Creates user `admin` with password `Scale_Us3r` and adds to `sudo` group |
| 2 | Runs `apt-get update` and installs `nginx` + `openssl` |
| 3 | Generates a self-signed TLS certificate in `/etc/nginx/ssl/` |
| 4 | Writes an Nginx config: port **80 → redirects to HTTPS**, port **443 → serves content** |
| 5 | Tests the Nginx config (`nginx -t`) and restarts the service |
| 6 | Opens ports 80, 443, and 22 in UFW (if UFW is present) |

---

## Recommended Follow-up: Let's Encrypt (Free Trusted Cert)

```bash
apt install -y certbot python3-certbot-nginx
certbot --nginx -d yourdomain.com -d www.yourdomain.com
```

Certbot will automatically update the Nginx config and handle renewals.
