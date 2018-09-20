$exts = code --list-extensions
$commands = New-Object System.Collections.ArrayList
$realCount = 0
for ($i = 0; $i -le $exts.Length; $i++) {
    $ext = $exts[$i]
    if (-Not ([string]::IsNullOrWhiteSpace($ext)) -and ($ext -notlike "*createInstance*")) {
        $command = "call code --install-extension " + $exts[$i]
        $commands.Add($command) > $null
        $realCount++
    }
}

Write-Host "Total extensions: " $realCount

$fileTag = Read-Host "Enter your name (will be used as tag for output file name)"

$outputFile = "import/import-extensions.$fileTag.win.cmd"
Write-Host "Saving to '$outputFile'."

Out-File -FilePath $outputFile -InputObject $commands -Encoding utf8
Write-Host "File saved."

