#!/bin/bash
set -euo pipefail

# Publish Basic Weigh for Debian server (linux-x64)
# Output goes to deploy/out/

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
OUT_DIR="$SCRIPT_DIR/out"

echo "==> Cleaning previous publish..."
rm -rf "$OUT_DIR"
mkdir -p "$OUT_DIR/basicweigh"

echo "==> Publishing BasicWeigh.Web (linux-x64, self-contained)..."
dotnet publish "$ROOT_DIR/web/BasicWeigh.Web/BasicWeigh.Web.csproj" \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -o "$OUT_DIR/basicweigh" \
  /p:PublishSingleFile=false

echo "==> Copying service files..."
cp "$SCRIPT_DIR/basicweigh.service" "$OUT_DIR/"
cp "$SCRIPT_DIR/install.sh" "$OUT_DIR/"

echo "==> Creating deploy tarball..."
cd "$OUT_DIR"
tar -czf "$SCRIPT_DIR/basicweigh-deploy.tar.gz" .

echo ""
echo "=========================================="
echo "  Publish complete!"
echo "=========================================="
echo "  Tarball: deploy/basicweigh-deploy.tar.gz"
echo "  Web App: deploy/out/basicweigh/"
echo ""
echo "  Deploy with:"
echo "    bash deploy/deploy.sh admin@<server> --domain your.domain.com --email you@email.com"
echo ""
echo "  To rebuild the database (WARNING: deletes all data):"
echo "    bash deploy/deploy.sh admin@<server> --domain your.domain.com --email you@email.com --rebuild-db"
echo "=========================================="
