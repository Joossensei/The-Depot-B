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

        /*public static List<string> ReadEntryTickets()
        {

        }*/
    }    