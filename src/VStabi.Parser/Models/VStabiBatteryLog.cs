namespace VStabiParser.Models
{
    using System;

    public class VStabiBatteryLog
    {
        public DateTime DateTime { get; set; }

        public double Amps { get; set; }

        public double Voltage { get; set; }

        public double MAh { get; set; }

        public int Headspeed { get; set; }

        public int Pwm { get; set; }

        public int TempC { get; set; }
    }
}