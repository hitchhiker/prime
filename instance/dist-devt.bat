@set packer=".\util\packer\Prime.ExtensionPackager.dll"
@set config=".\prime-client.config"
@set dist=Debug

@dotnet %packer% -c %config% -e ..\Prime.Base\bin\%dist%\netstandard2.0 --key prime:base
@dotnet %packer% -c %config% -e ..\Prime.Core\bin\%dist%\netstandard2.0 --key prime:core
@dotnet %packer% -c %config% -e ..\Ext\Prime.Finance\bin\%dist%\netstandard2.0
@dotnet %packer% -c %config% -e ..\Ext\Prime.Finance.Services\bin\%dist%\netstandard2.0
@dotnet %packer% -c %config% -e ..\Ext\Prime.IPFS\Prime.IPFS.Win64\bin\%dist%\netstandard2.0
@dotnet %packer% -c %config% -e ..\Ext\Prime.IPFS\Prime.IPFS.Win32\bin\%dist%\netstandard2.0
@dotnet %packer% -c %config% -e ..\Ext\Prime.Manager\bin\%dist%\netstandard2.0 --key prime:managerservice
@dotnet %packer% -c %config% -e ..\Ext\Prime.SocketServer\bin\%dist%\netstandard2.0 --key prime:socketserver
@dotnet %packer% -c %config% -e ..\Ext\Prime.WebSocketServer\bin\%dist%\netstandard2.0 --key prime:websocketserver
@dotnet %packer% -c %config% -e ..\Ext\Prime.MessagingServer\bin\%dist%\netstandard2.0

@dotnet %packer% -c %config% -e ..\Ext\Prime.Manager.Client\bin\%dist%\netcoreapp2.0 --key prime:manager-client
@dotnet %packer% -c %config% -e ..\Ext\Prime.ExtensionPackager\Prime.ExtensionPackager\bin\%dist%\netcoreapp2.0\

@pause