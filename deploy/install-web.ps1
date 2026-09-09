<#
.SYNOPSIS
    Install or update the Foundation web app on Windows.

.DESCRIPTION
    The Windows counterpart to deploy/install.sh. Works from a prebuilt,
    self-contained publish folder, so the target PC needs no .NET, no SDK, no
    git and no DevExpress licence - the normal state of a customer machine.

    Installs Foundation as a Windows service with automatic startup, opens the
    firewall port, and verifies the site actually answers before reporting
    success.

    Safe to re-run: on an existing install it stops the service, PRESERVES the
    database, the data-protection keys and any saved ticket templates, copies
    the new binaries, and starts it again. That is the supported update path.

.PARAMETER Port
    Port the site listens on. Default 5110. The kiosk displays, scale reader,
    print service and drivers' phones all connect to this.

.PARAMETER InstallDir
    Where the app is installed. Default C:\Foundation

.PARAMETER ServiceName
    Windows service name. Default Foundation.

.PARAMETER ResetDb
    Delete the existing database and start clean. This DESTROYS all tickets and
    settings. A timestamped backup is taken first regardless.

.PARAMETER SkipFirewall
    Do not add the inbound firewall rule. The site will then only answer on the
    machine itself - useful only if you manage firewall rules centrally.

.EXAMPLE
    .\install-web.ps1

.EXAMPLE
    .\install-web.ps1 -Port 8080 -InstallDir D:\Foundation
#>
[CmdletBinding()]
param(
    [int]$Port = 5110,
    [string]$InstallDir = "C:\Foundation",
    [string]$ServiceName = "Foundation",
    [switch]$ResetDb,
    [switch]$SkipFirewall
)

$ErrorActionPreference = "Stop"

function Step($n, $msg) { Write-Host "[$n/7] $msg" -ForegroundColor Cyan }
function Ok($msg)       { Write-Host "      $msg" -ForegroundColor Green }
function Note($msg)     { Write-Host "      $msg" -ForegroundColor Gray }
function Warn($msg)     { Write-Host "      $msg" -ForegroundColor Yellow }
function Die($msg)      { Write-Host ""; Write-Host "ERROR: $msg" -ForegroundColor Red; exit 1 }

Write-Host ""
Write-Host "=========================================" -ForegroundColor White
Write-Host "  Foundation - Windows installer" -ForegroundColor White
Write-Host "=========================================" -ForegroundColor White
Write-Host ""

# ---------------------------------------------------------------- preflight --
# Arguments and files first, elevation last: a typo should fail immediately and
# in any prompt, rather than only after the operator re-opens one as admin.

if ($Port -lt 1 -or $Port -gt 65535) { Die "Port must be 1-65535 - got $Port" }

$appSource = Join-Path $PSScriptRoot "app"
if (-not (Test-Path $appSource)) { $appSource = $PSScriptRoot }
$exeSource = Join-Path $appSource "Foundation.Web.exe"
if (-not (Test-Path $exeSource)) {
    Die "Foundation.Web.exe not found. Expected in '$appSource'. Run this from the unzipped package folder."
}

$identity  = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Die "This must run from an ADMIN PowerShell. Creating a Windows service needs it."
}

# A port already taken by something else is the one failure that looks like a
# broken install but is not, so say so plainly before touching anything.
$inUse = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue
if ($inUse) {
    $owner = (Get-Process -Id $inUse[0].OwningProcess -ErrorAction SilentlyContinue).ProcessName
    if ($owner -notmatch "Foundation") {
        Die "Port $Port is already in use by '$owner'. Pick another with -Port, or stop that process."
    }
}

# ----------------------------------------------------------------- 1. stop ---
Step 1 "Checking for an existing install..."
$existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
$isUpdate = $false
if ($existing) {
    $isUpdate = $true
    Note "Found service '$ServiceName' ($($existing.Status)). This is an update."
    if ($existing.Status -ne "Stopped") {
        Stop-Service -Name $ServiceName -Force
        # The SCM reports Stopped before the process has released its file
        # handles, and copying over a still-locked .exe fails halfway, leaving
        # a half-updated folder.
        $existing.WaitForStatus("Stopped", "00:00:30")
        Start-Sleep -Seconds 2
        Ok "Service stopped."
    }
} else {
    Note "No existing service - fresh install."
}

# --------------------------------------------------------------- 2. backup ---
Step 2 "Protecting existing data..."
$dbPath = Join-Path $InstallDir "Foundation.db"
if (Test-Path $dbPath) {
    $stamp  = Get-Date -Format "yyyyMMdd-HHmmss"
    $backup = Join-Path $InstallDir "Foundation.db.backup-$stamp"
    Copy-Item $dbPath $backup -Force
    Ok "Database backed up to $(Split-Path $backup -Leaf)"

    if ($ResetDb) {
        Remove-Item $dbPath -Force
        Remove-Item "$dbPath-wal", "$dbPath-shm" -Force -ErrorAction SilentlyContinue
        Warn "-ResetDb: database deleted. The site will start empty."
    }
} else {
    Note "No existing database - one will be created on first start."
}

# ----------------------------------------------------------------- 3. copy ---
Step 3 "Copying application files..."
New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null

# Everything the SITE owns rather than the build: its data, its keys, and any
# ticket templates edited in the Report Designer. Copying over these is what
# would turn an update into data loss. The publish output does not contain
# them, but an operator who once copied a folder by hand can leave stale ones
# lying around, and this is the update path.
$preserve = @("Foundation.db", "Foundation.db-wal", "Foundation.db-shm", "App_Data", "Reports")
Get-ChildItem $appSource -Force | Where-Object { $preserve -notcontains $_.Name } | ForEach-Object {
    Copy-Item $_.FullName -Destination $InstallDir -Recurse -Force
}
$exePath = Join-Path $InstallDir "Foundation.Web.exe"
if (-not (Test-Path $exePath)) { Die "Copy failed - $exePath is missing." }
Ok "Installed to $InstallDir"

# -------------------------------------------------------------- 4. service ---
Step 4 "Registering the Windows service..."
# Bind 0.0.0.0, not localhost: kiosk displays, the scale reader, the print
# service and drivers' phones all connect to this machine from elsewhere.
$binPath = "`"$exePath`" --urls http://0.0.0.0:$Port"

if ($existing) {
    & sc.exe config $ServiceName binPath= $binPath start= auto | Out-Null
    if ($LASTEXITCODE -ne 0) { Die "sc config failed with code $LASTEXITCODE" }
    Ok "Service updated."
} else {
    & sc.exe create $ServiceName binPath= $binPath start= auto DisplayName= "Foundation Truck Scale" | Out-Null
    if ($LASTEXITCODE -ne 0) { Die "sc create failed with code $LASTEXITCODE" }
    Ok "Service created."
}
& sc.exe description $ServiceName "Foundation truck scale management web app." | Out-Null
# Restart on crash rather than leaving a scale house with a dead site: 5s, 15s,
# then every minute, with the count resetting daily.
& sc.exe failure $ServiceName reset= 86400 actions= restart/5000/restart/15000/restart/60000 | Out-Null

# ------------------------------------------------------------- 5. firewall ---
Step 5 "Opening the firewall..."
if ($SkipFirewall) {
    Warn "-SkipFirewall: no rule added. Only this machine will reach the site."
} else {
    $ruleName = "Foundation $Port"
    Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue |
        Remove-NetFirewallRule -ErrorAction SilentlyContinue
    New-NetFirewallRule -DisplayName $ruleName -Direction Inbound -Action Allow `
        -Protocol TCP -LocalPort $Port -Profile Any | Out-Null
    Ok "Inbound TCP $Port allowed."
}

# ---------------------------------------------------------------- 6. start ---
Step 6 "Starting the service..."
Start-Service -Name $ServiceName
$svc = Get-Service -Name $ServiceName
$svc.WaitForStatus("Running", "00:00:30")
Ok "Service running."

# --------------------------------------------------------------- 7. verify ---
# An install that reports success without the site answering is worth very
# little: the first start also applies database migrations, which is exactly
# when a bad install shows up.
Step 7 "Verifying the site answers..."
$url = "http://localhost:$Port/"
$up  = $false
foreach ($attempt in 1..30) {
    Start-Sleep -Seconds 2
    try {
        $r = Invoke-WebRequest $url -UseBasicParsing -TimeoutSec 10
        if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 400) { $up = $true; break }
    } catch {
        $svc.Refresh()
        if ($svc.Status -ne "Running") {
            Die "The service stopped while starting. Check Event Viewer > Windows Logs > Application."
        }
    }
}
if (-not $up) {
    Die "The service is running but $url did not answer in 60s. The first start applies database migrations and can be slow on modest hardware - try again, then check Event Viewer."
}
Ok "Site is up."

$ip = (Get-NetIPAddress -AddressFamily IPv4 -ErrorAction SilentlyContinue |
       Where-Object { $_.IPAddress -notmatch '^(127\.|169\.254\.)' } |
       Select-Object -First 1).IPAddress

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  Install complete" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "  On this PC:        $url"
if ($ip) { Write-Host "  On the network:    http://${ip}:$Port/" }
Write-Host "  Installed in:      $InstallDir"
Write-Host "  Service:           $ServiceName (starts automatically at boot)"
Write-Host ""
if (-not $isUpdate) {
    Write-Host "  Next: open the site and work through Setup." -ForegroundColor Yellow
    Write-Host "  Point kiosks, the scale reader and the print service at the network URL above."
    Write-Host ""
}
Write-Host "  Back up $InstallDir\Foundation.db - that file is the site's data." -ForegroundColor Gray
Write-Host ""
