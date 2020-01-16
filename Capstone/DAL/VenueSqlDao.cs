using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class VenueSqlDao : IVenueDAO
    {
        private SqlConnection Conn { get; set; }
        private string sql_GetVenues = "SELECT * FROM venue;";
        private string sql_GetVenueDetails = @"SELECT venue.id AS id, venue.name AS venueName, city.name AS cityName, state.abbreviation as stateAbbreviation, category.name AS category, description FROM venue " +
                                            "JOIN city ON venue.city_id = city.id JOIN state ON city.state_abbreviation = state.abbreviation " +
                                            "LEFT JOIN category_venue ON venue.id = category_venue.venue_id LEFT JOIN category ON category.id = category_venue.category_id " +
                                            "WHERE venue.id = @id ";

        public VenueSqlDao(SqlConnection Conn)
        {
            this.Conn = Conn;
        }

        public List<Venue> GetVenueList()
        {
            //Returns a list of all venues

            List<Venue> venueList = new List<Venue>();



            using (SqlCommand cmd = new SqlCommand(sql_GetVenues, Conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Venue newVenue = new Venue();

                        newVenue.VenueId = Convert.ToInt32(reader["id"]);
                        newVenue.VenueName = Convert.ToString(reader["name"]);
                        newVenue.VenueDescription = Convert.ToString(reader["description"]);

                        venueList.Add(newVenue);
                    }
                }

                return venueList;
            }



        }

        
        public Venue GetVenueDetails(int menuID)
        {
            Venue newVenue = new Venue();

            using (SqlCommand cmd = new SqlCommand(sql_GetVenueDetails, Conn))
            {
                cmd.Parameters.AddWithValue("@id", menuID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                     
                            newVenue.VenueId = Convert.ToInt32(reader["id"]);
                            newVenue.VenueName = Convert.ToString(reader["venueName"]);
                            newVenue.VenueCityName = Convert.ToString(reader["cityName"]);
                            newVenue.VenueStateAbbreviation = Convert.ToString(reader["stateAbbreviation"]);
                            newVenue.Categories.Add(Convert.ToString(reader["category"]));
                            newVenue.VenueDescription = Convert.ToString(reader["description"]);
                        
                    }
                }

                return newVenue;
            }
        }
    }
}

