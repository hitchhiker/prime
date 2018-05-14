@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Prime.Base\bin\Debug\netstandard2.0 --key prime:base
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Prime.Core\bin\Debug\netstandard2.0 --key prime:core
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.Finance\bin\Debug\netstandard2.0
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.Finance.Services\bin\Debug\netstandard2.0
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.IPFS\Prime.IPFS.Win64\bin\Debug\
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.IPFS\Prime.IPFS.Win32\bin\Debug\
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.ExtensionPackager\Prime.ExtensionPackager\bin\Debug\netcoreapp2.0\
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.KeysManager\bin\Debug\netcoreapp2.0 --key prime:KeysManagerExtension
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.SocketServer\bin\Debug\netstandard2.0 --key prime:socketserver
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.WebSocketServer\bin\Debug\netstandard2.0 --key prime:websocketserver
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll -c .\prime-client.config -e ..\Ext\Prime.MessagingServer\bin\Debug\netstandard2.0
@pause