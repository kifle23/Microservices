using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient(IMapper mapper) : IPlatformDataClient
    {
        private readonly IMapper _mapper = mapper;

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling GRPC Service {Environment.GetEnvironmentVariable("GrpcPlatform")}");
            var channel = GrpcChannel.ForAddress(Environment.GetEnvironmentVariable("GrpcPlatform"));
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Couldnot call GRPC Server {ex.Message}");
                return [];
            }
        }
    }
}
