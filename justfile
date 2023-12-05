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
run-otel-collector:
    docker pull otel/opentelemetry-collector:0.90.1
    docker run -p 127.0.0.1:4317:4317 -p 127.0.0.1:55679:55679 otel/opentelemetry-collector:0.90.1

[no-exit-message]
jeager:
    docker run --rm --name jaeger \
        -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 \
        -p 6831:6831/udp \
        -p 6832:6832/udp \
        -p 5778:5778 \
        -p 16686:16686 \
        -p 4317:4317 \
        -p 4318:4318 \
        -p 14250:14250 \
        -p 14268:14268 \
        -p 14269:14269 \
        -p 9411:9411 \
        jaegertracing/all-in-one:1.51

[no-exit-message]
start-observe:
    docker compose up --force-recreate --remove-orphans --detach

[no-exit-message]
stop-observe:
    docker compose down --remove-orphans --volumes

test-localhost rps=default_rps time=default_time:
    @cd k6; \
    k6 run generic.js -e HOSTNAME='localhost:8080' -e MAX_RPS={{rps}} -e TEST_TIME={{time}} -e INSECURE=true

test hostname rps=default_rps time=default_time:
    @cd k6; \
    k6 run generic.js -e HOSTNAME='{{hostname}}' -e MAX_RPS={{rps}} -e TEST_TIME={{time}} -e INSECURE=false