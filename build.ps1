param([switch] $SkipRestore)
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

if(!(Test-Path "EvgTsvDotMe.PagesProvider/output/EvgTsvDotMe.PagesProvider.*.nupkg")) {
    Write-Host "EvgTsvDotMe.PagesProvider NuGet package not found. Building PagesProvider first..." -ForegroundColor Yellow
    Push-Location EvgTsvDotMe.PagesProvider
    Invoke-BuildCommand "dotnet tool restore" "Failed to restore dotnet tools in PagesProvider."
    Invoke-BuildCommand "dotnet paket update" "Failed to update paket dependencies in PagesProvider."
    Invoke-BuildCommand "dotnet build -c Release" "Failed to build .NET project in PagesProvider."
    Invoke-BuildCommand "dotnet paket pack output/" "Failed to pack the EvgTsvDotMe.PagesProvider into a NuGet package."
    Pop-Location
}
else {
    Write-Host "NuGet package for PagesProvider already exists. Skipping build and pack steps." -ForegroundColor Blue
}

if (-not $SkipRestore) {
    Invoke-BuildCommand "dotnet tool restore" "Failed to restore dotnet tools."
    Invoke-BuildCommand "dotnet paket update" "Failed to update paket dependencies."
    Invoke-BuildCommand "dotnet paket restore" "Failed to restore paket dependencies."
    Invoke-BuildCommand "dotnet restore" "Failed to restore dependencies."
}
else {
    Write-Host "Skipping restore steps as per the -SkipRestore flag." -ForegroundColor Blue
}

Invoke-BuildCommand "dotnet build -c Release --no-restore" "Failed to build .NET project."

if (!(Test-Path "EvgTsvDotMe\wwwroot\js\htmx.min.js"))
{
    # Copy htmx.min.js from paket-files to wwwroot
    Write-Host "Copying htmx.min.js to wwwroot/js..."
    $sourceFile = "paket-files\cdn.jsdelivr.net\htmx.min.js"
    $destDir = "EvgTsvDotMe\wwwroot\js"
    if (Test-Path $sourceFile)
    {
        if (-not (Test-Path $destDir))
        {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        }
        Copy-Item -Path $sourceFile -Destination $destDir -Force
        Write-Host "htmx.min.js copied successfully"
    }
    else
    {
        Write-Error "Source file not found: $sourceFile"
        exit 1
    }
}
else {
    Write-Host "htmx.min.js already exists in wwwroot/js. Skipping copy." -ForegroundColor Blue
}

Push-Location Tailwind
Invoke-BuildCommand "npm install" "Failed to install npm dependencies for Tailwind."
Invoke-BuildCommand "npm run build" "Failed to build Tailwind CSS."
Pop-Location

if (!(Test-Path ".env")) {
    Write-Host "Creating .env file with default environment variables..."
    Copy-Item -Path ".env.example" -Destination ".env" -Force
    Write-Host ".env file created successfully. Please fill it out according to the schema." -ForegroundColor Blue
}

$endTime = Get-Date
$elapsed = $endTime - $startTime
Write-Host "Build and restore successful! Completed in $($elapsed.TotalSeconds) seconds." -ForegroundColor Green

