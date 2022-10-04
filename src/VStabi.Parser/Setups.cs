using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using VStabiParser.Models;

namespace VStabiParser
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public partial class VStabiParser
    {
        public async Task<List<VStabiSetup>> Setups()
        {
            var setups = new List<VStabiSetup>();

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.Setups());

            var nodes = doc.DocumentNode.Descendants("tbody");
            if (nodes.Count() < 2)
                return setups;

            foreach (var node in nodes.Skip(1).First().Descendants("tr"))
            {
                var content = node.InnerHtml;

                if (content.Length > 100)
                {
                    var setup = new VStabiSetup();

                    //        //model.ImageData = imgs[0].Attributes["src"].Value;

                    var name = node.Descendants("td").ToList()[1].InnerHtml;
                    setup.Name = name.Substring(0, name.IndexOf("<"));

                    setup.ControllerId = node.Descendants("td").ToList()[2].InnerHtml;

                    var type = node.Descendants("td").ToList()[0].InnerText;
                    setup.Type = type;
                    //        var deleteInput = delete.Descendants("input").ToList();
                    //        var deleteValue = deleteInput[0].Attributes["value"].Value;
                    //        var deleteValues = deleteValue.Split(',');
                    //        model.Id = deleteValues[0];

                    var dateAndTime = node.Descendants("td").ToList()[3].InnerHtml;

                    setup.DateAndTime = DateTime.Parse(dateAndTime, CultureInfo.GetCultureInfo("de-DE"));

                    var downloadNode = node.Descendants("td").ToList()[5];
                    var downloadTag = downloadNode.Descendants("a").ToList()[0];
                    setup.FileData = await vstabiReader.SetupFile(downloadTag.Attributes["href"].Value);
                    //        //var lastFlightDuration = lastFlight.Substring(0, name.IndexOf("<"));


                    //        //model.LastFlightTime = DateTime.Parse(lastFlightTime);
                    //        //model.LastFlightDuration = TimeSpan.Parse(lastFlightDuration);

                    setups.Add(setup);
                }
            }

            return setups;
        }
    }
}