@echo off
REM ===========================================================================
REM  Foundation web app - Windows installer (wrapper)
REM
REM  Right-click -> "Run as administrator", or run from an admin prompt.
REM  Exists so the PowerShell script runs without changing the machine's
REM  execution policy.
REM
REM  Usage:
REM    INSTALL-WEB.bat
REM    INSTALL-WEB.bat -Port 80        (no port in the address; IIS must be off)
REM    INSTALL-WEB.bat -Port 8080
REM    INSTALL-WEB.bat -InstallDir D:\Foundation
REM
REM  Everything given here is passed through to install-web.ps1.
REM  Run  powershell -File install-web.ps1 -?  to see all options.
REM ===========================================================================

setlocal

REM Capture the script's own folder BEFORE any shift: `shift` moves %0 as well,
REM so %~dp0 read after shifting resolves to the caller's current directory
REM instead of this file's location.
set SCRIPTDIR=%~dp0

REM Collect any switches (-Port n, -InstallDir "x", -ResetDb, ...). Unlike the
REM print service there is no required argument: the defaults install a working
REM site, which is what most scale houses want.
set EXTRA=
:collect
if "%~1"=="" goto run
set EXTRA=%EXTRA% %1
shift
goto collect

:run
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPTDIR%install-web.ps1"%EXTRA%
set RC=%ERRORLEVEL%

echo.
if not "%RC%"=="0" (
    echo  Install FAILED with code %RC%.
) else (
    echo  Press any key to close.
)
pause >nul
exit /b %RC%

endlocal
