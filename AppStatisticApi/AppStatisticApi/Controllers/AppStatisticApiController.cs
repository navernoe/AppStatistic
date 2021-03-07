using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppStatisticApi.Repository;
using AppStatisticApi.Storage;
using AppStatisticApi.Storage.Entities;
using AppStatisticApi.Grpc;

namespace AppStatisticApi.Controllers
{
    [ApiController]
    [Route("app")]
    public class AppStatisticApiController : ControllerBase
    {

        private readonly ILogger<AppStatisticApiController> _logger;
        private AppStatisticDbContext db;
        private AppRepository appRepository;
        private GrpcClient grpcClient;

        public AppStatisticApiController(
            ILogger<AppStatisticApiController> logger,
            AppStatisticDbContext context
        ) {
            _logger = logger;
            db = context;
            appRepository = new AppRepository(db);
            grpcClient = new GrpcClient();
        }

        [HttpGet]
        public async Task<ActionResult<AppEntity>> Get([FromQuery] int id)
        {
            ActionResult<AppEntity> appActionResult = await appRepository.getById(id);
            AppEntity app = appActionResult.Value;

            if (app == null)
            {
                return NotFound();
            }

            return app;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post(string url)
        {
            ActionResult<AppEntity> appActionResult = await appRepository.create
            (
                 new AppEntity { url = url }
            );
            AppEntity app = appActionResult.Value;
            int newId = app.id;

            Dictionary<string, string> appStatistic = getAppStatisticById(newId);
            appRepository.update(app, appStatistic);

            string appJson = JsonSerializer.Serialize(app);

            return appJson;
        }

        private Dictionary<string, string> getAppStatisticById(int id)
        {
            AppReply appStatistic = grpcClient.getAppStatistic(id);
            string decodedName = HttpUtility.HtmlDecode(appStatistic.Name);

            return new Dictionary<string, string>
            {
                ["name"] = decodedName,
                ["downloads"] = appStatistic.Downloads
            };
        }
    }
}
