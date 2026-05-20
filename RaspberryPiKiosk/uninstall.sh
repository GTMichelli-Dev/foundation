#!/usr/bin/env bash
# uninstall.sh — remove the kiosk autostart entry and stop any running loop.
# Leaves the scripts and config in place so a re-install is just install.sh again.

set -u

CONFIG_DIR="$HOME/.config/basicweigh-kiosk"
STOP_FLAG="$CONFIG_DIR/STOP"
PID_FILE="$CONFIG_DIR/kiosk.pid"
AUTOSTART_FILE="$HOME/.config/autostart/basicweigh-kiosk.desktop"
PROFILE_DIR="$HOME/.cache/basicweigh-kiosk-profile"

echo "Stopping any running kiosk loop…"
touch "$STOP_FLAG"
if [[ -f "$PID_FILE" ]]; then
    pid="$(cat "$PID_FILE" 2>/dev/null || true)"
    if [[ -n "${pid:-}" ]] && kill -0 "$pid" 2>/dev/null; then
        kill "$pid" 2>/dev/null || true
        sleep 1
        kill -9 "$pid" 2>/dev/null || true
    fi
    rm -f "$PID_FILE"
fi
pkill -f "user-data-dir=$PROFILE_DIR" 2>/dev/null || true

if [[ -f "$AUTOSTART_FILE" ]]; then
    rm -f "$AUTOSTART_FILE"
    echo "Removed autostart: $AUTOSTART_FILE"
else
    echo "No autostart entry found."
fi

rm -f "$STOP_FLAG"

echo
echo "Done. Config + scripts left in place:"
echo "  $CONFIG_DIR"
echo "  $(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo
echo "To fully wipe the kiosk profile: rm -rf $PROFILE_DIR"
