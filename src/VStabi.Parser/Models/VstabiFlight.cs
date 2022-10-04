namespace VStabiParser.Models
{
    using System;

    public class VStabiFlight
    {
        public string ModelName { get; set; }

        public DateTime DateAndTime { get; set; }

        public double DurationS { get; set; }

        public int FlightNo { get; set; }

        public string ControllerId { get; set; }

        public string DeviceId { get; set; }
    }
}