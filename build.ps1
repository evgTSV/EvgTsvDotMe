Clear-Host
$startTime = Get-Date
Write-Host "Starting build process..."

dotnet tool restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error: Failed to restore dotnet tools."
    exit 1
}

dotnet paket update
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error: Failed to update paket dependencies."
    exit 1
}

dotnet paket restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error: Failed to restore paket dependencies."
    exit 1
}

dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error: Failed to build .NET project."
    exit 1
}

Push-Location Tailwind
npm install
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error: Failed to install npm dependencies for Tailwind."
    exit 1
}

npm run build
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error: Failed to build Tailwind CSS."
    exit 1
}

Pop-Location

$endTime = Get-Date
$elapsed = $endTime - $startTime
Write-Host "Build and restore successful! Completed in $($elapsed.TotalSeconds) seconds." -ForegroundColor Green
