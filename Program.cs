using Newtonsoft.Json;
using System.Globalization;

namespace ReservationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load reservations from JSON file
            List<Reservation> reservations = LoadReservations();

            // Load entry tickets from JSON file
            List<string> entryTickets = LoadEntryTickets();

            // Load manager password from JSON file
            string managerPassword = LoadManagerPassword();

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
                        MakeReservation(reservations, entryTickets);
                        break;
                    case "2":
                        CancelReservation(reservations);
                        break;
                    case "3":
                        ViewReservations(reservations);
                        break;
                    case "4":
                        if (LoginAsManager(managerPassword))
                        {
                            ManagerMenu(reservations, entryTickets);
                        }
                        else
                        {
                            Console.WriteLine("Incorrect password");
                        }
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }
            }

            // Save reservations to JSON file
            SaveReservations(reservations);
        }

        static List<Reservation> LoadReservations()
        {
            try
            {
                using (StreamReader reader = new StreamReader("reservations.json"))
                {
                    string json = reader.ReadToEnd();
                    List<Reservation> reservations = JsonConvert.DeserializeObject<List<Reservation>>(json);
                    return reservations ?? new List<Reservation>();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No reservations found");
                return new List<Reservation>();
            }
        }

        static void SaveReservations(List<Reservation> reservations)
        {
            using (StreamWriter writer = new StreamWriter("reservations.json"))
            {
                string json = JsonConvert.SerializeObject(reservations);
                writer.Write(json);
            }
        }

        static List<string> LoadEntryTickets()
        {
            try
            {
                using (StreamReader reader = new StreamReader("entryTickets.json"))
                {
                    string json = reader.ReadToEnd();
                    List<string> entryTickets = JsonConvert.DeserializeObject<List<string>>(json);
                    return entryTickets ?? new List<string>();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No entry tickets found");
                return new List<string>();
            }
        }

        static string LoadManagerPassword()
        {
            try
            {
                using (StreamReader reader = new StreamReader("managerPassword.json"))
                {
                    string json = reader.ReadToEnd();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    return data.password;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No manager password found");
                return "";
            }
        }

        static void MakeReservation(List<Reservation> reservations, List<string> entryTickets)
        {
            Console.WriteLine("Please enter your entry ticket code:");
            string entryTicket = Console.ReadLine();

            if (!entryTickets.Contains(entryTicket))
            {
                Console.WriteLine("Invalid entry ticket code");
                return;
            }

            Console.WriteLine("Please enter the number of people in your group:");
            int groupSize = int.Parse(Console.ReadLine());

            // Ensure that every member of the group has a valid ticket
            for (int i = 2; i <= groupSize; i++)
            {
                Console.WriteLine($"Please enter the entry ticket code for person {i} in your group:");
                string personEntryTicket = Console.ReadLine();

                if (!entryTickets.Contains(personEntryTicket))
                {
                    Console.WriteLine($"Invalid entry ticket code for person {i}");
                    return;
                }
            }

            Console.WriteLine("Please enter the reservation time (in format HH:mm):");
            string timeString = Console.ReadLine();
            if (!DateTime.TryParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime reservationTime))
            {
                Console.WriteLine("Invalid time format. Please use format HH:mm");
                return;
            }

            // Check if reservation time is within opening hours (11:00 AM to 5:30 PM)
            DateTime openingTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0);
            DateTime closingTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0);
            if (reservationTime < openingTime || reservationTime > closingTime)
            {
                Console.WriteLine("Sorry, the museum is closed at that time.");
                return;
            }

            // Check if reservation time is compatible with tour schedule
            TimeSpan timeDiff = reservationTime - openingTime;
            int tourNum = (int)timeDiff.TotalMinutes / 20; // Determine which tour the reservation corresponds to
            TimeSpan tourStartTime = TimeSpan.FromMinutes(tourNum * 20); // Calculate the start time of the tour
            DateTime tourTime = openingTime + tourStartTime;
            if (reservationTime != tourTime)
            {
                Console.WriteLine("Sorry, reservations are only available at :00, :20, and :40 past the hour.");
                return;
            }

            Reservation newReservation = new Reservation(entryTicket, groupSize, reservationTime);
            reservations.Add(newReservation);
            Console.WriteLine($"Reservation created. Your reservation code is {newReservation.Code}");
        }
        static void CancelReservation(List<Reservation> reservations)
        {
        Console.WriteLine("Please enter your reservation code:");
        string code = Console.ReadLine();

        Reservation reservation = reservations.FirstOrDefault(r => r.Code == code);

        if (reservation == null)
        {
            Console.WriteLine("Reservation not found");
            return;
        }

        reservations.Remove(reservation);
        Console.WriteLine("Reservation cancelled");
        }
        // viewreservation v2
        static void ViewReservations(List<Reservation> reservations)
        {
            Console.Write("Enter your reservation code: ");
            string reservationCode = Console.ReadLine();

            Reservation reservation = reservations.FirstOrDefault(r => r.Code == reservationCode);

            if (reservation == null)
            {
                Console.WriteLine($"No reservation found with code '{reservationCode}'");
                return;
            }

            Console.WriteLine("Reservation Details:");
            Console.WriteLine($"Reservation Code: {reservation.Code}");
            Console.WriteLine($"Entry Ticket Code: {reservation.EntryTicket}");
            Console.WriteLine($"Group Size: {reservation.GroupSize}");
            Console.WriteLine($"Reservation Time: {reservation.ReservationTime.ToString("HH:mm")}");
        }
        // viewreservation v1 manager
        static void ViewReservationsmanager(List<Reservation> reservations)
        {
        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations found");
            return;
        }

        Console.WriteLine("Reservations:");
        Console.WriteLine("Reservation Code   Entry Ticket Code                  Group Size   Reservation Time");

        foreach (Reservation reservation in reservations)
        {
            /*string entryTicketCode = reservation.EntryTicket == null ? "N/A" : reservation.EntryTicket.ToString();
            string reservationTime = reservation.ReservationTime.ToString("h:mm tt");
            string reservationDetails = String.Format("{0,-18} {1,-36} {2,-12} {3,-19}",
                                               reservation.Code, entryTicketCode, reservation.GroupSize, reservationTime);*/
            Console.WriteLine($"{reservation.Code,-18}{reservation.EntryTicket,-42}{reservation.GroupSize,-12}{reservation.ReservationTime.ToString("HH:mm"),-19}");
        }
        }

        static bool LoginAsManager(string managerPassword)
        {
            Console.WriteLine("Please enter the manager password:");
            string password = Console.ReadLine();

            return password == managerPassword;
        }

        static void ManagerMenu(List<Reservation> reservations, List<string> entryTickets)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("Manager menu");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. View entry tickets");
                Console.WriteLine("2. Generate new entry tickets");
                Console.WriteLine("3. View reservations");
                Console.WriteLine("4. Exit");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ViewEntryTickets(entryTickets);
                        break;
                    case "2":
                        GenerateEntryTickets(entryTickets);
                        break;
                    case "3":
                        ViewReservationsmanager(reservations);                  
                        break; 
                    case "4":
                        running = false;
                        break;
                   
                    default:
                        Console.WriteLine("Invalid input");
                        break;
                }
            }

            // Save entry tickets to JSON file
            SaveEntryTickets(entryTickets);
        }

        static void ViewEntryTickets(List<string> entryTickets)
        {
            Console.WriteLine("Entry tickets:");

            foreach (string entryTicket in entryTickets)
            {
                Console.WriteLine(entryTicket);
            }
        }

        static void GenerateEntryTickets(List<string> entryTickets)
        {
            Console.WriteLine("Please enter the number of entry tickets to generate:");
            int numTickets = int.Parse(Console.ReadLine());

            for (int i = 0; i < numTickets; i++)
            {
                string entryTicket = Guid.NewGuid().ToString();
                entryTickets.Add(entryTicket);
                Console.WriteLine(entryTicket);
            }
        }

        static void SaveEntryTickets(List<string> entryTickets)
        {
            using (StreamWriter writer = new StreamWriter("entryTickets.json"))
            {
                string json = JsonConvert.SerializeObject(entryTickets);
                writer.Write(json);
            }
        }
    

    
        
        public class Reservation
        {
            public string Code { get; set; }
            public string EntryTicket { get; set; }
            public int GroupSize { get; set; }
            public DateTime ReservationTime { get; set; }

            public Reservation(string entryTicket, int groupSize, DateTime reservationTime)
            {
                Code = GenerateCode();
                EntryTicket = entryTicket;
                GroupSize = groupSize;
                ReservationTime = reservationTime;
            }

            public string GenerateCode()
            {
                return Guid.NewGuid().ToString("N").Substring(0, 8);
            }

            public override string ToString()
            {
                return $"Reservation code: {Code}\nEntry ticket code: {EntryTicket}\nGroup size: {GroupSize}\nReservation time: {ReservationTime.ToString("HH:mm")}\n";
            }
        }
    }
}
