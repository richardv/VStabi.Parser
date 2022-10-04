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
        public async Task<List<VStabiFlight>> AllFlights(string controllerId)
        {
            var allFlights = new List<VStabiFlight>();

            uint page = 0;

            while (true)
            {
                var flights = await Flights(controllerId, page);

                allFlights.AddRange(flights);

                if (flights.Count != 30)
                    break;

                page++;
            }

            return allFlights;
        }

        public async Task<List<VStabiFlight>> FlightsSince(string controllerId, uint lastFlightNo)
        {
            var result = new List<VStabiFlight>();

            uint page = 0;

            while (true)
            {
                var flights = await Flights(controllerId, page);

                foreach (var flight in flights)
                {
                    if (flight.FlightNo > lastFlightNo)
                    {
                        result.Add(flight);
                    }
                    else
                    {
                        return result;
                    }
                }

                if (flights.Count != 30)
                    break;

                page++;
            }

            return result;
        }

        public async Task<List<VStabiFlight>> Flights(string controllerId, uint page = 0)
        {
            var flights = new List<VStabiFlight>();

            var doc = new HtmlDocument();
            doc.LoadHtml(await vstabiReader.Flights(controllerId, page));

            var nodes = doc.DocumentNode.Descendants("tbody").Skip(1).First();

            foreach (var node in nodes.Descendants("tr"))
            {
                var imgs = node.Descendants("img").ToList();

                if (imgs.Count > 0)
                {
                    var flight = new VStabiFlight();

                    //flight.ModelImageData = imgs[0].Attributes["src"].Value;

                    var name = node.Descendants("td").ToList()[1].InnerHtml;
                    flight.ModelName = name.Substring(0, name.IndexOf("<"));

                    //var delete = node.Descendants("td").ToList()[4].InnerHtml;

                    var lastFlight = node.Descendants("td").ToList()[3].InnerHtml;

                    var lastFlightTime = lastFlight.Substring(0, lastFlight.IndexOf("<"));
                    var lastFlightDuration = lastFlight.Substring(lastFlight.IndexOf(">") + 1, 8);

                    try
                    {
                        flight.DateAndTime = DateTime.Parse(lastFlightTime, CultureInfo.GetCultureInfo("de-DE"));
                    }
                    catch (Exception)
                    {

                    }

                    flight.DurationS = TimeSpan.Parse(lastFlightDuration).TotalSeconds;

                    var delete = node.Descendants("td").ToList()[4].Descendants("input");

                    var deleteValue = delete.ToList()[0].Attributes["value"].Value;

                    var deleteValues = deleteValue.Split(',');

                    flight.ControllerId = deleteValues[1];
                    flight.DeviceId = deleteValues[2].TrimStart('0');

                    var editModel = node.Descendants("td").ToList()[5].Descendants("a");

                    var lastFlightNoUri = editModel.Single().Attributes["href"].Value;

                    //flight.SId = HttpUtility.ParseQueryString(new Uri(HttpUtility.HtmlDecode(lastFlightNoUri)).Query)["sid"];

                    var lastFlightNo = HttpUtility.ParseQueryString(new Uri(HttpUtility.HtmlDecode(lastFlightNoUri)).Query)["flightno"];

                    flight.FlightNo = int.Parse(lastFlightNo.Trim());

                    flights.Add(flight);
                }
            }

            return flights;
        }
    }
}