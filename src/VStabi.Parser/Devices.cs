namespace VStabiParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using global::VStabiParser.Models;
    using HtmlAgilityPack;

    public partial class VStabiParser
    {
        public async Task< List<VStabiDevice>> Devices()
        {
            var devices = new List<VStabiDevice>();

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.Devices());

            var nodes = doc.DocumentNode.Descendants("tbody").First();

            foreach (var node in nodes.Descendants("tr"))
            {
                var serialNo = node.Descendants("td").ToList()[0].InnerText;

                if (serialNo == "Keyfile")
                {
                    break;
                }

                if (!string.IsNullOrWhiteSpace(serialNo))
                {
                    var device = new VStabiDevice
                    {
                        SerialNo = serialNo
                    };

                    var name = node.Descendants("td").ToList()[1].InnerHtml;
                    device.Name = name;

                    var note = node.Descendants("td").ToList()[2].InnerHtml;
                    device.Note = note;

                    var sidNode = new Uri(node.Descendants("td").ToList()[5].Descendants("a").First().Attributes["href"].Value);
                    device.SId = HttpUtility.ParseQueryString(sidNode.Query).Get("Sid");

                    var type = new Uri(node.Descendants("td").ToList()[6].Descendants("a").First().Attributes["href"].Value);
                    device.Type = int.Parse(HttpUtility.ParseQueryString(type.Query).Get("pre_group"));

                    var deviceSettings = await vstabiReader.DeviceSettings(device.SId);

                    device.Update = deviceSettings.Contains("&gt;Update&lt;");

                    devices.Add(device);
                }
            }

            return devices;
        }
    }
}