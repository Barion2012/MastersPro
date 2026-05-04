#!/bin/sh

if [ "$BUILD_ENV" = "dev" ]; then
    echo "Starting VitePress..."
    npm run docs:dev --port=5174
else
    echo "Prodiction version, skipping VitePress"
    exit 0
fi
