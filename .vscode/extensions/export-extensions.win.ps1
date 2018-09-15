$ext = code --list-extensions
for($i = 0; $i -le $ext.Length; $i++)
{
    Write-Host $ext[$i]
}