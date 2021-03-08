using System;
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
using AppStatisticApi.Exceptions;
using AppStatisticApi.Utils;
using Microsoft.Extensions.Configuration;

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
        private IConfiguration config;

        public AppStatisticApiController(
            ILogger<AppStatisticApiController> logger,
            AppStatisticDbContext context,
            IConfiguration configuration
        ) {
            config = configuration;
            _logger = logger;
            db = context;
            appRepository = new AppRepository(db);
            string grpcHost = config.GetValue<string>("Dependencies:GrpcHost");
            grpcClient = new GrpcClient(grpcHost);
        }

        [HttpGet]
        public async Task<ActionResult<AppEntity>> Get([FromQuery] int id)
        {
            ActionResult<AppEntity> appActionResult = await appRepository.getById(id);
            AppEntity app = appActionResult.Value;

            if (app == null)
            {
                return NotFound($"Приложение с id = {id} не найдено в БД");
            }

            return app;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post(string url)
        {
            url = this.tryValidate(url);

            if (String.IsNullOrEmpty(url))
            {
                return BadRequest("Неверная ссылка на приложение в Google Play");
            }

            ActionResult<AppEntity> appActionResult = null;

            try
            {
                appActionResult = await appRepository.create
                (
                     new AppEntity { url = url }
                );
            }
            catch (AppAlreadyExistsException e)
            {
                return BadRequest(e.Message);
            }

            AppEntity app = appActionResult.Value;
            int newId = app.id;

            Dictionary<string, string> appStatistic = getAppStatisticById(newId);
            appRepository.update(app, appStatistic);

            string appJson = JsonSerializer.Serialize(app);

            return appJson;
        }

        private string tryValidate(string url)
        {
            string validUrl = "";

            try
            {
                validUrl = UrlFormatter.validate(url);
            }
            catch(ArgumentException e)
            {
                return "";
            }

            return validUrl;
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
