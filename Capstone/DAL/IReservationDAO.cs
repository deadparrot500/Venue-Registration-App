using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public interface IReservationDAO
    {

        bool AddReservation(int spaceID, int numberOfGuests, DateTime startDate, DateTime endDate, string reservationName);

        Reservation GetReservationDetails(string reservationName);
    }
}
