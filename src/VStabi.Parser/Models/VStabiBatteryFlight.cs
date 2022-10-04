namespace VStabiParser.Models
{
    using System;

    public class VStabiBatteryFlight
    {
        public int FlightNo { get; set; }

        public string Model { get; set; }

        public DateTime DateTime { get; set; }

        public int Capacity { get; set; }

        public int CapacityUsed { get; set; }

        public double DurationS { get; set; }

        public double VoltMin { get; set; }

        public double AmpsMax { get; set; }

        public double VoltEmpty { get; set; }
    }
}