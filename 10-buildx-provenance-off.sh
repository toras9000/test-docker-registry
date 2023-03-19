#!/usr/bin/bash

script_dir=$(cd $(dirname ${BASH_SOURCE:-$0}); pwd)
builder_name=private-builder

bash "$script_dir/00-start-compose.sh"
bash "$script_dir/01-create-builder.sh"

docker buildx build \
    --builder "$builder_name" \
    --platform linux/amd64,linux/arm64,linux/arm/v7 \
    --pull \
    --push \
    --provenance=false \
    --tag localhost:5000/my-img:provenance-off \
    "${script_dir}/images/my-img"
