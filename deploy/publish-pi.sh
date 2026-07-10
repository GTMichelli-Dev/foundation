#!/bin/bash
set -euo pipefail

# Publish Kiosk Print Agent for Raspberry Pi (linux-arm64)
# Output goes to deploy/out-pi/

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
OUT_DIR="$SCRIPT_DIR/out-pi"

echo "==> Cleaning previous publish..."
rm -rf "$OUT_DIR"
mkdir -p "$OUT_DIR/kioskprint"

echo "==> Publishing KioskPrintAgent (linux-arm64, self-contained)..."
dotnet publish "$ROOT_DIR/KioskPrintAgent/KioskPrintAgent.csproj" \
  -c Release \
  -r linux-arm64 \
  --self-contained true \
  -o "$OUT_DIR/kioskprint" \
  -p:PublishSingleFile=false

echo "==> Copying service and install files..."
cp "$SCRIPT_DIR/kioskprint.service" "$OUT_DIR/"
cp "$SCRIPT_DIR/install-pi.sh" "$OUT_DIR/"

echo "==> Creating deploy tarball..."
cd "$OUT_DIR"
tar -czf "$SCRIPT_DIR/kioskprint-deploy.tar.gz" .

echo ""
echo "=========================================="
echo "  Pi Publish complete!"
echo "=========================================="
echo "  Tarball: deploy/kioskprint-deploy.tar.gz"
echo "  Print Agent: deploy/out-pi/kioskprint/"
echo ""
echo "  Deploy with:"
echo "    bash deploy/deploy-pi.sh pi@<pi-address>"
echo "=========================================="
