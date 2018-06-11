Write-Host "`n---------------------------------------"
Write-Host "---- Prime.Manager Firewall Setup -----"
Write-Host "---------------------------------------`n"

Write-Host "This script will setup Firewall rule for 'dotnet' process to be available only from LAN.`n"

$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent());
if (-Not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Throw "Please run script with administrative privilages.";
}

$primaryIpAddresses = Get-WmiObject Win32_NetworkAdapterConfiguration | Where-Object {$_.IPEnabled} | Select-Object -ExpandProperty IPAddress;
If ($primaryIpAddresses.Length -eq 0) {
    Throw "Unable to get primary IP address object."
}

$primaryIp = $primaryIpAddresses[0];

$primaryMask = (Get-NetIPAddress | Where-Object {$_.IPAddress -eq $primaryIp} | Select-Object -ExpandProperty PrefixLength);

$dotnetCmdName = "dotnet";

if(-Not (Get-Command $dotnetCmdName -ErrorAction SilentlyContinue)) {
    Throw "Command 'dotnet' does not exist. Please install .NET Core runtime."
}

$dotnetPath = Get-Command $dotnetCmdName | Select-Object -ExpandProperty Source;

$primePort = '9991';

Write-Host "Your IP is '$primaryIp/$primaryMask'.";
Write-Host "Path to 'dotnet' executable is '$dotnetPath'.";

$newPort = Read-Host "Enter Prime.Manager port or press Enter to leave it default '$primePort'? ";
if($newPort) {
    $primePort = $newPort;
    Write-Host "Prime.Manager port changed to '$primePort'."
}

$ruleName = "prime.manager.webserver";

Write-Host "Checking Firewall rules...";

if((Get-NetFirewallRule | Where-Object {$_.Name -eq $ruleName})) {
    Throw "Firewall rule already exists.";
}

Write-Host "Setting up Firewall rule...";

if(New-NetFirewallRule -Name $ruleName -DisplayName "Prime.Manager Inbound LAN" -Program $dotnetPath -Direction Inbound -Protocol TCP -Action Allow -RemoteAddress LocalSubnet -LocalPort $primePort) {
    Write-Host "Done!";
}

