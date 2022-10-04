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
        public async Task<List<VStabiBattery>> Batteries(string controllerId)
        {
            var batteries = new List<VStabiBattery>();

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.Batteries(controllerId));

            var nodes = doc.DocumentNode.Descendants("tbody").Skip(1).First();

            foreach (var node in nodes.Descendants("tr"))
            {
                var imgs = node.Descendants("img").ToList();

                if (imgs.Count > 0)
                {
                    var battery = new VStabiBattery();

                    //model.ImageData = imgs[0].Attributes["src"].Value;

                    var name = node.Descendants("td").ToList()[1].InnerHtml;
                    battery.Name = name.Substring(0, name.IndexOf("<")).Trim('\0');
                    var stPos = name.IndexOf('>') + 1;
                    var thisControllerId = name.Substring(stPos, name.Length - stPos);
                    battery.ControllerId = thisControllerId;

                    var delete = node.Descendants("td").ToList()[4];
                    var deleteInput = delete.Descendants("input").ToList();
                    var deleteValue = deleteInput[0].Attributes["value"].Value;
                    var deleteValues = deleteValue.Split(',');
                    battery.Id = deleteValues[0];

                    var spec = node.Descendants("td").ToList()[0].InnerHtml;

                    var cells = spec.Substring(0, spec.IndexOf("Cells"));
                    battery.Cells = int.Parse(cells);

                    int pos = spec.IndexOf("<br>") + 4;
                    var mAh = spec.Substring(pos, spec.IndexOf("mAh") - pos);
                    battery.mAh = int.Parse(mAh);

                    var lastUsed = node.Descendants("td").ToList()[3].InnerHtml;
                    var lastFlight = lastUsed.Substring(0, lastUsed.IndexOf("<"));
                    battery.LastFlight = DateTime.Parse(lastFlight, CultureInfo.GetCultureInfo("de-DE"));
                    stPos = lastUsed.IndexOf(">") + 1;
                    var lastFlightDuration = lastUsed.Substring(stPos, lastUsed.Length - stPos);
                    battery.LastDurationS = TimeSpan.Parse(lastFlightDuration).TotalSeconds;

                    var link = node.Descendants("a").ToList()[0].Attributes[0].Value;

                    var values = HttpUtility.ParseQueryString(link);

                    var sid = values["sid"];

                    var batteryDetails = await vstabiReader.Battery(battery.Id, sid);

                    var batteryDetailDoc = new HtmlDocument();

                    batteryDetailDoc.LoadHtml(batteryDetails);

                    var tbody = batteryDetailDoc.DocumentNode.Descendants("tbody").FirstOrDefault();

                    if (tbody != null)
                    {
                        var tr = tbody.Descendants("tr").ToList()[2];

                        var td = tr.Descendants("td").ToList()[2];

                        var inner = td.InnerHtml;

                        var fnPos = inner.IndexOf("<");

                        battery.FixedId = inner.Substring(0, fnPos);

                        stPos = fnPos + 4;

                        fnPos = inner.IndexOf("<", stPos);

                        // mAh
                        stPos = fnPos + 4;

                        fnPos = inner.IndexOf(" %");

                        battery.Used = int.Parse(inner.Substring(stPos, fnPos - stPos));

                        stPos = fnPos + 6;

                        fnPos = inner.IndexOf(" V", stPos);

                        battery.CellVolt = double.Parse(inner.Substring(stPos, fnPos - stPos));

                        td = tr.Descendants("td").ToList()[4];

                        inner = td.InnerHtml;

                        stPos = inner.IndexOf("<br>") + 4;

                        fnPos = inner.IndexOf("<", stPos);

                        battery.StoreAging = int.Parse(inner.Substring(stPos, fnPos - stPos));

                        stPos = fnPos + 4;

                        fnPos = inner.IndexOf("<", stPos);

                        battery.FlightAging = int.Parse(inner.Substring(stPos, fnPos - stPos));

                        stPos = batteryDetails.IndexOf("Flights: ") + 9;

                        fnPos = batteryDetails.IndexOf("<", stPos);

                        var flights = batteryDetails.Substring(stPos, fnPos - stPos);

                        if (!string.IsNullOrWhiteSpace(flights))
                        {
                            //battery.Flights = int.Parse(flights);
                        }

                        stPos = batteryDetails.IndexOf("Consumption: ") + 13;

                        fnPos = batteryDetails.IndexOf(" Ah", stPos);

                        battery.Consumption = double.Parse(batteryDetails.Substring(stPos, fnPos - stPos));

                        stPos = batteryDetails.IndexOf("Flighttime: ") + 12;

                        fnPos = batteryDetails.IndexOf("<", stPos);

                        var flighttime = batteryDetails.Substring(stPos, fnPos - stPos);

                        var parts = flighttime.Replace("h ", "").Replace("m ", "").Replace("s", "").Split(':');

                        battery.FlightTimeS = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])).TotalSeconds;

                        var flightRows = batteryDetailDoc.DocumentNode.Descendants("tbody").Skip(1).FirstOrDefault();

                        if (flightRows != null)
                        {
                            var x = flightRows.Descendants("tr");

                            battery.BatteryFlights = new List<VStabiBatteryFlight>();

                            foreach (var y in x)
                            {
                                var flightCells = y.Descendants("td");

                                var flightCell = flightCells.First();

                                var flightLink = flightCell.Descendants("a").FirstOrDefault();

                                if (flightLink != null)
                                {
                                    var batteryFlight = new VStabiBatteryFlight();

                                    var href = flightLink.GetAttributeValue("href", null);

                                    var hrefUri = new Uri(href);
                                    batteryFlight.Model = HttpUtility.ParseQueryString(hrefUri.Query).Get("model");
                                    batteryFlight.FlightNo = int.Parse(HttpUtility.ParseQueryString(hrefUri.Query).Get("flightno"));

                                    batteryFlight.DateTime = DateTime.ParseExact(flightLink.InnerText, "dd.MM.yyyy HH:mm:ss", new DateTimeFormatInfo());

                                    var capacity = flightCells.Skip(1).First();

                                    batteryFlight.Capacity = int.Parse(capacity.InnerText.Replace(" mAh", ""));

                                    var capacityUsed = flightCells.Skip(2).First();

                                    batteryFlight.CapacityUsed = int.Parse(capacityUsed.InnerText.Replace(" mAh", ""));

                                    var duration = flightCells.ElementAt(3);

                                    batteryFlight.DurationS = TimeSpan.Parse(duration.InnerText).TotalSeconds;

                                    var voltMin = flightCells.ElementAt(4);

                                    batteryFlight.VoltMin = double.Parse(voltMin.InnerText.Replace(" V", ""));

                                    var ampsMax = flightCells.ElementAt(5);

                                    batteryFlight.AmpsMax = double.Parse(ampsMax.InnerText.Replace(" A", ""));

                                    var voltEmpty = flightCells.ElementAt(6);

                                    batteryFlight.VoltEmpty = double.Parse(voltEmpty.InnerText.Replace(" V", ""));

                                    battery.BatteryFlights.Add(batteryFlight);
                                }
                            }
                        }
                    }

                    batteries.Add(battery);
                }
            }

            return batteries;
        }
    }
}