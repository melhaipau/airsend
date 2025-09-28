
# AirSend (WPF) — Codespaces Template

This repo lets you **edit and build** a Windows WPF AirPlay sender scaffold **entirely in the browser** using **GitHub Codespaces**—no Visual Studio installation needed locally. You can build the `.exe` here and then **download** it to your PC to run.

> Note: You can't run a WPF GUI inside Codespaces. Use Codespaces to **build**, then download the build output and run it on your Windows PC.

## Quick Start (Codespaces)
1. Click **Use this template → Create a new repo**, then **Code → Create codespace**.
2. Once the dev container starts, run:
   ```bash
   dotnet restore AirSend/AirSend.csproj
   dotnet build AirSend/AirSend.csproj -c Release
   ```
   The build output will be in `AirSend/bin/Release/net8.0-windows/`.

3. To publish a portable build (recommended for download):
   ```bash
   dotnet publish AirSend/AirSend.csproj -c Release -r win-x64 --self-contained false
   ```
   Output: `AirSend/bin/Release/net8.0-windows/win-x64/publish/` (contains `AirSend.exe`).

4. Download the published files via the Codespaces file explorer (right-click → Download) or using the GitHub web UI once you push the commit.

## What’s included
- WPF app scaffolding (.NET 8, UseWPF=true)
- Zeroconf discovery for `_airplay._tcp` / `_raop._tcp`
- WASAPI loopback capture via NAudio (Windows-only; compiles fine in Codespaces)
- Simple MVVM (no external MVVM libs)
- Placeholder RAOP sender (`MockRaopSender`) — ready for swapping with a real sender

## NuGet packages used
- Zeroconf
- NAudio
- Newtonsoft.Json

## Build locally (optional)
If you ever want to build on your PC without Visual Studio:
```bash
dotnet restore AirSend/AirSend.csproj
dotnet publish AirSend/AirSend.csproj -c Release -r win-x64 --self-contained false
```
Then run `AirSend.exe` from the `publish` folder.

## Next steps
- Replace `MockRaopSender` with a real RAOP/AirPlay sender implementation.
- Add per-device volume, grouping UI, installer script, etc.
