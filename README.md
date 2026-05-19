# Basic Weigh - Truck Scale Management System

Basic Weigh is a web-based truck scale management application for weighing inbound and outbound trucks, tracking transactions, and generating reports. It runs on ASP.NET Core 8 with a SQLite database and supports touchscreen kiosk terminals with remote ticket printing via Raspberry Pi.

---

## Table of Contents

- [Features](#features)
- [Deployment Guides](#deployment-guides)
  - [Debian Server (Vultr, etc.) — HTTPS](docs/deploy-vultr.md)
  - [Raspberry Pi (LAN only, HTTP)](docs/deploy-pi.md)
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

## Deployment Guides

Pick the path that matches where the app will run. Each guide is self-contained — follow it end to end.

| Target | When to use it | Guide |
|--------|----------------|-------|
| **Debian cloud server** (Vultr, etc.) | Internet-facing site with a real domain and HTTPS. Required if you need access from outside the LAN or want Let's Encrypt SSL. | [docs/deploy-vultr.md](docs/deploy-vultr.md) |
| **Raspberry Pi on the LAN** (HTTP) | Single weigh station, operators on the same local network, no domain or certificate. Reachable at `http://truckscale.local`. | [docs/deploy-pi.md](docs/deploy-pi.md) |

After the app is running, see [Server Management](#server-management) for updates and routine ops, and [Configuration](#configuration) for app settings.

<!-- Legacy heading anchors. The old inline deploy sections used these
     ids; readers with saved deep-links land on the table above and follow
     the link to the new file. -->
<a id="deploying-to-a-debian-server-vultr-etc"></a>
<a id="deploying-to-a-raspberry-pi-lan-only-http"></a>
<a id="updating-the-pi"></a>
<a id="updating-a-remote-pi-via-raspberry-pi-connect"></a>
<a id="changing-the-hostname-later"></a>

<!-- Inline deploy guides removed — see docs/deploy-vultr.md and docs/deploy-pi.md. -->
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

[↑ Back to top](#table-of-contents)

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

[↑ Back to top](#table-of-contents)

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

[↑ Back to top](#table-of-contents)

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

[↑ Back to top](#table-of-contents)

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
