#!/usr/bin/bash

script_dir=$(cd $(dirname ${BASH_SOURCE:-$0}); pwd)
registry_compose=${script_dir}/registry/docker-compose.yml

docker compose --file "$registry_compose" up -d

