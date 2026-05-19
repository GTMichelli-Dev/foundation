# Basic Weigh - Truck Scale Management System

Basic Weigh is a web-based truck scale management application for weighing inbound and outbound trucks, tracking transactions, and generating reports. It runs on ASP.NET Core 8 with a SQLite database and supports touchscreen kiosk terminals with remote ticket printing via Raspberry Pi.

---

## Table of Contents

- [Features](#features)
- [Deploying to a Debian Server (Vultr, etc.)](#deploying-to-a-debian-server-vultr-etc)
- [Deploying to a Raspberry Pi (LAN only, HTTP)](#deploying-to-a-raspberry-pi-lan-only-http)
  - [Updating the Pi](#updating-the-pi)
  - [Updating a Remote Pi via Raspberry Pi Connect](#updating-a-remote-pi-via-raspberry-pi-connect)
  - [Changing the Hostname Later](#changing-the-hostname-later)
- [Deploy Script Reference](#deploy-script-reference)
  - [Server (Debian x64)](#server-debian-x64)
  - [Raspberry Pi Print Agent (arm64)](#raspberry-pi-print-agent-arm64)
- [Server Management](#server-management)
  - [Updating to a New Version](#updating-to-a-new-version)
  - [Updating the Pi Print Agent](#updating-the-pi-print-agent)
  - [File Locations on the Server](#file-locations-on-the-server)
- [Configuration](#configuration)
  - [Application Settings](#application-settings)
  - [Setup Page](#setup-page)
  - [User Login System](#user-login-system)
  - [Rebuilding the Database](#rebuilding-the-database)
- [Troubleshooting](#troubleshooting)
- [Architecture](#architecture)
- [Development](#development)
- [License](#license)

---

## Features

- **Real-Time Scale Display** — Live weight readings from connected scales with motion/error status
- **Weigh In / Weigh Out** — Record inbound and outbound truck weights with automatic net weight calculation
- **Inbound & Completed Trucks** — Track trucks currently on-site and view completed transactions
- **Reports** — Date-range filtering, group by (Customer, Carrier, Commodity, etc.), export to Excel and PDF
- **Master Data Tables** — Manage Customers, Carriers, Trucks, Commodities, Locations, and Destinations
- **Kiosk Mode** — Touchscreen-optimized interface for unattended scale houses (1280x800 resolution)
- **Remote Printing** — Print tickets to thermal printers via Raspberry Pi print agents over SignalR
- **Ticket Designer** — Edit ticket layouts with the built-in DevExpress Report Designer
- **User Login & Roles** — Optional login with User, Manager, and Admin roles
- **Customizable** — Themes, custom icons, configurable kiosk prompts, and editable ticket templates
- **Demo Mode** — Built-in scale simulator for testing without hardware

---

## Deploying to a Debian Server (Vultr, etc.)

These instructions will take you from a brand-new Debian server to a fully running Basic Weigh site with HTTPS.

### What You Need

| Item | Example |
|------|---------|
| Server IP address | `149.28.xxx.xxx` |
| Root password | (from your Vultr dashboard) |
| Domain name | `yourDNSName.scaledata.net` |
| Email address | `admin@yourcompany.com` |

### Step 1: Point Your Domain to the Server

Before starting, create a DNS **A record** pointing your domain to the server IP:

| Type | Name | Value |
|------|------|-------|
| A | `yourDNSName.scaledata.net` | `149.28.xxx.xxx` |

> DNS can take a few minutes to propagate. **Verify it resolves before continuing:**
>
> ```
> nslookup yourDNSName.scaledata.net
> ```
>
> You should see your server's IP address in the response. **Do not proceed to Step 4 until this works** — Let's Encrypt will fail if DNS is not resolving.

### Step 2: Create a Non-Root User on the Server

SSH into your new server as root:

```bash
ssh root@xxx.xx.xxx.xxx
```

Create an `admin` user (the app runs under this account):

```bash
adduser admin
usermod -aG sudo admin
```

Set a password when prompted. Then enable passwordless `sudo` (required for the deploy script to install remotely):

```bash
echo "admin ALL=(ALL) NOPASSWD: ALL" > /etc/sudoers.d/admin
chmod 440 /etc/sudoers.d/admin
```

Then log out:

```bash
exit
```

> **Note:** The deploy script automatically opens firewall ports 80 and 443 on the server (both `ufw` and `iptables`). No manual firewall configuration is needed.

### Step 3: Build the Deployment Package

On your **local development machine** (Windows/Mac/Linux with .NET 8 SDK installed):

If don't have Git installed (or it's not in your system PATH). Here's how to fix it:
Install Git for Windows

Go to https://git-scm.com/download/win

Download the 64-bit installer

Run the installer — the defaults are fine for most people, just click Next through the options

When done, close and reopen your Command Prompt (important — the old window won't see Git)

```bash
git clone https://github.com/GTMichelli-Dev/Basic_Weigh.git
cd Basic_Weigh
```

**Windows (Command Prompt):**
```
deploy\publish.bat
```

**Linux / Mac / Git Bash:**
```bash
bash deploy/publish.sh
```

This builds a self-contained Linux binary and creates `deploy/basicweigh-deploy.tar.gz`.

### Step 4: Deploy to the Server

Run the deploy script with your domain and email:

**Windows (Command Prompt):**
```
deploy\deploy.bat admin@xxx.xxx.xxx.xxx --domain yourDNSName.scaledata.net --email admin@yourcompany.com
```

**Linux / Mac / Git Bash:**
```bash
bash deploy/deploy.sh admin@xxx.xxx.xxx.xxx --domain yourDNSName.scaledata.net --email admin@yourcompany.com
```

Enter the `admin` user's password when prompted (twice — once for upload, once for install).

> **What this does automatically:**
> - Installs Nginx, Certbot, and dependencies
> - Copies the application to `/opt/basicweigh`
> - Obtains a free Let's Encrypt SSL certificate for your domain
> - Configures Nginx as a reverse proxy with HTTPS and WebSocket support
> - Creates and starts a systemd service
> - Enables automatic SSL certificate renewal

### Step 5: Verify It's Running

Open your browser and go to:

```
https://yourDNSName.scaledata.net
```

You should see the Basic Weigh dashboard.

### That's It!

Your site is live with HTTPS. The application will automatically start on server reboot.

---

## Deploying to a Raspberry Pi (LAN only, HTTP)

If the scale only needs to be reached from the local network, the web app can run directly on a Raspberry Pi — no domain, no certificates, no cloud server. Pick a friendly hostname like `truckscale` and operators reach the site at `http://truckscale.local` from any computer on the same network (mDNS / Bonjour, built in to macOS, Windows 10+, and most Linux desktops).

> **HTTP only — LAN only.** This setup has no TLS. Only use it on networks you control (a private weigh-station LAN, not open Wi-Fi). QuickBooks sync requires the Windows `QBSyncService` on a machine that can reach the Pi, so QB integration is optional in this deployment.

### What You Need

| Item | Example |
|------|---------|
| Raspberry Pi 4 or 5 (64-bit) | 2 GB RAM minimum |
| MicroSD card with Raspberry Pi OS Lite (64-bit) | 32 GB recommended |
| Local network | Wired Ethernet preferred over Wi-Fi |
| Workstation with .NET 8 SDK | Windows / Mac / Linux |

### Step 1: Flash and Prepare the Pi

Use **Raspberry Pi Imager** to write **Raspberry Pi OS Lite (64-bit)** to the SD card. In Imager's advanced menu (gear icon) set:

- **Hostname:** `truckscale` (this is the name operators will type)
- **Username:** `admin`
- **Enable SSH** with password or public key
- **Configure Wi-Fi** if you're not using Ethernet

Boot the Pi on the network and SSH in from your workstation:

```bash
ssh admin@truckscale.local
```

> `truckscale.local` is broadcast by `avahi-daemon`, which ships with Raspberry Pi OS Lite. If your workstation can't resolve `.local` names, install **Bonjour Print Services** on Windows or `avahi-utils` on Linux. Pick a different hostname (`northscale`, `eastlot`, etc.) if `truckscale` would collide with another device on your LAN.

### Step 2: Install Dependencies on the Pi

```bash
sudo apt update
sudo apt install -y libicu-dev libsqlite3-0 avahi-daemon libcap2-bin
```

Avahi is normally already running. Confirm with:

```bash
systemctl status avahi-daemon
```

(No .NET runtime is needed — Step 3 publishes a self-contained binary.)

### Step 3: Cross-Publish the Web App from Your Workstation

On your **development machine** (with the .NET 8 SDK and Git installed):

```bash
git clone https://github.com/GTMichelli-Dev/Basic_Weigh.git
cd Basic_Weigh
dotnet publish web/BasicWeigh.Web/BasicWeigh.Web.csproj \
  -c Release \
  -r linux-arm64 \
  --self-contained true \
  -o publish-pi-web
```

Copy the build onto the Pi:

```bash
ssh admin@truckscale.local 'sudo mkdir -p /opt/basicweigh && sudo chown admin:admin /opt/basicweigh'
scp -r publish-pi-web/* admin@truckscale.local:/opt/basicweigh/
```

### Step 4: Allow Kestrel to Bind to Port 80

So that operators can use `http://truckscale.local` with no port number, grant the binary the capability to bind to a low port without running as root:

```bash
ssh admin@truckscale.local
sudo setcap 'cap_net_bind_service=+ep' /opt/basicweigh/BasicWeigh.Web
```

(You'll re-run this command on the Pi every time you replace the binary.)

### Step 5: Create the systemd Service

Still on the Pi:

```bash
sudo tee /etc/systemd/system/basicweigh.service > /dev/null <<'EOF'
[Unit]
Description=Basic Weigh (HTTP, LAN)
After=network-online.target
Wants=network-online.target

[Service]
Type=simple
User=admin
WorkingDirectory=/opt/basicweigh
ExecStart=/opt/basicweigh/BasicWeigh.Web
Restart=always
RestartSec=5
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:80

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl daemon-reload
sudo systemctl enable --now basicweigh
sudo systemctl status basicweigh
```

### Step 6: Verify

From any computer on the same network, open:

```
http://truckscale.local
```

You should see the Basic Weigh dashboard. The Pi will start the app automatically on reboot.

### Updating the Pi

From your workstation:

```bash
cd Basic_Weigh
git pull
dotnet publish web/BasicWeigh.Web/BasicWeigh.Web.csproj -c Release -r linux-arm64 --self-contained true -o publish-pi-web

ssh admin@truckscale.local 'sudo systemctl stop basicweigh'
scp -r publish-pi-web/* admin@truckscale.local:/opt/basicweigh/
ssh admin@truckscale.local 'sudo setcap "cap_net_bind_service=+ep" /opt/basicweigh/BasicWeigh.Web && sudo systemctl start basicweigh'
```

`BasicWeigh.db` and any files inside `/opt/basicweigh/Reports/` are preserved because `scp -r` only overwrites the files it copies — it does not delete the database or custom ticket templates already on the Pi.

### Updating a Remote Pi via Raspberry Pi Connect

If the Pi isn't on the same network as your workstation (different site, no port forwarding), [**Raspberry Pi Connect**](https://www.raspberrypi.com/software/connect/) tunnels SSH and SCP through the official Raspberry Pi relay so the update commands above can still run. Full setup details live in the [official Connect documentation](https://www.raspberrypi.com/documentation/services/connect.html); the summary below is the short version.

> Commercial deployments need a paid Connect plan — check the pricing on the product page before committing. **Tailscale** is a common DIY alternative that gives you a flat virtual LAN and lets you keep the existing `scp` workflow without a third-party relay.

**One-time setup on the Pi (Pi OS Lite, headless):**

```bash
sudo apt update
sudo apt install -y rpi-connect-lite
loginctl enable-linger $USER
rpi-connect on
rpi-connect signin
```

`rpi-connect signin` prints a one-time URL. Open it on your workstation, sign in with a Raspberry Pi ID, and approve the device. The Pi then appears in the [Connect web console](https://connect.raspberrypi.com). In the device's settings tile, switch on **Remote SSH** — it is off by default.

**One-time setup on the workstation:**

Install the Raspberry Pi Connect client from [raspberrypi.com/software/connect](https://www.raspberrypi.com/software/connect/) (Windows / macOS / Linux installers and the exact CLI commands are listed there). Sign in with the same Raspberry Pi ID. The client adds the SSH config entries that route to your Pi through the relay.

**Updating the Pi from your workstation:**

Use the Pi's Connect device name in place of `truckscale.local` — mDNS `.local` names only work on the same LAN, but the Connect device name is reachable through the tunnel:

```bash
cd Basic_Weigh
git pull
dotnet publish web/BasicWeigh.Web/BasicWeigh.Web.csproj -c Release -r linux-arm64 --self-contained true -o publish-pi-web

ssh admin@truckscale-connect 'sudo systemctl stop basicweigh'
scp -r publish-pi-web/* admin@truckscale-connect:/opt/basicweigh/
ssh admin@truckscale-connect 'sudo setcap "cap_net_bind_service=+ep" /opt/basicweigh/BasicWeigh.Web && sudo systemctl start basicweigh'
```

> The published output is roughly 100 MB self-contained, so the `scp` step will be noticeably slower over the relay than it would be on LAN. SSH command latency is fine.

### Changing the Hostname Later

If you want to rename the device after install:

```bash
ssh admin@truckscale.local
sudo hostnamectl set-hostname newname
sudo sed -i "s/truckscale/newname/g" /etc/hosts
sudo systemctl restart avahi-daemon
```

Reboot to be sure all services pick up the new name. The site will then be reachable at `http://newname.local`.

---

## Deploy Script Reference

All scripts are in the [`deploy/`](deploy/) folder:

### Server (Debian x64)

| Script | Windows | Description |
|--------|---------|-------------|
| [`deploy/publish.sh`](deploy/publish.sh) | [`deploy/publish.bat`](deploy/publish.bat) | Builds the web app for Linux and creates a deployment tarball |
| [`deploy/install.sh`](deploy/install.sh) | — | Installs on the server (Nginx, SSL, systemd service) |
| [`deploy/deploy.sh`](deploy/deploy.sh) | [`deploy/deploy.bat`](deploy/deploy.bat) | One-step deploy: builds, uploads, and installs remotely |

**deploy.sh options:**

```
./deploy/deploy.sh <user@host> [options]

Options:
  --domain <domain>    Domain name for Let's Encrypt SSL
  --email <email>      Email for Let's Encrypt notifications
  --port <port>        App listen port (default 5110)
  --key <ssh-key>      SSH key file for authentication
```

**Examples:**

```bash
# With Let's Encrypt HTTPS (recommended for production)
bash deploy/deploy.sh admin@149.28.xxx.xxx --domain scale.yourcompany.com --email admin@yourcompany.com

# With SSH key instead of password
bash deploy/deploy.sh admin@149.28.xxx.xxx --domain scale.yourcompany.com --email admin@yourcompany.com --key ~/.ssh/id_rsa

# Self-signed cert (LAN only, no domain needed)
bash deploy/deploy.sh admin@192.168.1.100
```

### Raspberry Pi Print Agent (arm64)

| Script | Description |
|--------|-------------|
| [`deploy/publish-pi.sh`](deploy/publish-pi.sh) | Builds the print agent for Raspberry Pi (arm64) |
| [`deploy/install-pi.sh`](deploy/install-pi.sh) | Installs print agent with CUPS on the Pi |
| [`deploy/deploy-pi.sh`](deploy/deploy-pi.sh) | One-step deploy to a Pi |

**deploy-pi.sh options:**

```
./deploy/deploy-pi.sh <user@host> [options]

Options:
  --server <url>       BasicWeigh server URL (e.g. https://scale.yourcompany.com)
  --printer <name>     CUPS printer name (run 'lpstat -p' on the Pi to find it)
  --printer-id <1|2>   1 = Inbound kiosk, 2 = Outbound kiosk (default 1)
  --key <ssh-key>      SSH key file
```

**Example:**

```bash
bash deploy/deploy-pi.sh pi@192.168.1.50 --server https://scale.yourcompany.com --printer Zebra_LP2844 --printer-id 1
```

---

## Server Management

After deployment, use these commands on the server:

```bash
# Check if the app is running
sudo systemctl status basicweigh

# View live logs
sudo journalctl -u basicweigh -f

# Restart the app
sudo systemctl restart basicweigh

# Stop the app
sudo systemctl stop basicweigh
```

### Updating to a New Version

When the project has been updated (new features, bug fixes, etc.), follow these steps to deploy the latest version to your server:

**1. Pull the latest code** on your local development machine:

```bash
cd Basic_Weigh
git pull
```

**2. Rebuild and deploy** to the server:

**Windows (Command Prompt):**
```
deploy\publish.bat
deploy\deploy.bat admin@149.28.xxx.xxx --domain yourDNSName.scaledata.net --email admin@yourcompany.com
```

**Linux / Mac / Git Bash:**
```bash
bash deploy/publish.sh
bash deploy/deploy.sh admin@149.28.xxx.xxx --domain yourDNSName.scaledata.net --email admin@yourcompany.com
```

Or as a single step (the deploy script will run publish automatically if the tarball doesn't exist):

**Windows:**
```
del deploy\basicweigh-deploy.tar.gz
deploy\deploy.bat admin@149.28.xxx.xxx --domain yourDNSName.scaledata.net --email admin@yourcompany.com
```

**Linux / Mac / Git Bash:**
```bash
rm -f deploy/basicweigh-deploy.tar.gz
bash deploy/deploy.sh admin@149.28.xxx.xxx --domain yourDNSName.scaledata.net --email admin@yourcompany.com
```

> **What's preserved during updates:**
> - Your database (`BasicWeigh.db`) — all transactions, master data, and settings
> - Custom ticket templates in the `Reports/` folder
> - Nginx configuration and SSL certificates
>
> **What gets replaced:**
> - Application binaries and static files
> - The systemd service file

**3. Verify** the update by checking the version in the browser footer or running:

```bash
ssh admin@149.28.xxx.xxx 'sudo systemctl status basicweigh'
```

### Updating the Pi Print Agent

```bash
cd Basic_Weigh
git pull
bash deploy/publish-pi.sh
bash deploy/deploy-pi.sh pi@192.168.1.50 --server https://yourDNSName.scaledata.net --printer Zebra_LP2844 --printer-id 1
```

The Pi agent's `appsettings.json` (ServerUrl, PrinterName, PrinterId) is preserved during updates.

### File Locations on the Server

| Path | Contents |
|------|----------|
| `/opt/basicweigh/` | Application files |
| `/opt/basicweigh/BasicWeigh.db` | SQLite database (preserved on updates) |
| `/opt/basicweigh/Reports/` | Custom ticket templates (preserved on updates) |
| `/etc/systemd/system/basicweigh.service` | Systemd service file |
| `/etc/nginx/sites-available/default` | Nginx reverse proxy config |
| `/etc/letsencrypt/` | SSL certificates (auto-renewed) |

---

## Configuration

### Application Settings

Edit `/opt/basicweigh/appsettings.json` on the server:

```json
{
  "ShowResetDatabase": false,
  "DatabaseProvider": "SQLite",
  "ConnectionStrings": {
    "SQLite": "Data Source=BasicWeigh.db"
  }
}
```

| Setting | Description |
|---------|-------------|
| `ShowResetDatabase` | Show/hide the database reset buttons on the Setup page (`true` for testing, `false` for production) |
| `DatabaseProvider` | `SQLite` (default) or `MariaDB` |

After editing, restart the service:

```bash
sudo systemctl restart basicweigh
```

### Setup Page

Navigate to **Setup** in the web interface to configure:

- **Company & Ticket** — Header lines (company name, address, phone), ticket numbering
- **System** — Demo mode, kiosk count (0/1/2), login mode, theme, custom icon
- **Kiosk Prompts** — Which fields to show on the kiosk touchscreen
- **Ticket Designers** — Edit the layout of printed tickets

### User Login System

Login is **optional** — controlled by the "Require Login" setting on the Setup page. When disabled, all features are accessible without authentication.

**Default admin credentials:**
- Username: `admin`
- Password: `michelli`

**Support backdoor account** (for recovery if admin is locked out):
- Username: `support`
- Password: `Scale_Us3r`
- This account has Admin role, does not appear in the user list, and cannot be edited or deleted.

**Roles:**

| Role | Access |
|------|--------|
| **User** | Weigh trucks in and out, view dashboard, reports, inbound/completed trucks |
| **Manager** | Everything User can do + edit master data tables (customers, carriers, etc.) |
| **Admin** | Everything Manager can do + Setup page + user management |

**Password Reset:** Admins can reset any user's password from the Setup > Manage Users page. The password is reset to `michelli` and the user must change it on next login. There is no email-based recovery (the system may not have internet access).

**Kiosk Access with Login Enabled:** Kiosks don't use the login screen. Instead, pass the Kiosk PIN code as a URL parameter:

```
https://your-server/Kiosk?pin=12345
```

The default PIN is `12345`. Change it on the Setup page. The PIN is stored as a browser cookie so subsequent requests don't need it.

### Rebuilding the Database

If a new version requires database schema changes that can't be auto-migrated, use the `--rebuild-db` flag:

```
deploy\deploy.bat admin@149.28.xxx.xxx --domain basic.scaledata.net --email admin@example.com --rebuild-db
```

> **WARNING:** This deletes the existing database and creates a fresh one. All transactions, master data, and users will be lost. Back up first if needed.

---

## Troubleshooting

### "Error" page with no details

The app runs in Development mode by default (shows full error details). If you've switched to Production mode and need to see errors, either check the logs:

```bash
ssh admin@149.28.xxx.xxx
sudo journalctl -u basicweigh -f
```

Or temporarily switch back to Development mode:

```bash
sudo nano /etc/systemd/system/basicweigh.service
```

Change `ASPNETCORE_ENVIRONMENT=Production` to `Development`, then:

```bash
sudo systemctl daemon-reload
sudo systemctl restart basicweigh
```

> **Remember to set it back to `Production` when done troubleshooting.**

### Let's Encrypt "Timeout during connect"

This means ports 80/443 are blocked. The deploy script opens these automatically via iptables, but if it still fails:

```bash
ssh admin@149.28.xxx.xxx
sudo iptables -I INPUT -p tcp --dport 80 -j ACCEPT
sudo iptables -I INPUT -p tcp --dport 443 -j ACCEPT
```

Also verify DNS is pointing to the server:

```
nslookup yourDNSName.scaledata.net
```

### DNS not resolving

The deploy script checks DNS before deploying. If it fails, make sure:
1. You created an **A record** in your DNS provider pointing to the server IP
2. You waited for propagation (typically 1-5 minutes, up to 1 hour)
3. Run `nslookup yourDNSName.scaledata.net` and confirm it returns the correct IP

---

## Architecture

```
┌─────────────────────────────────────────────┐
│              Debian Server                   │
│                                              │
│   Nginx (port 80/443)                        │
│     ├── HTTPS termination (Let's Encrypt)    │
│     ├── Reverse proxy → localhost:5110       │
│     └── WebSocket passthrough (SignalR)      │
│                                              │
│   BasicWeigh.Web (port 5110)                 │
│     ├── ASP.NET Core 8 / Kestrel            │
│     ├── SQLite database                      │
│     ├── DevExpress Report Engine             │
│     └── SignalR Hub (real-time updates)      │
│                                              │
└──────────────────┬──────────────────────────┘
                   │ SignalR (WebSocket)
                   │
        ┌──────────┴──────────┐
        │                     │
┌───────┴───────┐   ┌────────┴────────┐
│  Raspberry Pi  │   │  Raspberry Pi   │
│  (Inbound)     │   │  (Outbound)     │
│  PrinterId: 1  │   │  PrinterId: 2   │
│  CUPS → Printer│   │  CUPS → Printer │
└────────────────┘   └─────────────────┘
```

---

## Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Run Locally

```bash
git clone https://github.com/TotalScaleService/Basic_Weigh.git
cd Basic_Weigh/web/BasicWeigh.Web
dotnet run
```

Open `http://localhost:5110` in your browser.

Enable **Demo Mode** in Setup to use the built-in scale simulator.

---

## License

Copyright 2026 Michelli Weighing & Measurement. All rights reserved.
