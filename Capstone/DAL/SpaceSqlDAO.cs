using System;
using System.Collections.Generic;
using System.Text;
using Capstone.Models;
using Capstone.DAL;
using System.Data.SqlClient;


namespace Capstone.DAL
{
    public class SpaceSqlDAO : ISpaceDAO
    {
        private SqlConnection Conn { get; set; }   

        private string sql_GetSpaceAvailability = $"SELECT TOP 5 * FROM space s WHERE venue_id = @venueID AND s.max_occupancy >= @numberOfGuests " +
            $"AND (open_from <= @startMonth OR open_from IS NULL) AND (open_to >= @endMonth OR open_to IS NULL) AND s.id NOT IN (SELECT s.id from reservation r JOIN space s on r.space_id = s.id " +
            $"WHERE s.venue_id = @venueID AND r.end_date >= @reservationStartDate AND r.start_date <= @reservationEndDate)";

        public SpaceSqlDAO(SqlConnection Conn)
        {
            this.Conn = Conn;
        }

        public IList<Space> GetAllSpacesInVenue(int venueID)
        {
            IList<Space> allSpacesInVenue = new List<Space>();

            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM space WHERE venue_ID = @venueID;", Conn))
            {
                cmd.Parameters.AddWithValue("@venueID", venueID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Space newSpace = new Space();

                        newSpace.ID = Convert.ToInt32(reader["id"]);
                        newSpace.VenueID = Convert.ToInt32(reader["venue_id"]);
                        newSpace.Name = Convert.ToString(reader["name"]);
                        newSpace.IsAccessible = Convert.ToBoolean(reader["is_accessible"]);
                        if (reader["open_from"] is DBNull)
                        {
                            newSpace.OpenFrom = 1;
                        }
                        else
                        {
                            newSpace.OpenFrom = Convert.ToInt32(reader["open_from"]);
                        }
                        if (reader["open_to"] is DBNull)
                        {
                            newSpace.OpenTo = 12;
                        }
                        else
                        {
                            newSpace.OpenTo = Convert.ToInt32(reader["open_to"]);
                        }
                        newSpace.DailyRate = Convert.ToDecimal(reader["daily_rate"]);
                        newSpace.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);

                        allSpacesInVenue.Add(newSpace);
                    }
                }
            }
            return allSpacesInVenue;
        }

        public IList<Space> GetAvailableSpaces(int venueID, DateTime reservationStartDate, DateTime reservationEndDate, int numberOfGuests)
        {
            IList<Space> availableSpaces = new List<Space>();                     

            using (SqlCommand cmd = new SqlCommand(sql_GetSpaceAvailability, Conn))
            {
                cmd.Parameters.AddWithValue("@venueID", venueID);
                cmd.Parameters.AddWithValue("@reservationStartDate", reservationStartDate);
                cmd.Parameters.AddWithValue("@reservationEndDate", reservationEndDate);
                cmd.Parameters.AddWithValue("@numberOfGuests", numberOfGuests);
                cmd.Parameters.AddWithValue("@startMonth", reservationStartDate.Month);
                cmd.Parameters.AddWithValue("@endMonth", reservationEndDate.Month);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Space newSpace = new Space();

                        newSpace.ID = Convert.ToInt32(reader["id"]);
                        newSpace.VenueID = Convert.ToInt32(reader["venue_id"]);
                        newSpace.Name = Convert.ToString(reader["name"]);
                        newSpace.IsAccessible = Convert.ToBoolean(reader["is_accessible"]);
                        if (reader["open_from"] is DBNull)
                        {
                            newSpace.OpenFrom = 1;
                        }
                        else
                        {
                            newSpace.OpenFrom = Convert.ToInt32(reader["open_from"]);
                        }
                        if (reader["open_to"] is DBNull)
                        {
                            newSpace.OpenTo = 12;
                        }
                        else
                        {
                            newSpace.OpenTo = Convert.ToInt32(reader["open_to"]);
                        }
                        newSpace.DailyRate = Convert.ToDecimal(reader["daily_rate"]);
                        newSpace.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);

                        availableSpaces.Add(newSpace);
                    }
                }
            }
            return availableSpaces;
        }       
    }
}
