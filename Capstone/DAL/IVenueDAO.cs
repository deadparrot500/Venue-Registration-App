using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;

namespace Capstone.DAL
{
    public interface IVenueDAO
    {
        List<Venue> GetVenueList();

        Venue GetVenueDetails(int menuID);


    }
}
