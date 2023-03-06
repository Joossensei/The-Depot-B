using Newtonsoft.Json;
using System.Globalization;

namespace ReservationSystem
{
    class Program
    {
        // Comment die erg belangrijk is
        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                // Display main menu
                Console.WriteLine("Welcome to The Depot Reservation System");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Make a reservation");
                Console.WriteLine("2. Cancel a reservation");
                Console.WriteLine("3. View reservations");
                Console.WriteLine("4. Login as manager");
                Console.WriteLine("5. Exit");

                // Get user input
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine("---------------------------------------------------");
                        Console.WriteLine("\n");
                        Console.WriteLine("First clicked");
                        Console.WriteLine("\n");
                        Console.WriteLine("---------------------------------------------------");
                        break;
                    case "2":
                        Console.WriteLine("---------------------------------------------------");
                        Console.WriteLine("\n");
                        Console.WriteLine("Second clicked");
                        Console.WriteLine("\n");
                        Console.WriteLine("---------------------------------------------------");
                        break;
                    case "3":
                        Console.WriteLine("---------------------------------------------------");
                        Console.WriteLine("\n");
                        Console.WriteLine("Third clicked");
                        Console.WriteLine("\n");
                        Console.WriteLine("---------------------------------------------------");
                        break;
                    case "4":
                        Console.WriteLine("---------------------------------------------------");
                        Console.WriteLine("\n");
                        Console.WriteLine("Fourth clicked");
                        Console.WriteLine("\n");
                        Console.WriteLine("---------------------------------------------------");
                        break;
                    case "5":
                        Console.WriteLine("---------------------------------------------------");
                        Console.WriteLine("\n");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("---------------------------------------------------");
                        Console.WriteLine("\n");
                        Console.WriteLine("Invalid input");
                        Console.WriteLine("\n");
                        Console.WriteLine("---------------------------------------------------");
                        break;
                }
            }
        }
    }
}
// Raihan test pull