namespace VStabiParser.Models
{
    using System;
    using System.Collections.Generic;

    public class VStabiBattery
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Cells { get; set; }

        public int mAh { get; set; }

        public string ControllerId { get; set; }

        public DateTime LastFlight { get; set; }

        public double LastDurationS { get; set; }

        public int Used { get; set; }

        public double CellVolt { get; set; }

        public List<VStabiBatteryFlight> BatteryFlights { get; set; }

        public double Consumption { get; set; }

        public double FlightTimeS { get; set; }

        public string FixedId { get; set; }

        public int StoreAging { get; set; }

        public int FlightAging { get; set; }
    }
}