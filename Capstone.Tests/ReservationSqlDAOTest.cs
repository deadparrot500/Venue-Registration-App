using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Transactions;
using Capstone.Models;
using Capstone.DAL;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Capstone.Tests
{
    [TestClass]
    public class ReservationSqlDAOTest
    {
        public SqlConnection Conn { get; set; }

        private TransactionScope transaction;

        VenueSqlDaoTest venue = new VenueSqlDaoTest();

        public ReservationSqlDAOTest()
        {
            Conn = GenerateTestConnection();
        }

        [TestInitialize]
        public void Setup()
        {
            // Begin the transaction

            transaction = new TransactionScope();
            Conn.Open();

        }

        [TestCleanup]
        public void Cleanup()
        {
            Conn.Close();
            // Roll back the transaction
            transaction.Dispose();

        }

        [TestMethod]
        public void AddReservation_adds_a_Line_to_table()
        {
            int before = GetRowCount("reservation", Conn);
            IReservationDAO test = new ReservationSqlDAO(Conn);
            Reservation testReservation = new Reservation() ;

            testReservation = GenerateTestReservationModel(Conn);

            test.AddReservation(testReservation.SpaceID,testReservation.NumberOfAttendees,testReservation.StartDate, testReservation.EndDate, testReservation.ReservedFor);
            int after = GetRowCount("reservation", Conn);
            Assert.AreEqual(before + 1, after);

        }

        [TestMethod]
        public void GetReservationDetails_returns_data()
        {
            IReservationDAO test = new ReservationSqlDAO(Conn);
            int testSpaceID = GenerateTestReservation(Conn);
            Reservation testReservation = new Reservation();
            testReservation = test.GetReservationDetails("Test Reservation");

            Assert.AreEqual("Test Space", testReservation.SpaceName);
            Assert.AreEqual("Test Reservation", testReservation.ReservedFor);
            Assert.AreEqual(50, testReservation.NumberOfAttendees);
            Assert.AreEqual("Test Venue", testReservation.VenueName);


        }

        public Reservation GenerateTestReservationModel(SqlConnection Conn)
        {
            Reservation testReservation = new Reservation();

            testReservation.SpaceID = GenerateTestSpace(Conn);
            testReservation.NumberOfAttendees = 50;
            testReservation.StartDate = Convert.ToDateTime("2019/02/01");
            testReservation.EndDate = Convert.ToDateTime("2019/02/10");
            testReservation.ReservedFor = "Test Reservation";
          

            return testReservation;
        }

        public int GenerateTestSpace(SqlConnection Conn)
        {
            int testVenueID = venue.JoinTestCategoryVenue(Conn);

            string spaceStatement = "INSERT INTO space ( venue_id, name, is_accessible, open_from, open_to, daily_rate, max_occupancy) " +
                "VALUES ( @venueID, 'Test Space', 1, NULL, NULL, '1000', 100); " +
                "SELECT id FROM space WHERE name = 'Test Space'";

            int testSpaceID;
            using (SqlCommand cmd = new SqlCommand(spaceStatement, Conn))
            {
                cmd.Parameters.AddWithValue("@venueID", testVenueID);

                testSpaceID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return testSpaceID;
        }


        public int GenerateTestReservation(SqlConnection Conn)
        {
            int testVenueID = venue.JoinTestCategoryVenue(Conn);

            string spaceStatement = "INSERT INTO space ( venue_id, name, is_accessible, open_from, open_to, daily_rate, max_occupancy) " +
                "VALUES ( @venueID, 'Test Space', 1, NULL, NULL, '1000', 100); " +
                "SELECT id FROM space WHERE name = 'Test Space'";

            int testSpaceID;

            string statement = @"INSERT INTO reservation (space_id, number_of_attendees, start_date, end_date, reserved_for) " +
            "VALUES ( @spaceID, 50, GETDATE()+2, GETDATE()+6,  'Test Reservation');";

            using (SqlCommand cmd = new SqlCommand(spaceStatement, Conn))
            {
                cmd.Parameters.AddWithValue("@venueID", testVenueID);

                testSpaceID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            using (SqlCommand cmd = new SqlCommand(statement, Conn))
            {
                cmd.Parameters.AddWithValue("@spaceID", testSpaceID);

                cmd.ExecuteNonQuery();
            }
            return testSpaceID;
        }


        public int GetRowCount(string table, SqlConnection Conn)
        {

            SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM {table}", Conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count;

        }

        public SqlConnection GenerateTestConnection()  //copied from program- creates a new connection, connection opens in initialize, closes in cleanup
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("Project");

            SqlConnection conn = new SqlConnection(connectionString);
            return conn;
        }
    }
}
