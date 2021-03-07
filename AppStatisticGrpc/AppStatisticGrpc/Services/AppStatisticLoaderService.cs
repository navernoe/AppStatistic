using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.Json;

using Grpc.Core;
using Microsoft.Extensions.Logging;

using AppStatisticGrpc.Exceptions;

namespace AppStatisticGrpc
{
    public class AppStatisticLoaderService : AppStatisticLoader.AppStatisticLoaderBase
    {
        private readonly ILogger<AppStatisticLoaderService> _logger;
        public AppStatisticLoaderService(ILogger<AppStatisticLoaderService> logger)
        {
            _logger = logger;
        }

        public async override Task<AppReply> getStatistic(AppRequest request, ServerCallContext context)
        {
            AppStatistic app = await getAppById(request.Id);
            GooglePlayAppDataScrapper googlePlayAppDataScrapper = null;

            try
            {
                googlePlayAppDataScrapper = new GooglePlayAppDataScrapper(app.url);
            }
            catch (InvalidAppUrlException e)
            {
                _logger.LogError(e, e.Message);

                return new AppReply
                {
                    Error = e.Message
                };
            }
            

            app.name = googlePlayAppDataScrapper.getAppName();
            app.downloads = googlePlayAppDataScrapper.getDownloadsStatistic();

            return new AppReply
            {
                Name = app.name,
                Downloads = app.downloads
            };
        }

        private async Task<AppStatistic> getAppById(int id)
        {
            string appInfo = "";
            string apiUrl = "https://localhost:5001/app?id=" + id;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    appInfo += reader.ReadToEnd();
                }
            }
            response.Close();

            AppStatistic app = JsonSerializer.Deserialize<AppStatistic>(appInfo);

            return app;
        }

    }
}
