# Deploying to a Raspberry Pi (LAN only, HTTP)

[← Back to README](../README.md)

If the scale only needs to be reached from the local network, the web app can run directly on a Raspberry Pi — no domain, no certificates, no cloud server. Pick a friendly hostname like `truckscale` and operators reach the site at `http://truckscale.local` from any computer on the same network (mDNS / Bonjour, built in to macOS, Windows 10+, and most Linux desktops).

> **HTTP only — LAN only.** This setup has no TLS. Only use it on networks you control (a private weigh-station LAN, not open Wi-Fi). QuickBooks sync requires the Windows `QBSyncService` on a machine that can reach the Pi, so QB integration is optional in this deployment.

## Contents

- [What You Need](#what-you-need)
- [Step 1: Flash and Prepare the Pi](#step-1-flash-and-prepare-the-pi)
- [Step 2: Install Dependencies on the Pi](#step-2-install-dependencies-on-the-pi)
- [Step 3: Cross-Publish the Web App from Your Workstation](#step-3-cross-publish-the-web-app-from-your-workstation)
- [Step 4: Allow Kestrel to Bind to Port 80](#step-4-allow-kestrel-to-bind-to-port-80)
- [Step 5: Create the systemd Service](#step-5-create-the-systemd-service)
- [Step 6: Verify](#step-6-verify)
- [Updating the Pi](#updating-the-pi)
- [Updating a Remote Pi via Raspberry Pi Connect](#updating-a-remote-pi-via-raspberry-pi-connect)
- [Changing the Hostname Later](#changing-the-hostname-later)

## What You Need

| Item | Example |
|------|---------|
| Raspberry Pi 4 or 5 (64-bit) | 2 GB RAM minimum |
| MicroSD card with Raspberry Pi OS Lite (64-bit) | 32 GB recommended |
| Local network | Wired Ethernet preferred over Wi-Fi |
| Workstation with .NET 8 SDK | Windows / Mac / Linux |

## Step 1: Flash and Prepare the Pi

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

## Step 2: Install Dependencies on the Pi

```bash
sudo apt update
sudo apt install -y libicu-dev libsqlite3-0 avahi-daemon libcap2-bin
```

Avahi is normally already running. Confirm with:

```bash
systemctl status avahi-daemon
```

(No .NET runtime is needed — Step 3 publishes a self-contained binary.)

## Step 3: Cross-Publish the Web App from Your Workstation

On your **development machine** (with the .NET 8 SDK and Git installed):

```bash
git clone https://github.com/GTMichelli-Dev/Basic_Weigh.git
cd Basic_Weigh
dotnet publish web/BasicWeigh.Web/BasicWeigh.Web.csproj 
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

## Step 4: Allow Kestrel to Bind to Port 80

So that operators can use `http://truckscale.local` with no port number, grant the binary the capability to bind to a low port without running as root:

```bash
ssh admin@truckscale.local
sudo setcap 'cap_net_bind_service=+ep' /opt/basicweigh/BasicWeigh.Web
```

(You'll re-run this command on the Pi every time you replace the binary.)

## Step 5: Create the systemd Service

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

## Step 6: Verify

From any computer on the same network, open:

```
http://truckscale.local
```

You should see the Basic Weigh dashboard. The Pi will start the app automatically on reboot.

## Updating the Pi

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

## Updating a Remote Pi via Raspberry Pi Connect

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

## Changing the Hostname Later

If you want to rename the device after install:

```bash
ssh admin@truckscale.local
sudo hostnamectl set-hostname newname
sudo sed -i "s/truckscale/newname/g" /etc/hosts
sudo systemctl restart avahi-daemon
```

Reboot to be sure all services pick up the new name. The site will then be reachable at `http://newname.local`.

---

[← Back to README](../README.md)
