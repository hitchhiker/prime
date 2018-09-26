@dotnet restore

@dotnet publish -c Release -r osx.10.10-x64 -o ../dist/osx.10.10-x64/ --self-contained
@dotnet publish -c Release -r osx.10.11-x64 -o ../dist/osx.10.11-x64/ --self-contained
@dotnet publish -c Release -r osx.10.12-x64 -o ../dist/osx.10.12-x64/ --self-contained
@dotnet publish -c Release -r osx.10.13-x64 -o ../dist/osx.10.13-x64/ --self-contained
