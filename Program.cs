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

        static List<Action> getTours(bool hasActions = true)
        {
            List<Action> actions = new();

            foreach (var tour in tours)
            {

                //Getting the free places from the tour and checking if it is full
                int freePlaces = Tour.tourFreePlaces(tour);
                bool isFull = freePlaces <= 0;

                //Adding the action items
                actions.Add(
                    new()
                    {
                        text = $"{tour.dateTime.ToShortTimeString()} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()} ({(isFull ? "Volgeboekt" : $"{freePlaces} van de {tour.maxBookingCount} plaatsen vrij")})",
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

            //Attempt to get a ticketID and make a reservation
            if (ProgramManger.userRole == Role.Customer && !isFull)
            {
                Console.WriteLine($"Scan nu uw ticket om deze tour te boeken ({tour.dateTime})");
                string ticketID = ProgramManger.readLine();
                if (ticketID != "")
                {
                    if (makeReservation.checkTicketValidity(ticketID))
                    {
                        makeReservation.ReserveTour(ticketID, tour);
                        return new() { };
                    }
                    else
                    {
                        Console.WriteLine("Dit ticket is niet correct");
                    }
                }
            }

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
                    text = "Rondleiding reserveren",
                    onAction = line => {
                        Console.WriteLine("Scan nu uw code:");
                        makeReservation.ReserveTour(ProgramManger.readLine(), tour);
                }
                },
                new (){
                    validRoles = new Role[]{Role.Admin,Role.Guide},
                    text = "Rondleiding starten",
                    onAction = line => {
                        startTour.startTour.start(tour, 0);
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
            return statisticScreen.getStatistics();
        }


    }
}