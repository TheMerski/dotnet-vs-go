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

test-localhost rps=default_rps time=default_time:
    @cd k6; \
    k6 run generic.js -e HOSTNAME='localhost:8080' -e MAX_RPS={{rps}} -e TEST_TIME={{time}} -e INSECURE=true

test hostname rps=default_rps time=default_time:
    @cd k6; \
    k6 run generic.js -e HOSTNAME='{{hostname}}' -e MAX_RPS={{rps}} -e TEST_TIME={{time}} -e INSECURE=false