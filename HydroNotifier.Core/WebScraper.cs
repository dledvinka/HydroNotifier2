﻿using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HydroNotifier.Core
{
    public class WebScraper
    {
        private readonly HydroQuery _query;
        private readonly HttpClient _httpClient;
        private readonly ILogger _log;

        public WebScraper(HydroQuery query, HttpClient httpClient, ILogger log)
        {
            _query = query;
            _httpClient = httpClient;
            _log = log;
        }

        public async Task<HydroData> GetLatestValuesAsync()
        {
            string html = await _httpClient.GetStringAsync(_query.Url);
            
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var bigTable = doc.DocumentNode.SelectNodes("//table").FirstOrDefault(tbl => tbl.HasClass("stdstationtbl"));
            var thirdTable = bigTable.SelectNodes("tr")[2];
            var rows = thirdTable.SelectNodes(".//tr");

            var lastValueRow = rows[1];
            var values = lastValueRow.SelectNodes("td");
            var date = values[0].InnerText;
            var flowLitresPerSecond = double.Parse(values[2].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture) * 1000.0d;
            _log.LogInformation($"Name = '{_query.Name}', Date = '{date}', FlowInLitresPerSecond = '{flowLitresPerSecond}'");

            return new HydroData(_query.Name, date, flowLitresPerSecond);
        }
    }
}