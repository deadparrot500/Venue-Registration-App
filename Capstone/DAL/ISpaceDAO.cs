using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using Capstone.DAL;

namespace Capstone.DAL
{
    public interface ISpaceDAO
    {
        IList<Space> GetAllSpacesInVenue(int venueID);

        IList<Space> GetAvailableSpaces(int venueID, DateTime reservationStartDate, DateTime reservationEndDate, int numberOfGuests);


    }
}
