#!/usr/bin/env bash

start_time=$(date +%s.%N)
echo "Starting build process..."

dotnet tool restore
dotnet paket restore
dotnet run --project build/build.fsproj -t Build

if [ ! -f ".env" ]; then
    echo "Creating .env file with default values..."
    run_command "cat .env.example > .env" "Failed to create .env file from .env.example."
    echo "✓ .env file created successfully, please fill it out with your configuration values."
fi

end_time=$(date +%s.%N)
elapsed=$(echo "$end_time - $start_time" | bc)
echo "Build successful! Completed in $elapsed seconds."
