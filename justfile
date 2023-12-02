project := "dotnet-vs-go"
github_repo := "github.com/themerski/" / project 

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
    go run {{github_repo}}/connect/cmd
