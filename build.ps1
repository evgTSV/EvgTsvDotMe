Clear-Host
$startTime = Get-Date
Write-Host "Starting build process..."

function Invoke-BuildCommand {
    param([string]$Command, [string]$ErrorMessage)
    Write-Host "Running: $Command"
    Invoke-Expression $Command
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Error: $ErrorMessage"
        exit 1
    }
}

Push-Location EvgTsvDotMe.PagesProvider
Invoke-BuildCommand "dotnet tool restore" "Failed to restore dotnet tools in PagesProvider."
Invoke-BuildCommand "dotnet paket update" "Failed to update paket dependencies in PagesProvider."
Invoke-BuildCommand "dotnet build -c Release" "Failed to build .NET project in PagesProvider."
Invoke-BuildCommand "dotnet paket pack output/" "Failed to pack the EvgTsvDotMe.PagesProvider into a NuGet package."
Pop-Location

Invoke-BuildCommand "dotnet tool restore" "Failed to restore dotnet tools."
Invoke-BuildCommand "dotnet paket update" "Failed to update paket dependencies."
Invoke-BuildCommand "dotnet paket restore" "Failed to restore paket dependencies."
Invoke-BuildCommand "dotnet build -c Release" "Failed to build .NET project."

Push-Location Tailwind
Invoke-BuildCommand "npm install" "Failed to install npm dependencies for Tailwind."
Invoke-BuildCommand "npm run build" "Failed to build Tailwind CSS."
Pop-Location

$endTime = Get-Date
$elapsed = $endTime - $startTime
Write-Host "Build and restore successful! Completed in $($elapsed.TotalSeconds) seconds." -ForegroundColor Green
