﻿namespace HydroNotifier.Core.Entities;

using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Xml.XPath;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

public class WebScraper
{
    private readonly ILogger _log;
    private readonly HydroQuery _query;
    private readonly HttpClient _httpClient;

    public WebScraper(HydroQuery query, HttpClient httpClient, ILogger log)
    {
        _query = query;
        _httpClient = httpClient;
        _log = log;
    }

    public async Task<HydroData> GetLatestValuesAsync()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var html = await _httpClient.GetStringAsync(_query.Url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var table = doc.DocumentNode.SelectSingleNode("//*[@id=\"ObsahCPH_DataMereniGV\"]");
        var rows = table.SelectNodes("tr");
        var cells = rows[1].SelectNodes("td");
        var dateValueElement = cells[0];
        var flowValueElement = cells[2];
        var dateValue = dateValueElement.InnerText.Trim();
        var flowValue = flowValueElement.InnerText.Trim().Split(" ")[0];
        var flowLitersPerSecond = double.Parse(flowValue, NumberStyles.Any, new CultureInfo("cs-CZ")) * 1000.0;
        _log.LogInformation($"Name = '{_query.Name}', Date = '{dateValue}', FlowInLitersPerSecond = '{flowLitersPerSecond}'");

        return new HydroData(_query.Name, dateValue, flowLitersPerSecond);
    }
}