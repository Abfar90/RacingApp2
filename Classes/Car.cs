using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingApp2.Classes
{
    internal class Car
    {
        public string Name { get; set; }

        public int Speed { get; set; } = 120;

        public double RaceDistance { get; set; } = 10000;

        public double DistanceDriven { get; set; } = 0;

        public double RaceTime { get; set; }

        public double Penalty { get; set; } = 0;

        public string IncidentReport { get; set; } = "No Incident";

        public Car(string name)
        {
            Name = name;
        }
    }
}
