@echo off
setlocal enabledelayedexpansion

REM Publish Basic Weigh for Debian server (linux-x64)
REM Output goes to deploy\out\

set "SCRIPT_DIR=%~dp0"
set "ROOT_DIR=%SCRIPT_DIR%.."
set "OUT_DIR=%SCRIPT_DIR%out"

echo ==^> Cleaning previous publish...
if exist "%OUT_DIR%" rd /s /q "%OUT_DIR%"
mkdir "%OUT_DIR%\basicweigh"

echo ==^> Publishing BasicWeigh.Web (linux-x64, self-contained)...
dotnet publish "%ROOT_DIR%\web\BasicWeigh.Web\BasicWeigh.Web.csproj" -c Release -r linux-x64 --self-contained true -o "%OUT_DIR%\basicweigh" /p:PublishSingleFile=false
if errorlevel 1 (
    echo ERROR: dotnet publish failed
    exit /b 1
)

echo ==^> Copying service files...
copy "%SCRIPT_DIR%basicweigh.service" "%OUT_DIR%\" >nul
copy "%SCRIPT_DIR%install.sh" "%OUT_DIR%\" >nul

echo ==^> Creating deploy tarball...
where tar >nul 2>&1
if errorlevel 1 (
    echo ERROR: tar not found. Windows 10+ includes tar, or install Git for Windows.
    exit /b 1
)
pushd "%OUT_DIR%"
tar -czf "%SCRIPT_DIR%basicweigh-deploy.tar.gz" .
popd

echo.
echo ==========================================
echo   Publish complete!
echo ==========================================
echo   Tarball: deploy\basicweigh-deploy.tar.gz
echo   Web App: deploy\out\basicweigh\
echo.
echo   Deploy with:
echo     deploy\deploy.bat admin@^<server^> --domain your.domain.com --email you@email.com
echo ==========================================
