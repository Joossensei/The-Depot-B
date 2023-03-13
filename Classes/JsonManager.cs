using System;
using Newtonsoft.Json;
namespace ReservationSystem;

    class jsonManager
    
    {
        public static List<string> LoadEntryTickets()
        {
            try 
            {
                using(StreamReader reader = new StreamReader(@"JsonFiles\entryTickets.json"))
                {
                    string json = reader.ReadToEnd();
                    List<string> entryTickets = JsonConvert.DeserializeObject<List<string>>(json);
                    foreach (string Entryt in entryTickets)
                    {
                        System.Console.WriteLine(Entryt);
                    }
                    Console.WriteLine(entryTickets);
                    return entryTickets ?? new List<string>();
                }

            }
            catch (FileNotFoundException)
            {
                System.Console.WriteLine("file 'entryTickets.json' not found.\nPut the file in JsonFiles directory");
                return new List<string>();
            }
        }

        public static List<Tour> LoadTours()
        {
            try
            {
                using (StreamReader reader = new StreamReader(@"JsonFiles\tours.json"))
                {
                    System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");
                    string json = reader.ReadToEnd();
                    List<Tour> Tour = JsonConvert.DeserializeObject<List<Tour>>(json);
                    return Tour ?? new List<Tour>();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No reservations found");
                return new List<Tour>();
            }
        }
    }    

    