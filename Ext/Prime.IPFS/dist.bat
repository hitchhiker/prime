@cd V:\prime\src\publish
@V:

@dotnet .\packer\Prime.ExtensionPackager.dll ..\Ext\Prime.IPFS\Prime.IPFS.Win64\bin\Debug\ .\stage .\dist
@dotnet .\packer\Prime.ExtensionPackager.dll ..\Ext\Prime.IPFS\Prime.IPFS.Win32\bin\Debug\ .\stage .\dist