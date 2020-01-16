using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Capstone.Models;
using Capstone.DAL;

namespace Capstone
{

    public class UserInterface
    {

        public IReservationDAO reservationDAO;
        public ISpaceDAO spaceDAO;
        public IVenueDAO venueDAO;

        public SqlConnection Conn { get; set; }

        public UserInterface(string connectionString)
        {
            Conn = new SqlConnection(connectionString);
        }

        public void Run()
        {

            Conn.Open();

            reservationDAO = new ReservationSqlDAO(Conn);
            spaceDAO = new SpaceSqlDAO(Conn);
            venueDAO = new VenueSqlDao(Conn);

            string menuSelection = "";
            while (menuSelection != "Q")
            {
                DisplayMainMenu();
                menuSelection = Console.ReadLine().ToUpper();
                switch (menuSelection)
                {
                    case "1":
                        string venueListSelection = "";
                        while (venueListSelection != "R")
                        {
                            DisplayVenueList();
                            venueListSelection = Console.ReadLine().ToUpper();

                            if (venueListSelection != "R")
                            {
                                try
                                {
                                    int menuID = int.Parse(venueListSelection);

                                    PrintVenueDetails(menuID);
                                }
                                catch (System.FormatException)
                                {
                                    Console.WriteLine("Please enter a valid selection.");
                                }
                            }
                        }
                        break;
                    case "Q":
                        return;
                    default:
                        Console.WriteLine("Please enter a valid selection.");
                        break;
                }
            }

            Conn.Close();
        }

        public void DisplayMainMenu()
        {
            Console.WriteLine("Main Menu:");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1) List Venues");
            Console.WriteLine("Q) Quit");
        }

        public void DisplayVenueList()
        {
            Console.WriteLine();
            Console.WriteLine("Which venue would you like to view?");
            foreach (Venue venue in venueDAO.GetVenueList())
            {
                Console.WriteLine($"{venue.VenueId}) {venue.VenueName}");
            }
            Console.WriteLine("R) Return to Main Menu.");
        }

        public void PrintVenueDetails(int menuID)
        {
            Venue selectedVenue = venueDAO.GetVenueDetails(menuID);
            if (selectedVenue.VenueId > 0)
            {
                Console.WriteLine();
                Console.WriteLine(selectedVenue.VenueName);
                Console.WriteLine($"Location: {selectedVenue.VenueCityName}, {selectedVenue.VenueStateAbbreviation}");
                Console.Write($"Categories: ");
                Console.Write(string.Join(", ", selectedVenue.Categories.ToArray()));
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(selectedVenue.VenueDescription);
            }
            else
            {
                Console.WriteLine("Please choose a valid venue.");

                return;
            }

            VenueDetailMenu(menuID);
        }

        public void VenueDetailMenu(int menuID)
        {
            string venueDetailSelection = "";
            while (venueDetailSelection != "R")
            {

                Console.WriteLine();
                Console.WriteLine("What would you like to do next?");
                Console.WriteLine("1) View spaces");
                Console.WriteLine("2) Search for reservations");
                Console.WriteLine("R) Return to Venue List");
                venueDetailSelection = Console.ReadLine().ToUpper();

                switch (venueDetailSelection)
                {
                    case "1":
                        PrintSpaceList(menuID);
                        break;
                    case "2":
                        ReserveSpaceMenu(menuID);
                        break;
                    case "R":
                        return;
                    default:
                        Console.WriteLine("Please enter a valid selection.");
                        break;
                }
            }
        }

        public void PrintSpaceList(int menuID)
        {
            IList<Space> allSpaces = spaceDAO.GetAllSpacesInVenue(menuID);

            Console.WriteLine();
            Console.WriteLine("Space #".PadRight(10) + "Name".PadRight(27) + "Open".PadRight(10) + "Close".PadRight(10) + "Daily Rate".PadRight(15) +
                "Max Occupancy");
            Console.WriteLine("------------------------------------------------------------------------------------");

            foreach (Space space in allSpaces)
            {
                Console.WriteLine(space.SpaceListToString());
            }
            Console.WriteLine();

            string spaceListSelection = "";
            while (spaceListSelection != "R")
            {
                Console.WriteLine();
                Console.WriteLine("What would you like to do next?");
                Console.WriteLine("1) Reserve a space");
                Console.WriteLine("R) Return to Venue Detail Menu");
                spaceListSelection = Console.ReadLine().ToUpper();

                switch (spaceListSelection)
                {
                    case "1":
                        ReserveSpaceMenu(menuID);
                        break;
                    case "R":
                        return;
                    default:
                        Console.WriteLine("Please enter a valid selection.");
                        break;
                }
            }
        }

        public void ReserveSpaceMenu(int menuID)
        {
            Console.WriteLine();
            Console.Write("When do you need the space? (Use MM/DD/YYYY format) ");
            DateTime reservationStartDate = Convert.ToDateTime(Console.ReadLine());

            Console.Write("How many days will you need the space? ");
            int reservationDuration = Convert.ToInt32(Console.ReadLine());

            Console.Write("How many people will be in attendance? ");
            int numberOfGuests = Convert.ToInt32(Console.ReadLine());

            DateTime reservationEndDate = Reservation.CalculateReservationDepartureDate(reservationStartDate, reservationDuration);

            IList<Space> availableSpaces = spaceDAO.GetAvailableSpaces(menuID, reservationStartDate, reservationEndDate, numberOfGuests);

            if (availableSpaces.Count > 0)
            {

                Console.WriteLine();
                Console.WriteLine("The following spaces are available based on your needs:");
                Console.WriteLine();
                Console.WriteLine("Space #".PadRight(10) + "Name".PadRight(27) + "Daily Rate".PadRight(20) + "Max Occupancy".PadRight(15) + "Accessible?".PadRight(15) +
                   "Total Cost");
                Console.WriteLine("------------------------------------------------------------------------------------------");

                foreach (Space space in availableSpaces)
                {
                    Console.WriteLine($"{space.SpaceAvailabilityToString(reservationDuration)}");
                }
                Console.WriteLine();

                string selectedAvailableSpace = "";
                Console.WriteLine("Which space would you like to reserve?");
                Console.WriteLine("(Or enter 0 to cancel reservation)");
                selectedAvailableSpace = Console.ReadLine();
                Console.WriteLine();

                while (selectedAvailableSpace != "0")
                {
                    Console.WriteLine("Please enter a name for the reservation");
                    string reservationName = Console.ReadLine();

                    int spaceMenuID = int.Parse(selectedAvailableSpace);

                    if (reservationDAO.AddReservation(spaceMenuID, numberOfGuests, reservationStartDate, reservationEndDate, reservationName))
                    {
                        PrintReservationConfirmation(spaceMenuID, reservationName, numberOfGuests, reservationStartDate, reservationDuration);
                    }
                    else
                    {
                        Console.WriteLine("There was an error with your reservation submission. Please try again.");
                    }
                    return;
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("There are no available spaces, please try again.");
                Console.ReadLine();
            }
        }

        public void PrintReservationConfirmation(int spaceMenuID, string reservationName, int numberOfGuests, DateTime reservationStartDate, int reservationDuration)
        {
            Reservation newReservation = reservationDAO.GetReservationDetails(reservationName);

            Console.WriteLine();
            Console.WriteLine("Thanks for submitting your reservation! The details for your event are listed below:");
            Console.WriteLine();
            Console.WriteLine("Confirmation #: " + newReservation.ReservationID);
            Console.WriteLine("Venue: " + newReservation.VenueName);
            Console.WriteLine("Space: " + newReservation.SpaceName);
            Console.WriteLine("Reserved for: " + reservationName);
            Console.WriteLine("Attendees: " + numberOfGuests);
            Console.WriteLine("Arrival Date: " + reservationStartDate.ToString("d"));
            Console.WriteLine("Departure Date: " + newReservation.EndDate.ToString("d"));
            Console.WriteLine("Total Cost: " + (newReservation.CostPerDay * reservationDuration).ToString("C"));
        }
    }
}
