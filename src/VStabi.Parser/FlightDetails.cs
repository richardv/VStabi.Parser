namespace VStabiParser
{
    using global::VStabiParser.Models;
    using HtmlAgilityPack;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class VStabiParser
    {
        public async Task<VStabiFlightDetail> FlightDetail(string sid, int flightNo)
        {
            var result = new VStabiFlightDetail
            {
                FlightNo = flightNo,
                SId = sid
            };

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.FlightDetail(sid, flightNo));

            var tbody = doc.DocumentNode.Descendants("tbody").Skip(1).First();

            var trs = tbody.Descendants("tr");

            var trValues = trs.ToList()[0];

            var tds = trValues.Descendants("td");

            var tdValues = tds.ToList()[3].InnerHtml;

            var stPos = 0;

            var fnPos = tdValues.IndexOf("<");

            result.Date = DateTime.ParseExact(tdValues.Substring(stPos, fnPos), "dd.MM.yyyy HH:mm:ss", new DateTimeFormatInfo());

            stPos = 46;

            fnPos = stPos + 8;

            var duration = TimeSpan.Parse(tdValues.Substring(stPos, fnPos - stPos));

            result.DurationS = duration.TotalSeconds;

            stPos = 58;

            fnPos = tdValues.IndexOf(' ', stPos);

            result.Capacity = int.Parse(tdValues.Substring(stPos, fnPos - stPos));

            stPos = tdValues.IndexOf(">", fnPos) + 1;

            fnPos = tdValues.IndexOf("<", stPos);

            result.VoltStart = double.Parse(tdValues.Substring(stPos, fnPos - stPos - 2));

            tdValues = tds.ToList()[5].InnerHtml;

            fnPos = tdValues.IndexOf(" ");
            result.CapacityUsed = int.Parse(tdValues.Substring(0, fnPos));

            stPos = tdValues.IndexOf(">", fnPos) + 1;
            fnPos = tdValues.IndexOf(" ", stPos);
            result.VoltMin = double.Parse(tdValues.Substring(stPos, fnPos - stPos));

            stPos = tdValues.IndexOf(">", fnPos) + 1;
            fnPos = tdValues.IndexOf(" ", stPos);
            result.VoltEnd = double.Parse(tdValues.Substring(stPos, fnPos - stPos));

            stPos = tdValues.IndexOf(">", fnPos) + 1;
            fnPos = tdValues.IndexOf(" ", stPos);
            result.AmpsMax = double.Parse(tdValues.Substring(stPos, fnPos - stPos));

            stPos = tdValues.IndexOf(">", fnPos) + 1;
            fnPos = tdValues.IndexOf(" ", stPos);
            result.WattsMax = int.Parse(tdValues.Substring(stPos, fnPos - stPos));

            tbody = doc.DocumentNode.Descendants("tbody").Skip(2).First();

            stPos = tbody.InnerText.IndexOf("Good;/battery/");

            if (stPos >= 0)
            {
                stPos += 14;

                fnPos = tbody.InnerText.IndexOf("\r", stPos);

                result.BatteryId = tbody.InnerText.Substring(stPos, fnPos - stPos);
            }

            stPos = tbody.InnerText.IndexOf("Good;Battery by Batt ID:");

            if (stPos >= 0)
            {
                stPos += 25;

                fnPos = tbody.InnerText.IndexOf("\r", stPos);

                result.BatteryId = tbody.InnerText.Substring(stPos, fnPos - stPos);
            }

            stPos = tbody.InnerText.IndexOf("Good;Battery selected manually:");

            if (stPos >= 0)
            {
                stPos += 32;

                fnPos = tbody.InnerText.IndexOf("\r", stPos);

                result.BatteryId = tbody.InnerText.Substring(stPos, fnPos - stPos);
            }

            try
            {
                var textareas = tbody.Descendants("textarea").ToList();

                if (textareas.Count >= 1)
                {
                    var eventArea = textareas.First();

                    result.EventLogs = eventArea.InnerText;
                }

                if (textareas.Count >= 2)
                {
                    var batteryArea = textareas.Skip(1).First();

                    result.BatteryLogs = batteryArea.InnerText;
                }

                if (textareas.Count >= 3)
                {
                    var customArea = textareas.Skip(2).First();

                    result.CustomLogs = customArea.InnerText;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return result;
        }
    }
}