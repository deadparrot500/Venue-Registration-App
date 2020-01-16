using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationSqlDAO : IReservationDAO
    {
        public SqlConnection Conn { get; private set; }
        private string sql_AddReservation = @"INSERT INTO reservation (space_id, number_of_attendees, start_date, end_date, reserved_for) " +
            "VALUES ( @spaceID, @numberOfGuests, @startDate, @endDate, @reservationName);";

        private string sql_GetReservationDetails = @"Select r.reservation_id AS rID, v.name AS vName, s.name AS sName, " +
            "r.reserved_for AS reservedFor, r.number_of_attendees AS guests, r.start_date AS startDate, r.end_date AS endDate, s.daily_rate AS cost " +
            "FROM reservation AS r JOIN space AS s ON s.id = r.space_id JOIN venue AS v ON v.id = s.venue_id WHERE reserved_for = @reservationName";
        
        public ReservationSqlDAO(SqlConnection Conn)
        {
            this.Conn = Conn;
        }
        
        public bool AddReservation(int spaceID, int numberOfGuests, DateTime startDate, DateTime endDate, string reservationName)
        {
            bool result = false;
            
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql_AddReservation, Conn))
                {
                    cmd.Parameters.AddWithValue("@spaceID", spaceID);
                    cmd.Parameters.AddWithValue("@numberOfGuests", numberOfGuests);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@reservationName", reservationName);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        result = true;

                    }
                    
                }
 
            }
            catch
            {
                result = false;
            }

            return result;

        }


        public Reservation GetReservationDetails(string reservationName)
        {
            Reservation reservation = new Reservation();
            reservation.ReservedFor = reservationName;

            using (SqlCommand cmd = new SqlCommand(sql_GetReservationDetails, Conn))
            {
                cmd.Parameters.AddWithValue("@reservationName", reservationName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservation.ReservationID = Convert.ToInt32(reader["rID"]);
                        reservation.VenueName = Convert.ToString(reader["vName"]);
                        reservation.SpaceName = Convert.ToString(reader["sName"]);
                        reservation.ReservedFor = Convert.ToString(reader["reservedFor"]);
                        reservation.NumberOfAttendees = Convert.ToInt32(reader["guests"]);
                        reservation.StartDate = Convert.ToDateTime(reader["startDate"]);
                        reservation.EndDate = Convert.ToDateTime(reader["endDate"]);
                        reservation.CostPerDay = Convert.ToDecimal(reader["cost"]);                     
                    }
                }

            }

            return reservation;
        }


    }

}



