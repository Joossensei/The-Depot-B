using System;
using Newtonsoft.Json;
using System.Globalization;

namespace ReservationSystem
{
    class Program
    {

        // Get the current date and time
        public static DateTime now = DateTime.Now;

        // Set the opening and closing times for the restaurant
        DateTime openingTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
        DateTime closingTime = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0);

        //Some test data for the tours

            //v1 tours Pieter
            /*new (){
                dateTime = DateTime.Now
            },
            new(){
                dateTime = DateTime.Now.AddMinutes(20)
            },
            new(){
                dateTime = DateTime.Now.AddMinutes(40)
            },
            new(){
                dateTime = DateTime.Now.AddMinutes(60)
            }
        };*/
        /*static Tour[] tours =*/
        static List<Tour> tours = new List<Tour>
        {


            new (){
            dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 20, 0)
            },
            new(){
                dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 40, 0)
            },
            new(){
                dateTime = new DateTime(now.Year, now.Month, now.Day, now.AddHours(1).Hour, 0, 0)
            },

        };

        
        //v1 tours Pieter
        /*new (){
            dateTime = DateTime.Now
        },
        new(){
            dateTime = DateTime.Now.AddMinutes(20)
        },
        new(){
            dateTime = DateTime.Now.AddMinutes(40)
        },
        new(){
            dateTime = DateTime.Now.AddMinutes(60)
        }
    };*/

        static void Main(string[] args)
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Set the opening and closing times for the restaurant
            DateTime openingTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
            DateTime closingTime = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0);

            // Create a list to store the tours
            List<Tour> tours = new List<Tour>();

            // Set the tour duration to 20 minutes
            int tourDuration = 20;

            // Start time for the first tour
            DateTime startTime = openingTime;

            // Create tours until closing time

            while (startTime.AddMinutes(tourDuration) <= closingTime)
            {
                // Create a new tour with the current start time
                Tour tour = new Tour { dateTime = startTime };

                // Add the tour to the list of tours
                tours.Add(tour);

                // Increment the start time for the next tour
                startTime = startTime.AddMinutes(tourDuration);
            }

            // Write the list of tours to a JSON file
            var manager = new jsonManager();
            manager.writeToJson(tours, @"JsonFiles\tours.json");
            
            // Load entry tickets from JSON file
            List<string> entryTickets = jsonManager.LoadEntryTickets();

            // load tours from JSON file
            List<Tour> alltours = jsonManager.LoadTours();

            // staring the program    
            ProgramManger.start(getStartScreen());
        }

        //Function to get the home screen elements the start screen
        static List<Action> getStartScreen()
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
                    text = "Registratie controleren",
                    onAction = line => {
                        ProgramManger.setActions(new(){
                            new(){
                                text = "Enter value any value"
                            }
                        }, line =>{
                            Console.WriteLine($"Value: {line} has been enterd");
                        });
                    }
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

        static List<Action> getTours()
        {
            List<Action> actions = new();
            Tours myTours = new Tours();
            Tour[] tours = myTours.GetTours();

            foreach (var tour in tours)
            {   

                //Getting the free places from the tour and checking if it is full
                int freePlaces = tour.maxBookingCount - tour.bookings.Count;
                bool isFull = freePlaces == 0;

                //Adding the action items
                actions.Add(
                    new()
                    {
                        text = $"{tour.dateTime.ToShortTimeString()} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()} ({(isFull ? "Volgeboekt" : $"{freePlaces} van de {tour.maxBookingCount} plaatsen vrij")})",
                        onAction = line =>
                        {
                            ProgramManger.setActions(getTour(tour));
                        }
                    }
                );
            }

            return actions;
        }

        static List<Action> getTour(Tour tour)
        {
            //Getting the free places from the tour and checking if it is full
            int freePlaces = tour.maxBookingCount - tour.bookings.Count;
            bool isFull = freePlaces == 0;

            return new(){
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new()
                {
                    text = $"{tour.dateTime.ToShortTimeString()} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()}\n{(isFull ? "Volgeboekt" : $"{freePlaces} van de {tour.maxBookingCount} plaatsen vrij")}",
                    hasExtraBreak = true,
                },
                new (){
                    validRoles = new Role[]{Role.Customer},
                    text = "Rondleiding boeken",
                    onAction = line => {
                        makeReservation.ReserveTour(Console.ReadLine(), tour);
                }
                },
                new (){
                    validRoles = new Role[]{Role.Admin,Role.Guide},
                    text = "Rondleiding starten",
                    onAction = line => {
                        startTour.startTour.selectTour();
                    }
                },
                new (){
                    text = "Terug naar start",
                    onAction = line => {
                        ProgramManger.setActions(getStartScreen());
                    }
                },
            };
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
            actions.AddRange(getTours());

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