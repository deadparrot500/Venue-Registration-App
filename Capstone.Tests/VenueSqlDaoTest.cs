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
    public class VenueSqlDaoTest
    {


        public SqlConnection Conn { get; set; }

        private TransactionScope transaction;

        public VenueSqlDaoTest()
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
        public void GetVenueList_Returns_List()
        {
            bool result = false;
            IVenueDAO test = new VenueSqlDao(Conn);
            int testID = GenerateTestVenue(Conn);

            List<Venue> testList = new List<Venue>(test.GetVenueList());

            foreach (Venue venue in testList)
            {
                if (venue.VenueId == testID)
                {
                    result = true;
                }
            }

            Assert.AreEqual(true, result);

        }

        [TestMethod]
        public void GetVenueDetails_Returns_Data()
        {
            //Returns a list of the venue's details
            //[0] = the venue name
            //[1] = the city name 
            //[2] = the state abbrevtiation
            //[3] = category
            //[4] = the description

            IVenueDAO test = new VenueSqlDao(Conn);

            int testID = JoinTestCategoryVenue(Conn);
            //List<string> testList = new List<string>();
            //test.GetVenueDetails(testID);

            Assert.AreEqual("Test Venue", test.GetVenueDetails(testID).VenueName);
            Assert.AreEqual("Test City", test.GetVenueDetails(testID).VenueCityName);
            Assert.AreEqual("XX", test.GetVenueDetails(testID).VenueStateAbbreviation);
            //Assert.AreEqual("Test Category", test.GetVenueDetails(testID).Categories);
            Assert.AreEqual("Programmers have too much fun writing things like this.", test.GetVenueDetails(testID).VenueDescription);

        }




        public string GenerateTestState(SqlConnection Conn)
        {
            string statement = "INSERT INTO state (name, abbreviation)" +
                "VALUES ('Test State', 'XX' );" +
                "SELECT abbreviation FROM state WHERE abbreviation = 'XX'";
            string newstateID;

            using (SqlCommand cmd = new SqlCommand(statement, Conn))
            {

                newstateID = Convert.ToString(cmd.ExecuteScalar());

            }
            return newstateID;
        }


        public int GenerateTestCity(SqlConnection Conn)
        {
            int newCityID;
            string teststateID = GenerateTestState(Conn);
            string statement = @"INSERT INTO city (name, state_abbreviation) " +
                "VALUES ('Test City', @state_abbreviation );" +
                "SELECT id FROM city WHERE name = 'Test City'";

            using (SqlCommand cmd = new SqlCommand(statement, Conn))
            {
                cmd.Parameters.AddWithValue("@state_abbreviation", teststateID);
                newCityID = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return newCityID;

        }



        public int GenerateTestVenue(SqlConnection Conn)
        {

            string statement = @"INSERT INTO venue (name, city_id, description) " +
                 "VALUES ('Test Venue', @city_id,'Programmers have too much fun writing things like this.' );" +
                 "SELECT venue.id FROM venue WHERE venue.name = 'Test Venue'";
            int newVenueID;
            int testCityID = GenerateTestCity(Conn);

            using (SqlCommand cmd = new SqlCommand(statement, Conn))
            {
                cmd.Parameters.AddWithValue("@city_id", testCityID);

                newVenueID = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return newVenueID;
        }

        public int GenerateTestCategory(SqlConnection Conn)
        {
            string statement = "INSERT INTO category (name)" +
                "VALUES ('Test Category' )" +
                "SELECT id FROM category WHERE name = 'Test Category'";
            int newCategoryID;

            using (SqlCommand cmd = new SqlCommand(statement, Conn))
            {
                newCategoryID = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return newCategoryID;
        }
        public int JoinTestCategoryVenue(SqlConnection Conn)
        {
            string statement = @"INSERT INTO category_venue (venue_id, category_id)" +
                 "VALUES (@venue_id, @category_id );";
            int venueID = GenerateTestVenue(Conn);

            int categoryID = GenerateTestCategory(Conn);


            using (SqlCommand cmd = new SqlCommand(statement, Conn))
            {
                cmd.Parameters.AddWithValue("@venue_id", venueID);
                cmd.Parameters.AddWithValue("@category_id", categoryID);

                cmd.ExecuteNonQuery();
            }
            return venueID;
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












