#!/usr/bin/env bash
# install.sh — one-time setup for a Raspberry Pi kiosk pointed at Basic Weigh.
#
# Run on the Pi (the same Pi that will display the kiosk):
#   chmod +x install.sh && ./install.sh
#
# Prompts for the Basic Weigh server URL (e.g. http://truckscale.local),
# verifies that <url>/Kiosk is reachable, then installs an autostart entry
# that launches Chromium in kiosk mode on every desktop login. A watchdog
# restarts Chromium if the page is unreachable for 30 seconds.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIG_DIR="$HOME/.config/basicweigh-kiosk"
CONFIG_FILE="$CONFIG_DIR/config"
AUTOSTART_DIR="$HOME/.config/autostart"
AUTOSTART_FILE="$AUTOSTART_DIR/basicweigh-kiosk.desktop"

say()  { printf '\n\033[1;36m%s\033[0m\n' "$*"; }
warn() { printf '\033[1;33m%s\033[0m\n' "$*" >&2; }
die()  { printf '\033[1;31m%s\033[0m\n' "$*" >&2; exit 1; }

# ---------- sanity ----------
[[ "$(uname -s)" == "Linux" ]] || die "This installer must be run on the Raspberry Pi (Linux), not Windows/macOS."

# ---------- dependencies ----------
say "Checking dependencies…"
need_install=()
command -v curl >/dev/null 2>&1 || need_install+=(curl)
if   command -v chromium-browser >/dev/null 2>&1; then CHROMIUM_BIN="chromium-browser"
elif command -v chromium         >/dev/null 2>&1; then CHROMIUM_BIN="chromium"
else
    need_install+=(chromium-browser)
    CHROMIUM_BIN="chromium-browser"
fi
# unclutter hides the mouse cursor after a few seconds of inactivity — nice for a kiosk
command -v unclutter >/dev/null 2>&1 || need_install+=(unclutter)

if (( ${#need_install[@]} > 0 )); then
    say "Installing: ${need_install[*]}  (sudo apt)"
    sudo apt-get update -y
    sudo apt-get install -y "${need_install[@]}" || die "apt-get install failed."
fi

# Re-resolve chromium in case it was just installed
if   command -v chromium-browser >/dev/null 2>&1; then CHROMIUM_BIN="chromium-browser"
elif command -v chromium         >/dev/null 2>&1; then CHROMIUM_BIN="chromium"
else die "Chromium did not install. Install it manually with: sudo apt-get install chromium-browser"
fi
say "Chromium found at: $(command -v "$CHROMIUM_BIN")"

# ---------- prompt for server URL ----------
default_url=""
if [[ -f "$CONFIG_FILE" ]]; then
    # shellcheck disable=SC1090
    source "$CONFIG_FILE"
    default_url="${SERVER_URL:-}"
fi

while :; do
    if [[ -n "$default_url" ]]; then
        read -r -p "Basic Weigh server URL [$default_url]: " SERVER_URL
        SERVER_URL="${SERVER_URL:-$default_url}"
    else
        read -r -p "Basic Weigh server URL (e.g. http://truckscale.local): " SERVER_URL
    fi

    # Strip trailing slash
    SERVER_URL="${SERVER_URL%/}"

    if [[ -z "$SERVER_URL" ]]; then
        warn "URL is required."
        continue
    fi
    if [[ ! "$SERVER_URL" =~ ^https?:// ]]; then
        warn "URL must start with http:// or https://"
        continue
    fi

    KIOSK_URL="$SERVER_URL/Kiosk"
    say "Verifying connectivity to $KIOSK_URL …"
    if curl --silent --show-error --fail --max-time 10 --output /dev/null "$KIOSK_URL"; then
        say "OK — server responded."
        break
    fi

    http_code="$(curl --silent --show-error --max-time 10 --output /dev/null --write-out '%{http_code}' "$KIOSK_URL" || true)"
    warn "Could not reach $KIOSK_URL (HTTP $http_code)."
    read -r -p "Try a different URL? [Y/n] " ans
    [[ "${ans,,}" == "n" ]] && { warn "Saving anyway — the watchdog will keep retrying."; break; }
    default_url="$SERVER_URL"
done

# ---------- save config ----------
mkdir -p "$CONFIG_DIR"
cat > "$CONFIG_FILE" <<EOF
# Basic Weigh kiosk config — generated $(date -Iseconds)
SERVER_URL="$SERVER_URL"
KIOSK_URL="$SERVER_URL/Kiosk"
CHROMIUM_BIN="$CHROMIUM_BIN"
# How often the watchdog probes the server (seconds)
HEALTH_INTERVAL=5
# After this many seconds of unreachable, restart Chromium
UNREACHABLE_THRESHOLD=30
EOF
say "Wrote config: $CONFIG_FILE"

# ---------- make scripts executable ----------
chmod +x "$SCRIPT_DIR/kiosk-loop.sh" "$SCRIPT_DIR/kiosk-stop" "$SCRIPT_DIR/kiosk-start" "$SCRIPT_DIR/uninstall.sh"

# ---------- install autostart ----------
mkdir -p "$AUTOSTART_DIR"
cat > "$AUTOSTART_FILE" <<EOF
[Desktop Entry]
Type=Application
Name=Basic Weigh Kiosk
Comment=Launches Chromium in kiosk mode pointed at Basic Weigh
Exec=$SCRIPT_DIR/kiosk-loop.sh
X-GNOME-Autostart-enabled=true
NoDisplay=false
Terminal=false
EOF
say "Wrote autostart: $AUTOSTART_FILE"

# ---------- disable screen blanking (best effort) ----------
say "Disabling screen blanking (best effort)…"
if command -v xset >/dev/null 2>&1; then
    # Will only succeed once a desktop session is up; the autostart launch picks it up too.
    DISPLAY="${DISPLAY:-:0}" xset s off s noblank -dpms 2>/dev/null || true
fi

# ---------- disable gnome-keyring ----------
# The kiosk has no human to type an unlock password, and Chromium is launched
# with --password-store=basic so it never asks libsecret for anything. The
# keyring daemon is therefore dead weight (and on autologin sessions it can
# pop an "unlock keyring" dialog on top of the kiosk). Suppress its three
# autostart entries by shadowing them with user-level copies that set
# Hidden=true — XDG honors the user-level file and skips the system one.
say "Disabling gnome-keyring autostart…"
for keyring_entry in gnome-keyring-pkcs11 gnome-keyring-secrets gnome-keyring-ssh; do
    cat > "$AUTOSTART_DIR/${keyring_entry}.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=${keyring_entry} (disabled by Basic Weigh kiosk)
Hidden=true
NoDisplay=true
X-GNOME-Autostart-enabled=false
EOF
done

# ---------- done ----------
cat <<EOF

────────────────────────────────────────────────────────────
  Setup complete.

  Server URL : $SERVER_URL
  Kiosk URL  : $SERVER_URL/Kiosk
  Loop script: $SCRIPT_DIR/kiosk-loop.sh
  Config     : $CONFIG_FILE

  Reboot the Pi to start the kiosk:
      sudo reboot

  To stop the kiosk (e.g. for maintenance) — SSH into the Pi and run:
      $SCRIPT_DIR/kiosk-stop

  To resume after stopping:
      $SCRIPT_DIR/kiosk-start

  To remove autostart entirely:
      $SCRIPT_DIR/uninstall.sh
────────────────────────────────────────────────────────────
EOF
