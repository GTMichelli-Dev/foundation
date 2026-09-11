#!/usr/bin/env bash
# Copy each card's description to the truck it names in Tables -> Trucks.
#
# For every enabled card that carries a carrier and a truck ID, the truck with
# that carrier and truck ID gets the card's description. The server does the
# work (POST /api/cardadmin/copy-descriptions-to-trucks) and reports every
# truck it changed and every card it passed over.
#
# Cards are never changed. A truck named by two cards with different
# descriptions is left alone and listed, so you can decide which is right.
#
# Needs curl; python3 (if present) prints the report readably.

set -euo pipefail

usage() {
    cat <<'EOF'
Usage: copy-card-descriptions-to-trucks.sh [--server URL] [--dry-run] [--user NAME]

  --server URL   The web app's address (default http://localhost:5110).
  --dry-run      Report what would change without changing anything.
  --user NAME    Sign in first - needed when Require Login is on. The user must
                 be a Manager or Admin. The password is asked for.

Examples:
  bash scripts/copy-card-descriptions-to-trucks.sh --dry-run
  bash scripts/copy-card-descriptions-to-trucks.sh --server https://scale.example.com --user admin
EOF
}

server="http://localhost:5110"
dry_run=false
username=""

while [ $# -gt 0 ]; do
    case "$1" in
        --server) server="${2:?--server needs a URL}"; shift 2 ;;
        --dry-run) dry_run=true; shift ;;
        --user|--username) username="${2:?--user needs a name}"; shift 2 ;;
        -h|--help) usage; exit 0 ;;
        *) echo "Unknown option: $1" >&2; usage >&2; exit 2 ;;
    esac
done

base="${server%/}"
jar="$(mktemp)"
trap 'rm -f "$jar"' EXIT

if [ -n "$username" ]; then
    read -r -s -p "Password for $username: " password
    echo

    # The sign-in form carries an anti-forgery token that has to go back with it.
    page="$(curl -fsSL -c "$jar" -b "$jar" "$base/Account/Login")"
    token="$(printf '%s' "$page" | sed -n 's/.*name="__RequestVerificationToken"[^>]*value="\([^"]*\)".*/\1/p' | head -n 1)"
    if [ -z "$token" ]; then
        echo "No sign-in form at $base/Account/Login - is Require Login on?" >&2
        exit 1
    fi

    after="$(curl -fsSL -c "$jar" -b "$jar" \
        --data-urlencode "username=$username" \
        --data-urlencode "password=$password" \
        --data-urlencode "__RequestVerificationToken=$token" \
        "$base/Account/Login")"
    if printf '%s' "$after" | grep -q 'name="password"'; then
        echo "Sign-in failed for $username." >&2
        exit 1
    fi
fi

url="$base/api/cardadmin/copy-descriptions-to-trucks"
if [ "$dry_run" = true ]; then url="$url?dryRun=true"; fi

# -d '' makes it a POST; a redirect to the sign-in page is followed as a GET.
response="$(curl -sS -L -c "$jar" -b "$jar" -d '' -w $'\n%{http_code} %{content_type}' "$url")"
body="${response%$'\n'*}"
meta="${response##*$'\n'}"
code="${meta%% *}"
ctype="${meta#* }"

case "$code" in
    2*) ;;
    403) echo "Access denied: sign in as a Manager or Admin with --user." >&2; exit 1 ;;
    *) echo "The server answered $code: $body" >&2; exit 1 ;;
esac

# With Require Login on and nobody signed in, the server answers with its
# sign-in page instead of the report.
case "$ctype" in
    *json*) ;;
    *) echo "The server answered with a page, not a report - Require Login is on. Run again with --user NAME." >&2; exit 1 ;;
esac

if ! command -v python3 >/dev/null 2>&1; then
    printf '%s\n' "$body"
    exit 0
fi

printf '%s' "$body" | python3 -c '
import json, sys

r = json.load(sys.stdin)
verb = "Would update" if r["dryRun"] else "Updated"
print("%s %d truck(s); %d already matched." % (verb, r["updated"], r["unchanged"]))
for c in r["changes"]:
    before = "\"%s\"" % c["before"] if c["before"] else "(blank)"
    print("  %s / %s: %s -> \"%s\"  (card %s)" % (c["carrier"], c["truckId"], before, c["after"], ", ".join(c["cards"])))

if r["conflicts"]:
    print()
    print("Left alone - the cards naming these trucks disagree:")
    for c in r["conflicts"]:
        cards = ", ".join("card %s \"%s\"" % (x["cardNumber"], x["description"]) for x in c["cards"])
        print("  %s / %s: %s" % (c["carrier"], c["truckId"], cards))

if r["skipped"]:
    print()
    print("Cards passed over:")
    for s in r["skipped"]:
        print("  card %s: %s" % (s["cardNumber"], s["reason"]))

if r["dryRun"]:
    print()
    print("Nothing was changed. Run again without --dry-run to apply.")
'
