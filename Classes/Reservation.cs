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
            var weliets = false;
            // var text = "";

        List<Action> probeer = new List<Action>();

            if(entryTickets.Contains(tickets)){
                probeer.Add(new (){text = "Uw ticket is geldig"});
                

                foreach (var checkTour in alltours)
                {
                    if(checkTour.bookings.Any()){

                    foreach (var reservation in checkTour.bookings)
                    {
                        // if(reservation.userId == tickets && reservation.occupationStatus == OccupationStatus.Canceled){
                        //     probeer.Add(new (){text = "Uw vorige boekingen: \n"});
                        //     probeer.Add(new (){text = checkTour.dateTime.ToString()});
                        //     probeer.Add(new (){text = "Rondleiding duur: " + checkTour.tourDuration.ToString() + " min",hasExtraBreak = true});
                        //     break;
                        //     }
                        if(reservation.userId == tickets && reservation.occupationStatus == OccupationStatus.Joined){
                            weliets = true;

                            probeer.Add(new (){text = "De Rondleiding die u heeft geboekt: \n"});
                            probeer.Add(new (){text = checkTour.dateTime.ToString()});
                            probeer.Add(new (){text = "Rondleiding duur: " + checkTour.tourDuration.ToString() + " min",hasExtraBreak = true});
                            
                            probeer.Add(new(){
                                    text = "Reservering annuleren",
                                    onAction = line => {
                                        changeReservations.cancelReservation(checkTour, reservation, true);
                                    }
                                });
                            probeer.Add(new(){
                                    text = "Reservering wijzigen",
                                    onAction = line => {
                                        List<Action> actions = new();

                                        foreach (var tour in alltours)
                                        {
                                            //Getting the free places from the tour and checking if it is full
                                            int freePlaces = Tour.tourFreePlaces(tour);

                                            bool isFull = freePlaces == 0;
                                            bool isStarted = tour.tourStarted;

                                            //Adding the action items
                                            actions.Add(
                                                new()
                                                {
                                                    text = $"{tour.dateTime.ToShortTimeString()} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()} ({(isStarted ? "Tour al gestart" : isFull ? "Volgeboekt" : $"{freePlaces} van de {tour.maxBookingCount} plaatsen vrij")})",
                                                    onAction = line =>
                                                    {
                                                        changeReservations.moveReservation(tour, checkTour, reservation, tickets);
                                                    }
                                                    
                                                }
                                            );
                                        }   
                                        ProgramManger.setActions(actions);     
                                    }
                                });
                            probeer.Add(new(){
                                    text = "Terug naar start",
                                   
                                    onAction = line => {
                                        ProgramManger.setActions(Program.getStartScreen());
                                    }
                                });
                                break;
                            }
                        

                        }
                        
                    }
                    
                    }
                    if(weliets == false){
                        // Console.WriteLine("U heeft geen rondleiding geboekt");
                        probeer.Add(new (){text = "U heeft geen rondleiding geboekt", hasExtraBreak = true});
                        probeer.Add(new(){
                                    text = "Terug naar start",
                                   
                                    onAction = line => {
                                        ProgramManger.setActions(Program.getStartScreen());
                                    }
                                });
                        }
                    return probeer;

                }
                

                // if(tours.bookings.Contains(tickets)){
                //     Console.WriteLine("U heeft een boeking");
                //     // Console.WriteLine($"Uw rondleiding begint om {tours.dateTime}"); 
                // }else{
                //     Console.WriteLine("U heeft nog geen boeking staan");
                // }
            
            else{
                // Console.WriteLine("Uw ticket heeft geen recht op een rondleiding");
                probeer.Add(new (){text = "Uw ticket heeft geen recht op een rondleiding", hasExtraBreak = true});
                probeer.Add(new(){
                                    text = "Terug naar start",
                                   
                                    onAction = line => {
                                        ProgramManger.setActions(Program.getStartScreen());
                                    }
                                });
                return probeer;
            }
            }
            
        }
}
        