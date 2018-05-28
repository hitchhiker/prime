@dotnet restore

@dotnet publish -c Release -r ubuntu.14.04-x64 -o ../dist/ubuntu.14.04-x64/ --self-contained
@dotnet publish -c Release -r ubuntu.14.10-x64 -o ../dist/ubuntu.14.10-x64/ --self-contained
@dotnet publish -c Release -r ubuntu.15.04-x64 -o ../dist/ubuntu.15.04-x64/ --self-contained
@dotnet publish -c Release -r ubuntu.15.10-x64 -o ../dist/ubuntu.15.10-x64/ --self-contained
@dotnet publish -c Release -r ubuntu.16.04-x64 -o ../dist/ubuntu.16.04-x64/ --self-contained
@dotnet publish -c Release -r ubuntu.16.10-x64 -o ../dist/ubuntu.16.10-x64/ --self-contained

@dotnet publish -c Release -r win7-x64 -o ../dist/win7-x64/ --self-contained
@dotnet publish -c Release -r win7-x86 -o ../dist/win7-x86/ --self-contained

@dotnet publish -c Release -r win8-x64 -o ../dist/win8-x64/ --self-contained
@dotnet publish -c Release -r win8-x86 -o ../dist/win8-x86/ --self-contained
@dotnet publish -c Release -r win8-arm -o ../dist/win8-arm/ --self-contained

@dotnet publish -c Release -r win81-x64 -o ../dist/win81-x64/ --self-contained
@dotnet publish -c Release -r win81-x86 -o ../dist/win81-x86/ --self-contained
@dotnet publish -c Release -r win81-arm -o ../dist/win81-arm/ --self-contained

@dotnet publish -c Release -r win10-x64 -o ../dist/win10-x64/ --self-contained
@dotnet publish -c Release -r win10-x86 -o ../dist/win10-x86/ --self-contained
@dotnet publish -c Release -r win10-arm -o ../dist/win10-arm/ --self-contained
@dotnet publish -c Release -r win10-arm64 -o ../dist/win10-arm64/ --self-contained

@publish-dist.mac.bat