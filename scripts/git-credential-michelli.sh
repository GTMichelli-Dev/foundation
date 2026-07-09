#!/usr/bin/env bash
# git-credential-michelli.sh — git credential helper that returns a
# freshly-minted GitHub App installation token. Registered via:
#
#   git config --system credential.https://github.com/GTMichelli-Dev.helper \
#       /usr/local/bin/git-credential-michelli
#
# Git invokes credential helpers with one of: get / store / erase. Only "get"
# is meaningful here — store/erase are no-ops because the token isn't
# persisted in any git-known location (it comes from the App on every refresh).

set -euo pipefail

case "${1:-}" in
  get) ;;
  *)   exit 0 ;;
esac

# Drain stdin (git pipes the URL/host description in; we don't need it because
# this helper is already scoped to github.com/GTMichelli-Dev via gitconfig).
cat >/dev/null

TOKEN=$(michelli-github-app-token)
printf 'username=x-access-token\npassword=%s\n' "$TOKEN"
