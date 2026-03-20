# Basic Weigh - Truck Scale Management System

Basic Weigh is a web-based truck scale management application for weighing inbound and outbound trucks, tracking transactions, and generating reports. It runs on ASP.NET Core 8 with a SQLite database and supports touchscreen kiosk terminals with remote ticket printing via Raspberry Pi.

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

> DNS can take a few minutes to propagate. You can verify with: `ping scale.yourcompany.com`

### Step 2: Create a Non-Root User on the Server

SSH into your new server as root:

```bash
ssh root@149.28.xxx.xxx
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

### Step 3: Build the Deployment Package

On your **local development machine** (Windows/Mac/Linux with .NET 8 SDK installed):

```bash
git clone https://github.com/TotalScaleService/Basic_Weigh.git
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
deploy\deploy.bat admin@149.28.xxx.xxx --domain scale.yourcompany.com --email admin@yourcompany.com
```

**Linux / Mac / Git Bash:**
```bash
bash deploy/deploy.sh admin@149.28.xxx.xxx --domain scale.yourcompany.com --email admin@yourcompany.com
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
https://scale.yourcompany.com
```

You should see the Basic Weigh dashboard.

### That's It!

Your site is live with HTTPS. The application will automatically start on server reboot.

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

```bash
bash deploy/publish.sh
bash deploy/deploy.sh admin@149.28.xxx.xxx --domain yourDNSName.scaledata.net --email admin@yourcompany.com
```

Or as a single step (deploy.sh will run publish.sh automatically if the tarball doesn't exist):

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
- **System** — Demo mode, kiosk count (0/1/2), theme, custom icon
- **Kiosk Prompts** — Which fields to show on the kiosk touchscreen
- **Ticket Designers** — Edit the layout of printed tickets

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
