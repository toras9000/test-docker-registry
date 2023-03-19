#!/usr/bin/bash

script_dir=$(cd $(dirname ${BASH_SOURCE:-$0}); pwd)
builder_name=private-builder
toml_file=$script_dir/builder/buildkit.toml

if [ "$(docker buildx inspect "$builder_name" > /dev/null 2>&1; echo $?)" = "0" ]; then
    docker buildx rm "$builder_name"
fi

docker buildx create \
    --name "$builder_name" \
    --driver-opt network=host \
    --platform linux/amd64,linux/arm64,linux/arm/v7 \
    --config "$toml_file"
