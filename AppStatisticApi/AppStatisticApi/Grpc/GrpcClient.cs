using System;
using Grpc.Net.Client;

namespace AppStatisticApi.Grpc
{
    public class GrpcClient
    {
        GrpcChannel channel;
        AppStatisticLoader.AppStatisticLoaderClient client;
        public GrpcClient(string grpcHost)
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true
            );
            channel = GrpcChannel.ForAddress(grpcHost);
            client = new AppStatisticLoader.AppStatisticLoaderClient(channel);
        }

        public AppReply getAppStatistic(int id)
        {
            AppRequest request = new AppRequest { Id = id };
            AppReply reply = client.getStatistic(request);

            return reply;
        }
    }
}
