#!/bin/sh
echo "Removing nitro temp"
rm -rf /tmp/nitro
if [ "$BUILD_ENV" = "dev" ]; then
    echo "Starting in development mode..."
    npm run dev
elif [ "$BUILD_ENV" = "prod" ]; then
    echo "Building for production..."
    npm run generate
    npm run build
    node .output/server/index.mjs
else
    echo "Unknown environment. Using default command."
    npm run build
    npm run generate
    node .output/server/index.mjs
fi