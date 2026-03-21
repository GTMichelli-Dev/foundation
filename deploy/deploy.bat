@echo off
setlocal enabledelayedexpansion

REM Deploy Basic Weigh to a remote Debian server
REM Usage: deploy.bat <user@host> [options]
REM
REM Options:
REM   --domain <domain>    Domain name for Let's Encrypt SSL
REM   --email <email>      Email for Let's Encrypt notifications
REM   --port <port>        App listen port (default 5110)
REM   --key <ssh-key>      SSH key file
REM   --rebuild-db         Delete and recreate the database (CAUTION: destroys data!)

set "SCRIPT_DIR=%~dp0"
set "TARBALL=%SCRIPT_DIR%basicweigh-deploy.tar.gz"

set "REMOTE="
set "DOMAIN="
set "EMAIL="
set "APP_PORT=5110"
set "SSH_KEY="
set "REBUILD_DB=0"

REM Parse arguments
:parse_args
if "%~1"=="" goto :done_args
if "%~1"=="--domain"  ( set "DOMAIN=%~2" & shift & shift & goto :parse_args )
if "%~1"=="--email"   ( set "EMAIL=%~2" & shift & shift & goto :parse_args )
if "%~1"=="--port"    ( set "APP_PORT=%~2" & shift & shift & goto :parse_args )
if "%~1"=="--key"     ( set "SSH_KEY=%~2" & shift & shift & goto :parse_args )
if "%~1"=="--rebuild-db" ( set "REBUILD_DB=1" & shift & goto :parse_args )
set "REMOTE=%~1"
shift
goto :parse_args
:done_args

if "%REMOTE%"=="" (
    echo Usage: deploy.bat ^<user@host^> [options]
    echo.
    echo Options:
    echo   --domain ^<domain^>    Domain for Let's Encrypt SSL
    echo   --email ^<email^>      Email for Let's Encrypt
    echo   --port ^<port^>        App port ^(default 5110^)
    echo   --key ^<ssh-key^>      SSH key file
    echo   --rebuild-db         Delete and recreate the database
    echo.
    echo Examples:
    echo   deploy.bat admin@192.168.1.100
    echo   deploy.bat admin@149.28.xxx.xxx --domain scale.example.com --email admin@example.com
    exit /b 1
)

REM Build SSH options
set "SSH_OPTS=-o StrictHostKeyChecking=no"
set "SCP_OPTS=-o StrictHostKeyChecking=no"
if not "%SSH_KEY%"=="" (
    set "SSH_OPTS=!SSH_OPTS! -i %SSH_KEY%"
    set "SCP_OPTS=!SCP_OPTS! -i %SSH_KEY%"
)

REM Verify DNS resolves before deploying (if domain specified)
if not "%DOMAIN%"=="" (
    echo ==^> Verifying DNS for %DOMAIN%...
    nslookup %DOMAIN% >nul 2>&1
    if errorlevel 1 (
        echo ERROR: DNS lookup failed for %DOMAIN%
        echo        Create an A record pointing %DOMAIN% to your server IP.
        echo        Then wait for propagation and try again.
        exit /b 1
    )
    echo   DNS OK: %DOMAIN% resolves.
)

REM Check if tarball exists, if not run publish first
if not exist "%TARBALL%" (
    echo ==^> Tarball not found. Running publish first...
    call "%SCRIPT_DIR%publish.bat"
    if errorlevel 1 exit /b 1
)

echo ==^> Uploading to %REMOTE%...
scp %SCP_OPTS% "%TARBALL%" "%REMOTE%:/tmp/basicweigh-deploy.tar.gz"
if errorlevel 1 (
    echo ERROR: Upload failed
    exit /b 1
)

REM Build install command - sed fixes Windows CRLF line endings before running
set "INSTALL_CMD=cd /tmp && mkdir -p /tmp/basicweigh-install && tar -xzf /tmp/basicweigh-deploy.tar.gz -C /tmp/basicweigh-install && cd /tmp/basicweigh-install && sed -i 's/\r$//' install.sh && sudo DOMAIN='%DOMAIN%' EMAIL='%EMAIL%' PORT='%APP_PORT%' REBUILD_DB='%REBUILD_DB%' bash install.sh && rm -rf /tmp/basicweigh-install /tmp/basicweigh-deploy.tar.gz"

echo ==^> Installing on remote server...
ssh %SSH_OPTS% "%REMOTE%" "%INSTALL_CMD%"
if errorlevel 1 (
    echo ERROR: Remote install failed
    exit /b 1
)

echo.
echo ==========================================
echo   Deploy complete!
echo ==========================================
echo   Server: %REMOTE%
if not "%DOMAIN%"=="" (
    echo   URL:    https://%DOMAIN%
) else (
    echo   URL:    https://%REMOTE%
)
echo   Check:  ssh %REMOTE% "systemctl status basicweigh"
echo ==========================================
