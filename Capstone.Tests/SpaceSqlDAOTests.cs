using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Capstone.Models;
using Capstone.DAL;
using System.Transactions;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Capstone.Tests
{
    [TestClass]
    public class SpaceSqlDAOTests
    {
        private TransactionScope transaction;

        public SqlConnection Conn { get; set; }

        public SpaceSqlDAOTests()
        {
            Conn = GenerateTestConnection();
        }

        [TestInitialize]
        public void Setup()
        {         
            transaction = new TransactionScope();
            Conn.Open();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Conn.Close();
            transaction.Dispose();      
        }

        [TestMethod]
        public void GetAllSpacesInVenue_Returns_Non_Empty_List()
        {
            SpaceSqlDAO dao = new SpaceSqlDAO(Conn);

            GenerateTestSpace(Conn);

            Assert.IsTrue(dao.GetAllSpacesInVenue(1).Count() > 0);
        }

        [TestMethod]
        public void GetAvailableSpaces_Returns_Non_Empty_List()
        {
            SpaceSqlDAO dao = new SpaceSqlDAO(Conn);

            GenerateTestSpace(Conn);

            Assert.IsTrue(dao.GetAvailableSpaces(1, Convert.ToDateTime("01/01/2019"), Convert.ToDateTime("01/05/2019"), 1).Count() > 0);
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

        public void GenerateTestSpace(SqlConnection Conn)
        {
            string statement = "INSERT INTO space (venue_id, name, is_accessible, open_from, open_to, daily_rate, max_occupancy) " +
                "VALUES (1, 'Test Space', 1, 1, 12, 1.00, 1);";

            using (SqlCommand cmd = new SqlCommand(statement, Conn))
            {
                cmd.ExecuteNonQuery();
            }
        } 




    }
}
