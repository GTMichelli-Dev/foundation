# Deploying to a Debian Server (Vultr, etc.)

[← Back to README](../README.md)

These instructions take you from a brand-new Debian server to a fully running Foundation site with HTTPS.

## Contents

- [What You Need](#what-you-need)
- [Step 1: Point Your Domain to the Server](#step-1-point-your-domain-to-the-server)
- [Step 2: Create a Non-Root User on the Server](#step-2-create-a-non-root-user-on-the-server)
- [Step 3: Build the Deployment Package](#step-3-build-the-deployment-package)
- [Step 4: Deploy to the Server](#step-4-deploy-to-the-server)
- [Step 5: Verify It's Running](#step-5-verify-its-running)
- [That's It!](#thats-it)

## What You Need

| Item | Example |
|------|---------|
| Server IP address | `149.28.xxx.xxx` |
| Root password | (from your Vultr dashboard) |
| Domain name | `yourDNSName.scaledata.net` |
| Email address | `admin@yourcompany.com` |

## Step 1: Point Your Domain to the Server

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

## Step 2: Create a Non-Root User on the Server

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

## Step 3: Build the Deployment Package

On your **local development machine** (Windows/Mac/Linux with .NET 8 SDK installed):

If don't have Git installed (or it's not in your system PATH). Here's how to fix it:
Install Git for Windows

Go to https://git-scm.com/download/win

Download the 64-bit installer

Run the installer — the defaults are fine for most people, just click Next through the options

When done, close and reopen your Command Prompt (important — the old window won't see Git)

```bash
git clone https://github.com/GTMichelli-Dev/foundation.git
cd foundation
```

**Windows (Command Prompt):**
```
deploy\publish.bat
```

**Linux / Mac / Git Bash:**
```bash
bash deploy/publish.sh
```

This builds a self-contained Linux binary and creates `deploy/foundation-deploy.tar.gz`.

## Step 4: Deploy to the Server

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
> - Copies the application to `/opt/foundation`
> - Obtains a free Let's Encrypt SSL certificate for your domain
> - Configures Nginx as a reverse proxy with HTTPS and WebSocket support
> - Creates and starts a systemd service
> - Enables automatic SSL certificate renewal

## Step 5: Verify It's Running

Open your browser and go to:

```
https://yourDNSName.scaledata.net
```

You should see the Foundation dashboard.

## That's It!

Your site is live with HTTPS. The application will automatically start on server reboot.

For routine operations (updating the app, viewing logs, restarting the service), see the [Server Management](../README.md#server-management) section in the main README.

---

[← Back to README](../README.md)
