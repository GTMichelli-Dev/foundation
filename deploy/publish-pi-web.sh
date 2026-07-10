#!/bin/bash
set -euo pipefail

# Publish Foundation web app for Raspberry Pi (linux-arm64)
# Output goes to deploy/out-pi/

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
OUT_DIR="$SCRIPT_DIR/out-pi"

echo "==> Cleaning previous publish..."
rm -rf "$OUT_DIR"
mkdir -p "$OUT_DIR/foundation"

echo "==> Publishing Foundation.Web (linux-arm64, self-contained)..."
dotnet publish "$ROOT_DIR/web/Foundation.Web/Foundation.Web.csproj" \
  -c Release \
  -r linux-arm64 \
  --self-contained true \
  -o "$OUT_DIR/foundation" \
  -p:PublishSingleFile=false

echo "==> Copying service files..."
cp "$SCRIPT_DIR/foundation-pi.service" "$OUT_DIR/"
cp "$SCRIPT_DIR/install-pi-web.sh" "$OUT_DIR/"

echo "==> Creating deploy tarball..."
cd "$OUT_DIR"
tar -czf "$SCRIPT_DIR/foundation-pi-deploy.tar.gz" .

echo ""
echo "=========================================="
echo "  Publish complete!"
echo "=========================================="
echo "  Tarball: deploy/foundation-pi-deploy.tar.gz"
echo "  Web App: deploy/out-pi/foundation/"
echo ""
echo "  Deploy with:"
echo "    bash deploy/deploy-pi-web.sh admin@<pi-ip>"
echo ""
echo "  To rebuild the database (WARNING: deletes all data):"
echo "    bash deploy/deploy-pi-web.sh admin@<pi-ip> --rebuild-db"
echo "=========================================="
