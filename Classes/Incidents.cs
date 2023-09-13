using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingApp2.Classes
{
    internal class Incidents
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double PenaltyTime { get; set; }
        public int PenaltySpeed { get; set; }

        public Incidents(string name, string description, double penaltyTime, int penaltySpeed)
        {
            Name = name;
            Description = description;
            PenaltyTime = penaltyTime;
            PenaltySpeed = penaltySpeed;
        }
    }
}
