<#
.SYNOPSIS
    Copy each card's description to the truck it names in Tables -> Trucks.

.DESCRIPTION
    For every enabled card that carries a carrier and a truck ID, the truck
    with that carrier and truck ID gets the card's description. The server
    does the work (POST /api/cardadmin/copy-descriptions-to-trucks) and
    reports every truck it changed and every card it passed over.

    Cards are never changed. A truck named by two cards with different
    descriptions is left alone and listed, so you can decide which is right.

    Works in Windows PowerShell 5.1 and PowerShell 7.

.PARAMETER Server
    The web app's address. Default http://localhost:5110.

.PARAMETER DryRun
    Report what would change without changing anything.

.PARAMETER Username
    Sign in as this user first - needed when Require Login is on. The user
    must be a Manager or Admin. The password is asked for, never taken on
    the command line.

.EXAMPLE
    .\copy-card-descriptions-to-trucks.ps1 -DryRun

.EXAMPLE
    .\copy-card-descriptions-to-trucks.ps1 -Server https://scale.example.com -Username admin
#>
[CmdletBinding()]
param(
    [string]$Server = 'http://localhost:5110',
    [switch]$DryRun,
    [string]$Username
)

$ErrorActionPreference = 'Stop'
$base = $Server.TrimEnd('/')
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

if ($Username) {
    $secure = Read-Host "Password for $Username" -AsSecureString
    $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
    try { $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) }

    # The sign-in form carries an anti-forgery token that has to go back with it.
    $page = Invoke-WebRequest "$base/Account/Login" -WebSession $session -UseBasicParsing
    $token = [regex]::Match($page.Content, 'name="__RequestVerificationToken"[^>]*?value="([^"]+)"').Groups[1].Value
    if (-not $token) { throw "No sign-in form at $base/Account/Login - is Require Login on?" }

    $signin = Invoke-WebRequest "$base/Account/Login" -Method Post -WebSession $session -UseBasicParsing -Body @{
        username                   = $Username
        password                   = $password
        __RequestVerificationToken = $token
    }
    if ($signin.Content -match 'name="password"') { throw "Sign-in failed for $Username." }
}

$uri = "$base/api/cardadmin/copy-descriptions-to-trucks"
if ($DryRun) { $uri += '?dryRun=true' }

try {
    $response = Invoke-WebRequest $uri -Method Post -WebSession $session -UseBasicParsing
}
catch {
    if ($_.Exception.Response -and [int]$_.Exception.Response.StatusCode -eq 403) {
        throw 'Access denied: sign in as a Manager or Admin with -Username.'
    }
    throw
}

# With Require Login on and nobody signed in, the server answers with its
# sign-in page instead of the report.
if (-not (@($response.Headers['Content-Type']) -match 'json')) {
    throw 'The server answered with a page, not a report - Require Login is on. Run again with -Username.'
}
$r = $response.Content | ConvertFrom-Json

$verb = if ($r.dryRun) { 'Would update' } else { 'Updated' }
Write-Host ("{0} {1} truck(s); {2} already matched." -f $verb, $r.updated, $r.unchanged)
foreach ($c in @($r.changes)) {
    $before = if ($c.before) { '"' + $c.before + '"' } else { '(blank)' }
    Write-Host ('  {0} / {1}: {2} -> "{3}"  (card {4})' -f $c.carrier, $c.truckId, $before, $c.after, (@($c.cards) -join ', '))
}

if (@($r.conflicts).Count -gt 0) {
    Write-Host ''
    Write-Host 'Left alone - the cards naming these trucks disagree:' -ForegroundColor Yellow
    foreach ($c in @($r.conflicts)) {
        $list = (@($c.cards) | ForEach-Object { 'card {0} "{1}"' -f $_.cardNumber, $_.description }) -join ', '
        Write-Host ('  {0} / {1}: {2}' -f $c.carrier, $c.truckId, $list)
    }
}

if (@($r.skipped).Count -gt 0) {
    Write-Host ''
    Write-Host 'Cards passed over:'
    foreach ($s in @($r.skipped)) { Write-Host ('  card {0}: {1}' -f $s.cardNumber, $s.reason) }
}

if ($r.dryRun) {
    Write-Host ''
    Write-Host 'Nothing was changed. Run again without -DryRun to apply.'
}
