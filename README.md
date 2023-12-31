# gRPC Server Runtime Comparison

This project is designed to test the runtime differences between Go and .NET for gRPC servers. It aims to provide insights into the performance and efficiency of these two runtime environments.

## Usage

To run the gRPC server locally, you can use the [justfile](https://github.com/casey/just) included in this project. The justfile contains commands that simplify the process of starting the server. Here are the steps to run the servers:

1. Start the [OpenTelemetry collector](https://opentelemetry.io/docs/collector/) and other metrics & tracing tools with `just start-observe-services`
2. Start one of the gRPC servers by running the start command.
   1. For the [connectRPC](https://connectrpc.com/) server use `just run-connect-server` (requires [go](https://go.dev/) to be installed)
   2. For the dotnet server use `just run-dotnet-server` (requires the [.NET SDK](https://dotnet.microsoft.com/en-us/download) to be installed)
3. The server should now be running locally and ready to accept incoming gRPC requests.
4. Start the load test using the following command (requires [k6](https://k6.io/docs/get-started/installation/) to be installed):
    ```bash
    just test-localhost
    ```
    or by providing an optional rps target and total test time with:
    ```bash
    just test-localhost {rps} {time_s}
    ```
5. You can stop the observation tools with `just stop-observe-services`

## Contributing

Contributions are welcome! If you have any ideas, suggestions, or bug reports, please feel free to open an issue or submit a pull request.
