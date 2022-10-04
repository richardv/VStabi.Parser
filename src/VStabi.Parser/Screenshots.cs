namespace VStabiParser
{
    using HtmlAgilityPack;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class VStabiParser
    {
        public async Task<List<VStabiScreenshot>> Screenshots(string controllerId, uint page = 0)
        {
            var screenshots = new List<VStabiScreenshot>();

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.Screenshots(controllerId, page));

            var nodes = doc.DocumentNode.Descendants("tbody").Skip(1).First();

            foreach (var node in nodes.Descendants("tr"))
            {
                var imgs = node.Descendants("img").ToList();

                if (imgs.Count > 0)
                {
                    var screenshot = new VStabiScreenshot();

                    screenshot.ImageData = imgs[0].Attributes["src"].Value;

                    var name = node.Descendants("td").ToList()[1].InnerHtml;
                    screenshot.Name = name.Substring(0, name.IndexOf("<")).Trim();

                    var date = name.Substring(name.IndexOf(">") + 1, 10);

                    try
                    {
                        screenshot.Date = DateTime.Parse(date, CultureInfo.GetCultureInfo("de-DE"));
                    }
                    catch (Exception)
                    {

                    }

                    screenshots.Add(screenshot);
                }
            }

            return screenshots;
        }
    }
}
