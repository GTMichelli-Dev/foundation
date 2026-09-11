Foundation web app - Windows install package
============================================

Prebuilt and SELF-CONTAINED. The .NET runtime is bundled, so this PC needs no
.NET install, no SDK, no git and no DevExpress licence.

This is the Foundation server: the site operators use, and the thing kiosk
displays, the scale reader, the print service and drivers' phones all connect
to. Install it on ONE machine per site.


INSTALL OR UPDATE
-----------------
Right-click INSTALL-WEB.bat and choose "Run as administrator".

Or, from an ADMIN command prompt in this folder:

    INSTALL-WEB.bat

That is the whole install. It asks which port to use first: press Enter for
5110 (or, on an update, the port the site is already on), or type 80 to serve
the site with no port in the address. The same command handles a fresh
install and an update, and is safe to re-run - an update keeps the database,
the login keys and any ticket layouts edited in the Report Designer.

    INSTALL-WEB.bat -Port 80
        Serve the site with no port in the address: http://scale.local/
        instead of http://scale.local:5110/. IIS must be off - the installer
        stops and says so if it is in the way. Moving an existing site means
        re-pointing kiosks, the scale reader and the print service at the new
        address.

    INSTALL-WEB.bat -Port 8080
        Set the port without being asked - for a scripted install, or to skip
        the question.

    INSTALL-WEB.bat -InstallDir D:\Foundation
        Install somewhere other than C:\Foundation.

    INSTALL-WEB.bat -ResetDb
        Start from an empty database. DESTROYS every ticket and setting. A
        timestamped backup is taken first regardless.

    INSTALL-WEB.bat -SkipFirewall
        Do not add the inbound firewall rule. Only do this if firewall rules
        are managed centrally - without one, nothing else on the network can
        reach the site.

    powershell -ExecutionPolicy Bypass -File install-web.ps1 -?
        All options.


WHAT IT DOES
------------
  - Copies the app to C:\Foundation (or -InstallDir).
  - Registers a Windows service named "Foundation", set to start at boot and
    to restart itself if it crashes.
  - Opens inbound TCP 5110 (or -Port) in Windows Firewall.
  - Starts the service and waits until the site actually answers before
    reporting success. The first start also creates the database and applies
    every migration, so it takes longer than later ones.

It prints the address to use at the end - both the local one and the network
one to point kiosks and services at.


AFTER INSTALLING
----------------
Open the site and work through Setup: company details, scales, printers and
users. Then point the other pieces at the network address the installer
printed.

Nothing else on the network can reach the site until the firewall rule exists,
which the installer adds unless told not to.


HTTPS
-----
Not required on a LAN, and not configured here. A scale house on its own
network runs fine on plain HTTP - that is a supported setup, not a compromise.

Add TLS when the site is reachable from outside the LAN, or when policy asks
for it: passwords and session cookies otherwise cross the network in the clear.
Put IIS or another reverse proxy in front and terminate there. Nothing in the
app changes.


BACKUPS
-------
    C:\Foundation\Foundation.db

That single file is the site's data - every ticket, customer and setting. Back
it up. The installer copies it aside before each update, but those backups sit
on the same machine and the same disk.

If cameras are enabled, the ticket photos under wwwroot\images\tickets are the
only thing that grows appreciably: roughly two images per ticket at ~127 KB.


MANAGING THE SERVICE
--------------------
    sc query Foundation           Is it running?
    sc stop Foundation            Stop it.
    sc start Foundation           Start it.

Startup errors land in Event Viewer > Windows Logs > Application.


REQUIREMENTS
------------
  Windows 10 (1607+), Windows 11, or Windows Server 2016+, x64
  2 GB free RAM (the app idles around 130 MB, peaks near 220 MB printing)
  1 GB disk, plus room for ticket photos if cameras are on

If the service fails to start with a native-DLL error, install the Microsoft
Visual C++ Redistributable (x64):

    https://aka.ms/vs/17/release/vc_redist.x64.exe

A bare Windows Server install can lack it; a normal desktop almost never does.


Full documentation:
    https://github.com/GTMichelli-Dev/foundation/blob/main/docs/deploy-windows.md
