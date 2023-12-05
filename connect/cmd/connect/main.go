package main

import (
	"context"
	"errors"
	"log/slog"
	"net"
	"net/http"
	"time"

	"connectrpc.com/connect"
	"connectrpc.com/grpcreflect"
	"connectrpc.com/otelconnect"
	"github.com/prometheus/client_golang/prometheus/promhttp"
	"github.com/themerski/dotnet-vs-go/connect/internal/gen/generic/v1/genericv1connect"
	"github.com/themerski/dotnet-vs-go/connect/internal/otel"
	"github.com/themerski/dotnet-vs-go/connect/internal/service/generic"
	"golang.org/x/net/http2"
	"golang.org/x/net/http2/h2c"
)

func main() {
	ctx, stop := context.WithCancel(context.Background())
	defer stop()

	otelShutdown, err := otel.SetupOTelSDK(ctx, "connect-server", "1")
	if err != nil {
		slog.Error("Failed to setup OpenTelemetry SDK", "error", err)
		return
	}
	// Handle shutdown properly so nothing leaks.
	defer func() {
		err = errors.Join(err, otelShutdown(context.Background()))
	}()

	// Setup the generic service and the handler for the generic API.
	genericService := generic.NewGenericService()
	mux := http.NewServeMux()
	path, handler := genericv1connect.NewGenericServiceHandler(genericService, connect.WithInterceptors(
		otelconnect.NewInterceptor(),
	))
	mux.Handle(path, handler)

	// Setup the reflection service and the handler for the reflection API.
	reflectHandler := grpcreflect.NewStaticReflector(
		genericv1connect.GenericServiceName,
	)
	mux.Handle(grpcreflect.NewHandlerV1(reflectHandler))
	mux.Handle(grpcreflect.NewHandlerV1Alpha(reflectHandler))
	mux.Handle("/metrics", promhttp.Handler())

	slog.Info("Starting server on port 8080")

	// Start HTTP server.
	srv := &http.Server{
		Addr:         ":8080",
		BaseContext:  func(_ net.Listener) context.Context { return ctx },
		ReadTimeout:  time.Second,
		WriteTimeout: 10 * time.Second,
		Handler:      h2c.NewHandler(mux, &http2.Server{}),
	}
	srvErr := make(chan error, 1)
	go func() {
		srvErr <- srv.ListenAndServe()
	}()

	// Wait for interruption.
	select {
	case err = <-srvErr:
		// Error when starting HTTP server.
		slog.Error("Failed to start server", "error", err)
		return
	case <-ctx.Done():
		// Wait for first CTRL+C.
		// Stop receiving signal notifications as soon as possible.
		stop()
	}

	// When Shutdown is called, ListenAndServe immediately returns ErrServerClosed.
	err = srv.Shutdown(context.Background())
	slog.Info("Server stopped")
	return
}
