package generic

import (
	"context"
	"log/slog"

	"connectrpc.com/connect"
	"github.com/google/uuid"
	genericv1 "github.com/themerski/dotnet-vs-go/connect/internal/gen/generic/v1"
	"github.com/themerski/dotnet-vs-go/connect/internal/gen/generic/v1/genericv1connect"
	"google.golang.org/protobuf/types/known/emptypb"
)

type genericService struct{}

// GetDynamicData implements genericv1connect.GenericApiHandler.
func (*genericService) GetDynamicData(ctx context.Context, r *connect.Request[genericv1.GetDynamicDataRequest]) (*connect.Response[genericv1.GetDynamicDataResponse], error) {
	slog.Info("Received dynamic data request", "requestId", r.Msg.RequestId)
	return &connect.Response[genericv1.GetDynamicDataResponse]{
		Msg: &genericv1.GetDynamicDataResponse{
			OriginalRequestId: r.Msg.RequestId,
			UniqueResponseId:  uuid.New().String(),
		}}, nil
}

// GetStaticData implements genericv1connect.GenericApiHandler.
func (*genericService) GetStaticData(context.Context, *connect.Request[emptypb.Empty]) (*connect.Response[genericv1.GetStaticDataResponse], error) {
	return &connect.Response[genericv1.GetStaticDataResponse]{
		Msg: &genericv1.GetStaticDataResponse{
			Name:        "Static data",
			Version:     int32(1),
			Description: "This data does not change between requests.",
			Content: []*genericv1.Content{
				{
					Id:          "1",
					Name:        "Text content 1",
					Content:     "Lorum ipsum dolor sit amet.",
					ContentType: genericv1.Content_TEXT,
				},
				{
					Id:          "2",
					Name:        "Image content 2",
					Content:     "https://en.meming.world/images/en/6/6e/Surprised_Pikachu.jpg",
					ContentType: genericv1.Content_IMAGE,
				},
				{
					Id:          "3",
					Name:        "Video content 3",
					Content:     "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
					ContentType: genericv1.Content_VIDEO,
				},
			},
		},
	}, nil
}

var _ genericv1connect.GenericServiceHandler = &genericService{}

func NewGenericService() *genericService {
	return &genericService{}
}
