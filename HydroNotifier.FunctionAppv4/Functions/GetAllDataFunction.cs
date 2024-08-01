namespace HydroNotifier.FunctionAppv4.Functions;

using System;
using System.Threading.Tasks;
using HydroNotifier.Core.Storage;
using HydroNotifier.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class GetAllDataFunction
{
    [FunctionName("GetAllData")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        try
        {
            var settingsService = new SettingsService();
            var tableService = new TableService(settingsService);
            var allData = tableService.GetAll();
            var responseMessage = JsonConvert.SerializeObject(allData);

            return new OkObjectResult(responseMessage);
        }
        catch (Exception ex)
        {
            log.LogCritical(ex, $"Exception found {ex.Message}");
            return new BadRequestResult();
        }
    }
}