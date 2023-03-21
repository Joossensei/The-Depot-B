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

            if(entryTickets.Contains(tickets)){
                Console.WriteLine("Uw ticket is geldig");
<<<<<<< Updated upstream
                //jsonmaniger . get tours
                //for loop
                //if currant tour . booking contains line
                //als dat zo is voeg tour curront tour toe aan tour currusertour
                //
                // if(bookings.Contains(line)){
=======
                // if(tours.bookings.Contains(tickets)){
>>>>>>> Stashed changes
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