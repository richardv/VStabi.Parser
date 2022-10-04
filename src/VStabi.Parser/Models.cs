namespace VStabiParser
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using global::VStabiParser.Models;
    using HtmlAgilityPack;

    public partial class VStabiParser
    {
        public async Task<List<VStabiModel>> Models(string controllerId)
        {
            var models = new List<VStabiModel>();

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.Models(controllerId));

            var nodes = doc.DocumentNode.Descendants("tbody").Skip(1).First();

            foreach (var node in nodes.Descendants("tr"))
            {
                try
                {
                    var imgs = node.Descendants("img").ToList();

                    if (imgs.Count > 0)
                    {
                        var model = new VStabiModel();

                        model.ImageData = imgs[0].Attributes["src"].Value;

                        var name = node.Descendants("td").ToList()[1].InnerHtml;
                        model.Name = name.Substring(0, name.IndexOf("<"));

                        //var delete = node.Descendants("td").ToList()[4].InnerHtml;

                        var lastFlight = node.Descendants("td").ToList()[3].InnerHtml;

                        var lastFlightTime = lastFlight.Substring(0, lastFlight.IndexOf("<"));
                        var lastFlightDuration = lastFlight.Substring(lastFlight.IndexOf(">") + 1, 8);

                        model.LastFlightTime = DateTime.Parse(lastFlightTime, CultureInfo.GetCultureInfo("de-DE"));
                        model.LastFlightDurationS = TimeSpan.Parse(lastFlightDuration).TotalSeconds;

                        var delete = node.Descendants("td").ToList()[4].Descendants("input");

                        var deleteValue = delete.ToList()[0].Attributes["value"].Value;

                        var deleteValues = deleteValue.Split(',');

                        model.ControllerId = deleteValues[1];
                        model.DeviceId = deleteValues[2].TrimStart('0');

                        var editModel = node.Descendants("td").ToList()[5].Descendants("a");

                        var lastFlightNoUri = editModel.Single().Attributes["href"].Value;

                        var lastFlightNo = HttpUtility.ParseQueryString(new Uri(HttpUtility.HtmlDecode(lastFlightNoUri)).Query)["flightno"];

                        model.LastFlightNo = lastFlightNo.Trim();

                        models.Add(model);
                    }
                }
                catch
                {
                    // Ignore this model
                }
            }

            return models;
        }
    }
}