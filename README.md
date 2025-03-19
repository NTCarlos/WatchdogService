# WatchdogService
A lightweight Windows Service that monitors and restarts a specified application if it crashes or becomes unresponsive. It runs with admin rights, reads configurations from appsettings.json, and ensures uninterrupted application uptime.

# Build & Install as a Windows Service
## Publish the app (Run in Command Prompt or PowerShell)
dotnet publish -c Release -r win-x64 --self-contained false

## Install the service (Run as Admin)
sc create WatchdogService binPath="C:\Path\To\PublishedApp\WatchdogService.exe" start=auto
sc start WatchdogService

## Verify it's running
sc query WatchdogService
