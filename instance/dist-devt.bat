@set packer=".\util\packer\Prime.ExtensionPackager.dll" -c .\prime-client.config -e"
@set config=".\prime-client.config"

@dotnet %packer% -c %config% ..\Prime.Base\bin\Debug\netstandard2.0 --key prime:base
@dotnet %packer% -c %config% ..\Prime.Core\bin\Debug\netstandard2.0 --key prime:core
@dotnet %packer% -c %config% ..\Ext\Prime.Finance\bin\Debug\netstandard2.0
@dotnet %packer% -c %config% ..\Ext\Prime.Finance.Services\bin\Debug\netstandard2.0
@dotnet %packer% -c %config% ..\Ext\Prime.IPFS\Prime.IPFS.Win64\bin\Debug\netstandard2.0
@dotnet %packer% -c %config% ..\Ext\Prime.IPFS\Prime.IPFS.Win32\bin\Debug\netstandard2.0
@dotnet %packer% -c %config% ..\Ext\Prime.ExtensionPackager\Prime.ExtensionPackager\bin\Debug\netcoreapp2.0\
@dotnet %packer% -c %config% ..\Ext\Prime.Manager\bin\Debug\netstandard2.0 --key prime:managerservice
@dotnet %packer% -c %config% ..\Ext\Prime.Manager.Client\bin\Debug\netcoreapp2.0 --key prime:manager-client
@dotnet %packer% -c %config% ..\Ext\Prime.SocketServer\bin\Debug\netstandard2.0 --key prime:socketserver
@dotnet %packer% -c %config% ..\Ext\Prime.WebSocketServer\bin\Debug\netstandard2.0 --key prime:websocketserver
@dotnet %packer% -c %config% ..\Ext\Prime.MessagingServer\bin\Debug\netstandard2.0

@pause