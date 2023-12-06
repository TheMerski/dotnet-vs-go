project := "dotnet-vs-go"
github_repo := "github.com/themerski/" / project 
default_rps := '250'
default_time := '60'


proto TARGET:
    #!/usr/bin/env bash
    case "{{TARGET}}" in
        connect)
            rm -rf connect/internal/gen
            ;;
        *)
        >&2 echo "invalid target {{TARGET}}"
        exit 1
    esac
    buf generate --template buf.gen.{{TARGET}}.yaml

proto-all: (proto 'connect')

[no-exit-message]
run-connect-server:
    @cd connect; \
    go run {{github_repo}}/connect/cmd/connect

[no-exit-message]
run-dotnet-server:
    @cd dotnet/server; \
    dotnet run --property:Configuration=Release

[no-exit-message]
start-observe-services:
    docker compose up --force-recreate --remove-orphans --detach
    @echo "starting Grafana at http://localhost:3000"
    @echo "starting Jeager at http://localhost:${JAEGER_SERVICE_PORT}"

[no-exit-message]
stop-observe-services:
    docker compose down --remove-orphans --volumes

test-localhost rps=default_rps time=default_time:
    @cd k6; \
    k6 run generic.js -e HOSTNAME='localhost:8080' -e MAX_RPS={{rps}} -e TEST_TIME={{time}} -e INSECURE=true

test hostname rps=default_rps time=default_time:
    @cd k6; \
    k6 run generic.js -e HOSTNAME='{{hostname}}' -e MAX_RPS={{rps}} -e TEST_TIME={{time}} -e INSECURE=false