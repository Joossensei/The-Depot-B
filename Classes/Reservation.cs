using System;
using Newtonsoft.Json;
using System.Globalization;

namespace ReservationSystem
{
    public static class Reservation
    {
        public static void tourRes(string tickets){
            jsonManager manager = new jsonManager();
            List<string> entryTickets = jsonManager.LoadEntryTickets();

            

            // entryTickets.Add("12");
            if(entryTickets.Contains(tickets)){
                Console.WriteLine("Uw ticket is geldig");
                // if(tours.bookings.Contains(line)){
                //     Console.WriteLine("U heeft een boeking");
                //     // Console.WriteLine($"Uw rondleiding begint om {tours.dateTime}"); 
                // }else{
                //     Console.WriteLine("U heeft nog geen boeking staan");
                // }
            }
            else{
                Console.WriteLine("Uw ticket heeft geen recht op een rondleiding");
            }

            
        }
    }
}