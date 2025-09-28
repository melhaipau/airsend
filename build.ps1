
dotnet restore AirSend/AirSend.csproj
dotnet publish AirSend/AirSend.csproj -c Release -r win-x64 --self-contained false
Write-Host "Publish complete: AirSend/bin/Release/net8.0-windows/win-x64/publish/"
