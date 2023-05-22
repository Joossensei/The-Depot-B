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
            var tourFound = false;
            // var text = "";

            List<Action> TourCheckReturn = new List<Action>();
            List<Tour> cancelledTours = new List<Tour>(); 

            if(entryTickets.Contains(tickets)){
                TourCheckReturn.Add(new (){text = "Uw ticket is geldig",textType = TextType.Success});
                

                foreach (var checkTour in alltours)
                {
                    if(checkTour.bookings.Any()){

                    foreach (var reservation in checkTour.bookings)
                    {
                        if(reservation.userId == tickets && reservation.occupationStatus == OccupationStatus.Canceled){
                            cancelledTours.Add(checkTour);
                            continue;
                        }
                        if(reservation.userId == tickets && reservation.occupationStatus == OccupationStatus.Joined){
                            tourFound = true;

                            TourCheckReturn.Add(new (){text = "De Rondleiding die u heeft geboekt: \n"});
                            TourCheckReturn.Add(new (){text = checkTour.dateTime.ToString("HH:mm")});
                            
                            TourCheckReturn.Add(new(){
                                    text = "Reservering annuleren",
                                    onAction = line => {
                                        changeReservations.cancelReservation(checkTour, reservation, true);
                                    }
                                });
                            TourCheckReturn.Add(new(){
                                    text = "Reservering wijzigen",
                                    hasExtraBreak=true,
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

                                                    text = $"{tour.dateTime.ToString("HH:mm")} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()} ({(isStarted ? "Rondleiding al gestart" : isFull ? "Volgeboekt" : $"{freePlaces} plaatsen vrij")})",
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
                                continue;
                            }
                        

                        }
                    }
                }
                if(cancelledTours.Count >= 1 && tourFound == true){
                    
                    TourCheckReturn.Add(new(){
                        text="Dit zijn uw geannuleerde rondleidingen:"
                    });
                    foreach (Tour cancelledTour in cancelledTours)
                    {
                        TourCheckReturn.AddRange(new List<Action>  {
                            new (){text = cancelledTour.dateTime.ToString("HH:mm")}
                        });
                    }
                }
                TourCheckReturn.Add(new(){
                    text = "Terug naar overzicht",
                    onAction = line => {
                        ProgramManger.setActions(Program.getStartScreen());
                    }
                });
                return TourCheckReturn;

            }
                

                // if(tours.bookings.Contains(tickets)){
                //     Console.WriteLine("U heeft een boeking");
                //     // Console.WriteLine($"Uw rondleiding begint om {tours.dateTime}"); 
                // }else{
                //     Console.WriteLine("U heeft nog geen boeking staan");
                // }
            
            else{
                // Console.WriteLine("Uw ticket heeft geen recht op een rondleiding");
                TourCheckReturn.AddRange(
                    new List<Action> {
                        new (){text = "Ongeldig ticket", hasExtraBreak = true},
                        new (){
                            validRoles = new Role[]{Role.Bezoeker},
                            text = "Nog een ticket scannen",
                            onAction = line => {
                                ProgramManger.setActions(new(){
                                    new(){
                                        text = "Vul uw ticket in:"
                                    }
                                }, line =>{
                                    List<Action> actions = Reservation.tourRes(line);

                                    ProgramManger.setActions(actions);
                                });
                            },
                        }
                    }
                );
                
                TourCheckReturn.Add(new(){
                                    text = "Terug naar overzicht",
                                   
                                    onAction = line => {
                                        ProgramManger.setActions(Program.getStartScreen());
                                    }
                                });
                return TourCheckReturn;
            }
            }
            
        }
}
        