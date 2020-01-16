using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }

        public string VenueName { get; set; }

        public int SpaceID { get; set; } 

        public string SpaceName { get; set; }

        public int NumberOfAttendees { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ReservedFor{ get; set; }

        public decimal CostPerDay { get; set; }

        public static DateTime CalculateReservationDepartureDate(DateTime reservationStartDate, int reservationDuration)
        {
            DateTime reservationDepartureDate = reservationStartDate.AddDays(reservationDuration);

            return reservationDepartureDate;
        }

    }
}
