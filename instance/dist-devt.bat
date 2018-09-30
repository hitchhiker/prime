@SET c=%cd%
@set packer=%c%\util\packer\Prime.ExtensionPackager.dll
@set config=%c%\prime-client.config
@set tmp=%c%\prime-client\tmp\publish
@set dist=Debug

@dotnet publish  ..\Ext\Prime.ExtensionPackager\Prime.ExtensionPackager -c Release -o %tmp%\Prime.ExtensionPackager
@dotnet %packer% -c %config% -e %tmp%\Prime.ExtensionPackager
@echo -------------------------------------

@dotnet publish  ..\Prime\Prime.Base -c Release -o %tmp%\Prime.Base
@dotnet %packer% -c %config% -e %tmp%\Prime.Base --key prime:base
@echo -------------------------------------

@dotnet publish  ..\Prime\Prime.Core -c Release -o %tmp%\Prime.Core
@dotnet %packer% -c %config% -e %tmp%\Prime.Core --key prime:core
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.Radiant -c Release -o %tmp%\Prime.Radiant
@dotnet %packer% -c %config% -e %tmp%\Prime.Radiant --key prime:radiant
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.PackageManager\Prime.PackageManager -c Release -o %tmp%\Prime.PackageManager
@dotnet %packer% -c %config% -e %tmp%\Prime.PackageManager --key prime:pm
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.Finance -c Release -o %tmp%\Prime.Finance
@dotnet %packer% -c %config% -e %tmp%\Prime.Finance
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.Finance.Services -c Release -o %tmp%\Prime.Finance.Services
@dotnet %packer% -c %config% -e %tmp%\Prime.Finance.Services
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.IPFS\Prime.IPFS.Lin64 -c Release -o %tmp%\Prime.IPFS.Lin64
@dotnet %packer% -c %config% -e %tmp%\Prime.IPFS.Lin64
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.IPFS\Prime.IPFS.Win64 -c Release -o %tmp%\Prime.IPFS.Win64
@dotnet %packer% -c %config% -e %tmp%\Prime.IPFS.Win64
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.IPFS\Prime.IPFS.Win32 -c Release -o %tmp%\Prime.IPFS.Win32
@dotnet %packer% -c %config% -e %tmp%\Prime.IPFS.Win32
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.Manager -c Release -o %tmp%\Prime.Manager
@dotnet %packer% -c %config% -e %tmp%\Prime.Manager --key prime:managerservice
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.MessagingServer -c Release -o %tmp%\Prime.MessagingServer
@dotnet %packer% -c %config% -e %tmp%\Prime.MessagingServer
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.SocketServer -c Release -o %tmp%\Prime.SocketServer
@dotnet %packer% -c %config% -e %tmp%\Prime.SocketServer --key prime:socketserver
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.WebSocketServer -c Release -o %tmp%\Prime.WebSocketServer
@dotnet %packer% -c %config% -e %tmp%\Prime.WebSocketServer --key prime:websocketserver
@echo -------------------------------------

@dotnet publish  ..\Ext\Prime.Manager.Client -c Release -o %tmp%\Prime.Manager.Client
@dotnet %packer% -c %config% -e %tmp%\Prime.Manager.Client --key prime:manager-client
@echo -------------------------------------


@pause