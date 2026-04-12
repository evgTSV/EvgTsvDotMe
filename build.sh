#!/bin/bash

start_time=$(date +%s)
echo "Starting build process..."

dotnet tool restore
if [ $? -ne 0 ]; then
    echo "Error: Failed to restore dotnet tools."
    exit 1
fi

dotnet paket update
if [ $? -ne 0 ]; then
    echo "Error: Failed to update paket dependencies."
    exit 1
fi

dotnet paket restore
if [ $? -ne 0 ]; then
    echo "Error: Failed to restore paket dependencies."
    exit 1
fi

dotnet build -c Release
if [ $? -ne 0 ]; then
    echo "Error: Failed to build .NET project."
    exit 1
fi

# shellcheck disable=SC2164
cd Tailwind
if [ $? -ne 0 ]; then
    echo "Error: Failed to change directory to Tailwind."
    exit 1
fi

npm install
if [ $? -ne 0 ]; then
    echo "Error: Failed to install npm dependencies for Tailwind."
    exit 1
fi

npm run build
if [ $? -ne 0 ]; then
    echo "Error: Failed to build Tailwind CSS."
    exit 1
fi

end_time=$(date +%s)
elapsed=$((end_time - start_time))
echo "Build successful! Completed in $elapsed seconds."
