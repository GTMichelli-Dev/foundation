#!/bin/bash
set -euo pipefail

# Deploy the Foundation web app to a Raspberry Pi (Kestrel direct, no nginx)
# Usage: ./deploy-pi-web.sh <user@host> [options]
#
# Options:
#   --port <port>        App listen port (default 80)
#   --key <ssh-key>      SSH key file
#   --rebuild-db         Delete and recreate the database (WARNING: deletes all data)
#
# Examples:
#   ./deploy-pi-web.sh admin@192.168.1.60
#   ./deploy-pi-web.sh admin@192.168.1.60 --port 8080 --key ~/.ssh/id_rsa

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARBALL="$SCRIPT_DIR/foundation-pi-deploy.tar.gz"

# Parse arguments
REMOTE=""
APP_PORT="80"
SSH_KEY=""
REBUILD_DB="0"
SKIP_BUILD="0"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --port)       APP_PORT="$2"; shift 2 ;;
    --key)        SSH_KEY="$2";  shift 2 ;;
    --rebuild-db) REBUILD_DB="1"; shift ;;
    --skip-build) SKIP_BUILD="1"; shift ;;
    -*)           echo "Unknown option: $1"; exit 1 ;;
    *)            REMOTE="$1";   shift ;;
  esac
done

if [[ -z "$REMOTE" ]]; then
  echo "Usage: ./deploy-pi-web.sh <user@host> [options]"
  echo ""
  echo "Options:"
  echo "  --port <port>        App port (default 80)"
  echo "  --key <ssh-key>      SSH key file"
  echo "  --rebuild-db         Delete and recreate the database"
  echo ""
  echo "Examples:"
  echo "  ./deploy-pi-web.sh admin@192.168.1.60"
  echo "  ./deploy-pi-web.sh admin@192.168.1.60 --port 8080"
  exit 1
fi

# Build SSH options
SSH_OPTS="-o StrictHostKeyChecking=no"
SCP_OPTS="-o StrictHostKeyChecking=no"
if [[ -n "$SSH_KEY" ]]; then
  SSH_OPTS="$SSH_OPTS -i $SSH_KEY"
  SCP_OPTS="$SCP_OPTS -i $SSH_KEY"
fi

# Always publish a fresh build — a cached tarball silently deploys stale
# code. Pass --skip-build to reuse the existing tarball (e.g. fanning the
# same build out to several Pis).
if [[ "$SKIP_BUILD" == "1" && -f "$TARBALL" ]]; then
  echo "==> Reusing existing tarball (--skip-build)."
else
  echo "==> Publishing fresh build..."
  bash "$SCRIPT_DIR/publish-pi-web.sh"
fi

echo "==> Uploading to $REMOTE..."
scp $SCP_OPTS "$TARBALL" "$REMOTE:/tmp/foundation-pi-deploy.tar.gz"

# Build install command with parameters
INSTALL_CMD="cd /tmp"
INSTALL_CMD="$INSTALL_CMD && mkdir -p /tmp/foundation-pi-install"
INSTALL_CMD="$INSTALL_CMD && tar -xzf /tmp/foundation-pi-deploy.tar.gz -C /tmp/foundation-pi-install"
INSTALL_CMD="$INSTALL_CMD && cd /tmp/foundation-pi-install"
INSTALL_CMD="$INSTALL_CMD && sed -i 's/\r$//' install-pi-web.sh foundation-pi.service"
INSTALL_CMD="$INSTALL_CMD && sudo PORT='$APP_PORT' REBUILD_DB='$REBUILD_DB' bash install-pi-web.sh"
INSTALL_CMD="$INSTALL_CMD && rm -rf /tmp/foundation-pi-install /tmp/foundation-pi-deploy.tar.gz"

echo "==> Installing on Pi..."
ssh $SSH_OPTS "$REMOTE" "$INSTALL_CMD"

echo ""
echo "=========================================="
echo "  Pi deploy complete!"
echo "=========================================="
echo "  Pi: $REMOTE"
if [[ "$APP_PORT" == "80" ]]; then
echo "  URL:    http://${REMOTE#*@}"
else
echo "  URL:    http://${REMOTE#*@}:$APP_PORT"
fi
echo "  Check:  ssh $REMOTE 'systemctl status foundation'"
echo "=========================================="
