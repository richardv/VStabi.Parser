namespace VStabiParser.Models
{
    using System;

    public class VStabiModel
    {
        public string Name { get; set; }

        public string ImageData { get; set; }

        public string LastFlightNo { get; set; }

        public DateTime? LastFlightTime { get; set; }

        public double? LastFlightDurationS { get; set; }

        public string ControllerId { get; set; }

        public string DeviceId { get; set; }
    }
}