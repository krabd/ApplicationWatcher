using System;

namespace ApplicationWatcherService.Api.Models.Options
{
    public class GrpcOptions
    {
        public const string Grpc = "Grpc";

        public string Host { get; set; }

        public int Port { get; set; }

        public Uri GetUri()
        {
            return new UriBuilder("http", Host, Port).Uri;
        }
    }
}
