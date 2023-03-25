using System;
using Newtonsoft.Json;
using System.Globalization;

namespace ReservationSystem
{
    public static class Reservation
    {
        public static List<Action> tourRes(string tickets){
            jsonManager manager = new jsonManager();
            List<string> entryTickets = jsonManager.LoadEntryTickets();
            List<Tour> alltours = jsonManager.LoadTours();
            // var text = "";

        List<Action> probeer = new List<Action>();

            if(entryTickets.Contains(tickets)){
                probeer.Add(new (){text = "Uw ticket is geldig"});

                foreach (var checkTour in alltours)
                {
                    if(checkTour.bookings.Any()){

                    foreach (var reservation in checkTour.bookings)
                    {
                        if(reservation.userId == tickets){
                            probeer.Add(new (){text = "De Rondleidin die u heeft geboekt: \n"});
                            probeer.Add(new (){text = checkTour.dateTime.ToString()});
                            probeer.Add(new (){text = checkTour.tourDuration.ToString(),hasExtraBreak = true});
                        }
                        
                    }
                    }else{
                        // Console.WriteLine("U heeft geen rondleiding geboekt");
                        probeer.Add(new (){text = "U heeft geen rondleiding geboekt", hasExtraBreak = true});
                        break;
                    }
                    
                }
                return probeer;

                // if(tours.bookings.Contains(tickets)){
                //     Console.WriteLine("U heeft een boeking");
                //     // Console.WriteLine($"Uw rondleiding begint om {tours.dateTime}"); 
                // }else{
                //     Console.WriteLine("U heeft nog geen boeking staan");
                // }
            }
            else{
                // Console.WriteLine("Uw ticket heeft geen recht op een rondleiding");
                probeer.Add(new (){text = "Uw ticket heeft geen recht op een rondleiding", hasExtraBreak = true});
                return probeer;
            }
            }
            
        }
        }
        
