#!/bin/bash

start_time=$(date +%s.%N)
echo "Starting build process..."

run_command() {
    echo "Running: $1"
    eval $1
    if [ $? -ne 0 ]; then
        echo "Error: $2"
        exit 1
    fi
}

pushd EvgTsvDotMe.PagesProvider
run_command "dotnet tool restore" "Failed to restore dotnet tools in PagesProvider."
run_command "dotnet paket update" "Failed to update paket dependencies in PagesProvider."
run_command "dotnet build -c Release" "Failed to build .NET project in PagesProvider."
run_command "dotnet paket pack output/" "Failed to pack the EvgTsvDotMe.PagesProvider into a NuGet package."
popd

run_command "dotnet tool restore" "Failed to restore dotnet tools."
run_command "dotnet paket update" "Failed to update paket dependencies."
run_command "dotnet paket restore" "Failed to restore paket dependencies."
run_command "dotnet build -c Release" "Failed to build .NET project."

# Copy htmx.min.js from paket-files to wwwroot
echo "Copying htmx.min.js to wwwroot/js..."
source_file="paket-files/cdn.jsdelivr.net/htmx.min.js"
dest_dir="EvgTsvDotMe/wwwroot/js"

if [ -f "$source_file" ]; then
    mkdir -p "$dest_dir"
    cp "$source_file" "$dest_dir/"
    if [ $? -eq 0 ]; then
        echo "✓ htmx.min.js copied successfully"
    else
        echo "Error: Failed to copy htmx.min.js"
        exit 1
    fi
else
    echo "Error: Source file not found: $source_file"
    exit 1
fi

pushd Tailwind
run_command "npm install" "Failed to install npm dependencies for Tailwind."
run_command "npm run build" "Failed to build Tailwind CSS."
popd

end_time=$(date +%s.%N)
elapsed=$(echo "$end_time - $start_time" | bc)
echo "Build successful! Completed in $elapsed seconds."
