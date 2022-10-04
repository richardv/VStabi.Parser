using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using VStabiParser.Models;

namespace VStabiParser
{
    using System.Threading.Tasks;

    public partial class VStabiParser
    {
        public async Task< List<VStabiController>> Controllers()
        {
            var controllers = new List<VStabiController>();

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.Main());

            var nodes = doc.DocumentNode.Descendants("tbody").First();

            foreach (var node in nodes.Descendants("tr"))
            {
                var imgs = node.Descendants("img").ToList();

                if (imgs.Count > 0)
                {
                    var controller = new VStabiController();

                    controller.ImageData = imgs[0].Attributes["src"].Value;

                    var name = node.Descendants("td").ToList()[1].InnerHtml;
                    controller.Name = name;

                    var id = node.Descendants("td").ToList()[2].InnerHtml;
                    controller.Id = id;

                    var sid = node.Descendants("input").First().Attributes["value"].Value;
                    controller.SoftwareId = sid;

                    controllers.Add(controller);
                }
            }

            return controllers;
        }
    }
}