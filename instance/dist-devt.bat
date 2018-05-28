@SET c=%cd%
@set packer=%c%\util\packer\Prime.ExtensionPackager.dll
@set config=%c%\prime-client.config
@set tmp=%c%\prime-client\tmp\publish
@set dist=Debug

@dotnet publish  ..\Prime.Base -c Release -o %tmp%\Prime.Base
@dotnet publish  ..\Prime.Core -c Release -o %tmp%\Prime.Core

@dotnet publish  ..\Ext\Prime.Finance -c Release -o %tmp%\Prime.Finance 
@dotnet publish  ..\Ext\Prime.Finance.Services -c Release -o %tmp%\Prime.Finance.Services 

@dotnet publish  ..\Ext\Prime.IPFS\Prime.IPFS.Win64 -c Release -o %tmp%\Prime.IPFS.Win64
@dotnet publish  ..\Ext\Prime.IPFS\Prime.IPFS.Win32 -c Release -o %tmp%\Prime.IPFS.Win32

@dotnet publish  ..\Ext\Prime.Manager -c Release -o %tmp%\Prime.Manager
@dotnet publish  ..\Ext\Prime.SocketServer -c Release -o %tmp%\Prime.SocketServer
@dotnet publish  ..\Ext\Prime.WebSocketServer -c Release -o %tmp%\Prime.WebSocketServer
@dotnet publish  ..\Ext\Prime.MessagingServer -c Release -o %tmp%\Prime.MessagingServer
@dotnet publish  ..\Ext\Prime.Manager.Client -c Release -o %tmp%\Prime.Manager.Client
@dotnet publish  ..\Ext\Prime.ExtensionPackager\Prime.ExtensionPackager -c Release -o %tmp%\Prime.ExtensionPackager


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