# gRPC Server Runtime Comparison

This project is designed to test the runtime differences between Go and .NET for gRPC servers. It aims to provide insights into the performance and efficiency of these two runtime environments.

## Usage

To run the gRPC server locally, you can use the [Justfile](https://github.com/casey/just) included in this project. The Justfile contains commands that simplify the process of starting the server. Here are the steps to run the servers:

1. Start one of the gRPC servers by running the start command.
   1. For the [connectRPC](https://connectrpc.com/) server use `just run-connect-server` (requires [go](https://go.dev/) to be installed)
2. The server should now be running locally and ready to accept incoming gRPC requests.
3. Start the loadtest using the following command (requires [k6](https://k6.io/docs/get-started/installation/) to be installed):
    ```bash
    just test-localhost
    ```
    or by provinding an optional rps target with:
    ```bash
    just test-localhost {rps}
    ```

## Contributing

Contributions are welcome! If you have any ideas, suggestions, or bug reports, please feel free to open an issue or submit a pull request.
