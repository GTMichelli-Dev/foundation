# Deploying to a Windows Server (LAN or HTTPS)

[← Back to README](../README.md)

The scripted deploys target Debian. Windows runs the same app the same way —
Kestrel listening on a port — but there is no install script, so the steps here
are manual. Nothing about the app differs; only how you get it onto the machine
and keep it running.

> **HTTP is fine on a LAN.** A scale house on its own network needs no
> certificate and no reverse proxy. Add TLS only when the site is reachable
> from outside the LAN — see [HTTPS](#https-optional).

## Contents

- [What You Need](#what-you-need)
- [Step 1: Publish](#step-1-publish)
- [Step 2: Run It](#step-2-run-it)
- [Step 3: Open the Firewall Port](#step-3-open-the-firewall-port)
- [Step 4: Keep It Running](#step-4-keep-it-running)
- [HTTPS (optional)](#https-optional)
- [What Lives in the Publish Folder](#what-lives-in-the-publish-folder)
- [Updating](#updating)

## What You Need

Split the work across two machines where you can: **build** on a developer PC,
**run** on the server. The reporting packages are licensed DevExpress builds
that are not on nuget.org, so a machine that has never built this app cannot
restore it without your DevExpress feed configured. A published folder carries
those DLLs with it, so the server needs neither the feed nor the SDK.

| | Needs |
|---|---|
| Machine that **builds** | [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) + access to your DevExpress NuGet feed |
| Machine that **runs** | [ASP.NET Core 10 Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) only |

## Step 1: Publish

```
dotnet publish web\Foundation.Web -c Release -r win-x64 --self-contained false -o C:\Foundation
```

Copy `C:\Foundation` to the server if you built elsewhere. Use
`--self-contained true` instead when the server has no .NET at all — the output
is much larger but needs no runtime installed.

## Step 2: Run It

```
cd C:\Foundation
Foundation.Web.exe --urls http://0.0.0.0:5110
```

Bind `0.0.0.0`, not `localhost`. Kiosk displays, the scale reader, the print
service and drivers' phones all connect *to* this machine, and a localhost bind
accepts none of them.

The first start creates the database and applies every migration — the console
shows each one. Browse to `http://<server>:5110` and work through Setup.

## Step 3: Open the Firewall Port

Nothing reaches the app until Windows Firewall allows it. From an **admin**
prompt:

```
netsh advfirewall firewall add rule name="Foundation 5110" dir=in action=allow protocol=TCP localport=5110
```

## Step 4: Keep It Running

The app has no Windows service host compiled in, so `sc.exe` cannot run it
directly. Either register a Task Scheduler task (trigger *At startup*, "Run
whether user is logged on or not") or wrap it with a supervisor such as NSSM.
Without one of those it stops when the console window closes.

## HTTPS (optional)

A scale house on its own LAN can stay on plain HTTP — that is a supported mode,
not a compromise, and it is exactly what the [Raspberry Pi LAN
deploy](deploy-pi.md) does. Kestrel serves HTTP and nothing in the app needs a
secure browser context.

The one artifact is a startup warning:

```
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.
```

That is the redirect middleware finding no HTTPS port to send anyone to, so it
stops redirecting and serves the request. Harmless, and the reason HTTP works
at all outside Development.

Add TLS when the site is reachable from outside the LAN, or when policy asks
for it — passwords and session cookies otherwise cross the network in the
clear. On Debian that is Nginx with a Let's Encrypt (or self-signed)
certificate; on Windows put IIS or another reverse proxy in front and terminate
there. Nothing in the app changes either way.

## What Lives in the Publish Folder

| Path | |
|---|---|
| `Foundation.db` (+ `-wal`, `-shm`) | The database. **This is the backup target.** |
| `App_Data\keys` | Data-protection keys. Deleting them signs every user out. |
| `appsettings.json` | `Display:TimeZone` and the database provider. |
| `Reports\*.repx` | Only present once the site customizes a ticket in the Report Designer. Absent, the app uses its built-in layout — publishing deliberately does not overwrite a site's saved templates. |

## Updating

Publish over the top of the same folder. `Foundation.db`, `App_Data` and any
saved `.repx` are not part of the publish output, so they survive — but stop the
app first, or the running `.exe` is locked and the copy fails partway.
