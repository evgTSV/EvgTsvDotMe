Clear-Host
$startTime = Get-Date
Write-Host "Starting build process..."

dotnet tool restore
dotnet paket restore
dotnet run --project build/build.fsproj -t Build

if (!(Test-Path ".env")) {
    Write-Host "Creating .env file with default environment variables..."
    Copy-Item -Path ".env.example" -Destination ".env" -Force
    Write-Host ".env file created successfully. Please fill it out according to the schema." -ForegroundColor Blue
}

$endTime = Get-Date
$elapsed = $endTime - $startTime
Write-Host "Build and restore successful! Completed in $($elapsed.TotalSeconds) seconds." -ForegroundColor Green

