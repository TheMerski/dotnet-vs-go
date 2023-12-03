project := "dotnet-vs-go"
github_repo := "github.com/themerski/" / project 
default_rps := '250'


proto TARGET:
    #!/usr/bin/env bash
    case "{{TARGET}}" in
        connect)
            rm -rf connect/internal/gen
            ;;
        >&2 echo "invalid target {{TARGET}}"
        exit 1
    esac
    buf generate --template buf.gen.{{TARGET}}.yaml

proto-all: (proto 'connect')

[no-exit-message]
run-connect-server:
    @cd connect; \
    go run {{github_repo}}/connect/cmd/connect

test-localhost rps=default_rps:
    @cd k6; \
    k6 run generic.js -e HOSTNAME='localhost:8080' -e MAX_RPS={{rps}}