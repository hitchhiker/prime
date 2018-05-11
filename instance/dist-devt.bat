echo @dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Prime.Core\bin\Debug\netstandard2.0 -core
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.Finance\bin\Debug\netstandard2.0
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.Finance.Services\bin\Debug\netstandard2.0
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.IPFS\Prime.IPFS.Win64\bin\Debug\
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.IPFS\Prime.IPFS.Win32\bin\Debug\
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.ExtensionPackager\Prime.ExtensionPackager\bin\Debug\netcoreapp2.0\
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.KeysManager\bin\Debug\netcoreapp2.0
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.SocketServer\bin\Debug\netstandard2.0
@dotnet ..\..\util\packer\Prime.ExtensionPackager.dll .\prime-client.config ..\Ext\Prime.MessageServer\bin\Debug\netstandard2.0
@pause