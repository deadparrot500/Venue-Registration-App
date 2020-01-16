using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        public string VenueName { get; set; }

        public string VenueCityName { get; set; }

        public string VenueStateAbbreviation { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public string VenueDescription { get; set; }       
    }
}
