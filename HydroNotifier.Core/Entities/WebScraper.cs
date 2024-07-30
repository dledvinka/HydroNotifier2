namespace HydroNotifier.Core.Entities;

using System.Globalization;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

public class WebScraper
{
    private readonly ILogger _log;
    private readonly HydroQuery _query;
    private readonly WebDriver _webDriver;

    public WebScraper(HydroQuery query, WebDriver webDriver, ILogger log)
    {
        _query = query;
        _webDriver = webDriver;
        _log = log;
    }

    public async Task<HydroData> GetLatestValuesAsync()
    {
        await _webDriver.Navigate().GoToUrlAsync(_query.Url);
        await Task.Delay(2000);
        var html = _webDriver.PageSource;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var dataColumns = doc.DocumentNode.SelectNodes("//*[@id=\"HAC-tab\"]/table/tbody/tr/td");
        var date = dataColumns[5].InnerText;
        var flowLitersPerSecond = double.Parse(dataColumns[7].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture) * 1000.0;
        _log.LogInformation($"Name = '{_query.Name}', Date = '{date}', FlowInLitersPerSecond = '{flowLitersPerSecond}'");

        return new HydroData(_query.Name, date, flowLitersPerSecond);
    }
}