using System;
using Newtonsoft.Json;
using System.Globalization;

namespace ReservationSystem
{
    class Program
    {

        // Get the current date and time
        public static DateTime now = DateTime.Now;

        // Set the opening and closing times for the Depot
        DateTime openingTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
        DateTime closingTime = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0);

        //Some test data for the tours
        public static List<Tour> tours = new List<Tour> { };

        static void Main(string[] args)
        {

            //make all the tour for today
            Tour.writeAllTours();

            // Load entry tickets from JSON file
            List<string> entryTickets = jsonManager.LoadEntryTickets();

            // load tours from JSON file
            tours = jsonManager.LoadTours();

            // staring the program    
            ProgramManger.start(getStartScreen());
        }

        //Function to get the home screen elements the start screen
        public static List<Action> getStartScreen()
        {

            List<Action> actions = new List<Action>{
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new (){
                    text = $"Beschikbare rondleidingen ({DateTime.Now.ToShortDateString()})",
                    hasExtraBreak = true
                }
            };

            //Adding the tours
            actions.AddRange(getTours());

            //Adding actions
            actions.AddRange(new List<Action>(){
                 new (){},
                 new (){
                    validRoles = new Role[]{Role.Customer},
                    text = "Reserveringen controleren",
                    onAction = line => {
                        ProgramManger.setActions(new(){
                            new(){
                                text = "Enter value any value"
                            }
                        }, line =>{
                            Console.WriteLine($"Value: {line} has been enterd");
                            List<Action> actions = Reservation.tourRes(line); 
                            actions.Add( new(){
                                    text = "Terug naar start",
                                   
                                    onAction = line => {
                                        ProgramManger.setActions(Program.getStartScreen());
                                    }
                                }
                            );  
                            ProgramManger.setActions(actions);                        
                        });
                    },      
                 },
                new (){
                    validRoles = new Role[]{Role.Admin},
                    text = "Statistieken inzien",
                    onAction = line => {
                        ProgramManger.setActions(getStatistics());
                    }
                },
                new (){
                    validRoles = new Role[]{Role.Admin,Role.Guide},
                    text = "Uitloggen",
                    onAction = line => {
                        ProgramManger.userRole = Role.Customer;
                        ProgramManger.setActions(getStartScreen());
                    }
                },
                new (){
                    validRoles = new Role[]{Role.Customer},
                    text = "Personeel login",
                    onAction = line => {
                        ProgramManger.setActions(new List<Action>{
                            new(){
                                text = "Voer je unieke code in of scan je badge om in te loggen"
                            }
                        }, (line)=>{
                            if(line == "admin"){
                                ProgramManger.userRole = Role.Admin;
                                ProgramManger.setActions(getStartScreen());
                            }
                            else if(line == "guide"){
                                ProgramManger.userRole = Role.Guide;
                                ProgramManger.setActions(getStartScreen());
                            }
                        });
                    }
                },
            });

            return actions;
        }

        static List<Action> getTours(bool hasActions = true)
        {
            List<Action> actions = new();

            foreach (var tour in tours)
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
                        onAction = hasActions ? line =>
                        {
                            ProgramManger.setActions(getTour(tour));
                        }
                        : null
                    }
                );
            }

            return actions;
        }

        static List<Action> getTour(Tour tour)
        {
            //Getting the free places from the tour and checking if it is full
            int freePlaces = Tour.tourFreePlaces(tour);
            bool isFull = freePlaces == 0;
            bool isStarted = tour.tourStarted;
            List<Action> actions = new List<Action> {};

            //Attempt to get a ticketID and make a reservation
            if (!makeReservation.getUsersTicketAndMakeReservation(tour))
            {
                actions.Add(new()
                {
                    text = "Dit ticket mag geen reservingen maken"
                });
            }

            actions.AddRange( new List<Action> {
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new()
                {
                    text = $"{tour.dateTime.ToShortTimeString()} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()}\n{(isStarted? "Tour is al gestart" : isFull ? "Volgeboekt" : $"{freePlaces} van de {tour.maxBookingCount} plaatsen vrij")}",
                    hasExtraBreak = true,

                }
            }
            );

            if(isStarted == false) {
                if(isFull == false) {
                    actions.Add(
                        new (){
                        validRoles = new Role[]{Role.Customer},
                        text = "Rondleiding reserveren",
                        onAction = line => {
                            if (!makeReservation.getUsersTicketAndMakeReservation(tour))
                            {
                                actions.Add(new()
                                {
                                    text = "Dit ticket mag geen reservingen maken"
                                });
                            }
                        }
                    }
                    );
                };
                actions.Add(
                    new (){
                    validRoles = new Role[]{Role.Admin,Role.Guide},
                    text = "Rondleiding starten",
                    onAction = line => {
                        startTour.startTour.start(tour, 0);
                    }
                }
                );
            }

            actions.Add(
                new (){
                    text = "Terug naar start",
                    onAction = line => {
                        ProgramManger.setActions(getStartScreen());
                    }
                }
            );

            return actions;
        }

        static List<Action> getStatistics()
        {
            List<Action> actions = new(){
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new (){
                    text = $"Rondleidingen ({DateTime.Now.ToShortDateString()})",
                    hasExtraBreak = true
                }
            };

            //Add tours from today
            actions.AddRange(getTours(hasActions: false));

            //Add other statistics
            actions.AddRange(new List<Action>(){
                new (){ text = "\nBezoekers: 3242",},
                new (){ text = "Rondleiding boekingen: 120",},
                new (){ text = "Rondleiding aanwezigen: 112",},
                new (){ text = "Rondleiding afwezigen: 8",},
                new (){
                    text = "Annuleringen: 34",
                    hasExtraBreak = true,
                },
                new (){
                    text = "Statistieken periode",
                    onAction = line => {}
                },
                new (){
                    text = "Terug naar start",
                    onAction = line => {
                        ProgramManger.setActions(getStartScreen());
                    }
                },
            });

            return actions;
        }


    }
}