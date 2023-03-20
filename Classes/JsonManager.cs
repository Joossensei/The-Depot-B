using System;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ReservationSystem;
    public class jsonManager
    
    {
        public static List<string> LoadEntryTickets()
        {
            try 
            {
                using(StreamReader reader = new StreamReader(@"JsonFiles/entryTickets.json"))
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
            catch (DirectoryNotFoundException)
            {
                try 
                {
                    using(StreamReader reader = new StreamReader(@"../../../JsonFiles/entryTickets.json"))
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
                    string json = reader.ReadToEnd();
                    List<Tour> tours = JsonConvert.DeserializeObject<List<Tour>>(json);
                    return tours ?? new List<Tour>();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No reservations found");
                return new List<Tour>();
            }
        }
        public void writeToJson(List<Tour> tours, string fileName)
        {
            string json = JsonConvert.SerializeObject(tours);
            File.WriteAllText(fileName, json);
        }
    }    

