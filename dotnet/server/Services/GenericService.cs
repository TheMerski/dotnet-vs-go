using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using server;

namespace server.Services
{
    public class GenericService : Generic.V1.GenericService.GenericServiceBase
    {
        private readonly ILogger<GenericService> _logger;
        public GenericService(ILogger<GenericService> logger)
        {
            _logger = logger;
        }

        public override Task<Generic.V1.GetStaticDataResponse> GetStaticData(Empty request, ServerCallContext context)
        {
            var res = new Generic.V1.GetStaticDataResponse
            {
                Name = "Static data",
                Version = 1,
                Description = "This data does not change between requests.",
            };

            res.Content.AddRange([
                new Generic.V1.Content
                {
                    Id = "1",
                    Name = "Text content 1",
                    Content_ = "Lorum ipsum dolor sit amet.",
                    ContentType = Generic.V1.Content.Types.ContentType.Text
                },
                new Generic.V1.Content
                {
                    Id = "2",
                    Name = "Image content 2",
                    Content_ = "https://en.meming.world/images/en/6/6e/Surprised_Pikachu.jpg",
                    ContentType = Generic.V1.Content.Types.ContentType.Image
                },
                new Generic.V1.Content
                {
                    Id = "3",
                    Name = "Video content 3",
                    Content_ = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                    ContentType = Generic.V1.Content.Types.ContentType.Video
                }
            ]);

            return Task.FromResult(res);
        }

        public override Task<Generic.V1.GetDynamicDataResponse> GetDynamicData(Generic.V1.GetDynamicDataRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received dynamic data request: requestId={req}", request.RequestId);
            return Task.FromResult(new Generic.V1.GetDynamicDataResponse
            {
                OriginalRequestId = request.RequestId,
                UniqueResponseId = Guid.NewGuid().ToString(),
            });
        }
    }
}
