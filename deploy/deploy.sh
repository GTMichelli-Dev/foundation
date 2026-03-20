#!/bin/bash
set -euo pipefail

# Deploy Basic Weigh to a remote Debian server
# Usage: ./deploy.sh <user@host> [ssh-key]
# Example: ./deploy.sh admin@192.168.1.100
# Example: ./deploy.sh admin@192.168.1.100 ~/.ssh/id_rsa

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARBALL="$SCRIPT_DIR/basicweigh-deploy.tar.gz"
REMOTE="${1:-}"
SSH_KEY="${2:-}"

if [[ -z "$REMOTE" ]]; then
  echo "Usage: ./deploy.sh <user@host> [ssh-key]"
  echo "Example: ./deploy.sh admin@192.168.1.100"
  exit 1
fi

# Build SSH options
SSH_OPTS="-o StrictHostKeyChecking=no"
SCP_OPTS="-o StrictHostKeyChecking=no"
if [[ -n "$SSH_KEY" ]]; then
  SSH_OPTS="$SSH_OPTS -i $SSH_KEY"
  SCP_OPTS="$SCP_OPTS -i $SSH_KEY"
fi

# Check if tarball exists, if not run publish first
if [[ ! -f "$TARBALL" ]]; then
  echo "==> Tarball not found. Running publish first..."
  bash "$SCRIPT_DIR/publish.sh"
fi

echo "==> Uploading to $REMOTE..."
scp $SCP_OPTS "$TARBALL" "$REMOTE:/tmp/basicweigh-deploy.tar.gz"

echo "==> Installing on remote server..."
ssh $SSH_OPTS "$REMOTE" "
  cd /tmp && \
  mkdir -p /tmp/basicweigh-install && \
  tar -xzf /tmp/basicweigh-deploy.tar.gz -C /tmp/basicweigh-install && \
  cd /tmp/basicweigh-install && \
  sudo bash install.sh && \
  rm -rf /tmp/basicweigh-install /tmp/basicweigh-deploy.tar.gz
"

echo ""
echo "=========================================="
echo "  Deploy complete!"
echo "=========================================="
echo "  Server: $REMOTE"
echo "  Check:  ssh $REMOTE 'systemctl status basicweigh'"
echo "=========================================="
