package main

import (
	"context"
	"log/slog"
	"net/http"

	"connectrpc.com/grpcreflect"
	"github.com/themerski/dotnet-vs-go/connect/internal/gen/generic/v1/genericv1connect"
	"github.com/themerski/dotnet-vs-go/connect/internal/service/generic"
	"golang.org/x/net/http2"
	"golang.org/x/net/http2/h2c"
)

func main() {
	_, stop := context.WithCancel(context.Background())
	defer stop()

	// Setup the generic service and the handler for the generic API.
	genericService := generic.NewGenericService()
	mux := http.NewServeMux()
	path, handler := genericv1connect.NewGenericServiceHandler(genericService)
	mux.Handle(path, handler)

	// Setup the reflection service and the handler for the reflection API.
	reflectHandler := grpcreflect.NewStaticReflector(
		genericv1connect.GenericServiceName,
	)
	mux.Handle(grpcreflect.NewHandlerV1(reflectHandler))
	mux.Handle(grpcreflect.NewHandlerV1Alpha(reflectHandler))

	slog.Info("Starting server on port 8080")
	http.ListenAndServe("localhost:8080", h2c.NewHandler(mux, &http2.Server{}))
	slog.Info("Server stopped")
}
