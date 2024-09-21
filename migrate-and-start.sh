#!/bin/bash

set -e

# Add migration if it doesn't exist (replace 'InitialCreate' with your desired name)
if ! dotnet ef migrations list | grep -q 'InitialCreate'; then
  echo "Creating migration..."
  dotnet ef migrations add InitialCreate
fi

# Apply migrations
echo "Applying migrations..."
dotnet ef database update

# Start the application
echo "Starting application..."
dotnet DreckTrack_API.dll
