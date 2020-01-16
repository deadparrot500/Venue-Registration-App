using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using Capstone.DAL;

namespace Capstone.Models
{
    public class Space
    {
        public int ID { get; set; }
        public int VenueID { get; set; }
        public string Name { get; set; }
        public bool IsAccessible { get; set; }
        public int OpenFrom { get; set; }
        public int OpenTo { get; set; }
        public decimal DailyRate { get; set; }
        public int MaxOccupancy { get; set; }

        public string SpaceListToString()
        {
            string result = ID.ToString().PadRight(10) + Name.PadRight(27) + MonthNumberToName(OpenFrom).PadRight(10) + MonthNumberToName(OpenTo).PadRight(10) +
                DailyRate.ToString("C").PadRight(15) + MaxOccupancy.ToString();
            return result;
        }

        public string SpaceAvailabilityToString(int reservationDuration)
        {
            string result = ID.ToString().PadRight(10) + Name.PadRight(27) + DailyRate.ToString("C").PadRight(20) + MaxOccupancy.ToString().PadRight(15) +
                IsAccessible.ToString().PadRight(15) + (DailyRate * reservationDuration).ToString("C");
            return result;
        }      

        public string MonthNumberToName(int month)
        {
            string monthName = "";
            switch (month)
            {
                case 1:
                    monthName = "January";
                    break;
                case 2:
                    monthName = "February";
                    break;
                case 3:
                    monthName = "March";
                    break;
                case 4:
                    monthName = "April";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "June";
                    break;
                case 7:
                    monthName = "July";
                    break;
                case 8:
                    monthName = "August";
                    break;
                case 9:
                    monthName = "September";
                    break;
                case 10:
                    monthName = "October";
                    break;
                case 11:
                    monthName = "November";
                    break;
                case 12:
                    monthName = "December";
                    break;
            }
            return monthName;
        }
    }
}
