"Copying files..."

if(-Not (Test-Path "main.js")) {
    Throw "Unable to find 'main.js' file."; 
}

if(-Not (Test-Path "package.json")) {
    Throw "Unable to find 'package.json file.'";
}

Copy-Item "main.js" -Destination "dist/prime-manager";
Copy-Item "package.json" -Destination "dist/prime-manager";

"Files copied.";
"Running packager...";

if(-Not (Get-Command electron-packager -ErrorAction SilentlyContinue)) {
    Throw "Command 'electron-packager' does not exist. Please run 'npm install electron-packager -g'.";
    
}

electron-packager ./dist/prime-manager/ --asar --arch ia32 --out ./dist/prime-manager/packed/
