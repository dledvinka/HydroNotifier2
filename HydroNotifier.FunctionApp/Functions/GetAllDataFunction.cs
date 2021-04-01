using System;
using System.IO;
using System.Threading.Tasks;
using HydroNotifier.FunctionApp.Storage;
using HydroNotifier.FunctionApp.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HydroNotifier.FunctionApp.Functions
{
    public static class GetAllDataFunction
    {
        [FunctionName("GetAllData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var settingsService = new SettingsService();
                var tableService = new TableService(settingsService);
                var allData = tableService.GetAll();
                string responseMessage = JsonConvert.SerializeObject(allData);

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, $"Exception found {ex.Message}");
                return new BadRequestResult();
            }
        }
    }
}

