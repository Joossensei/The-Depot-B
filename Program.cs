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
        public static List<Tour> tourstoday = new List<Tour> { };
        public static List<string> entryTickets = new List<string>();

        public static List<string> employeCodes = new() { "g1", "g2", "g3", "a1" };

        static void Main(string[] args)
        {

            //make all the tour for today
            Tour.writeAllTours();

            // Load entry tickets from JSON file
            entryTickets = jsonManager.LoadEntryTickets();

            // load tours from JSON file
            tourstoday = jsonManager.LoadToursToday();
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
                    validRoles = new Role[]{Role.Bezoeker},
                    text = "Reserveringen controleren",
                    onAction = line => {
                        ProgramManger.setActions(new(){
                            new(){
                                text = "Vul uw ticket in:"
                            }
                        }, line =>{
                            Console.WriteLine($"Ticket: {line} is ingevuld.");
                            List<Action> actions = Reservation.tourRes(line);

                            ProgramManger.setActions(actions);
                        });
                    },
                 },
                new (){
                    validRoles = new Role[]{Role.Afdelingshoofd},
                    text = "Statistieken inzien",
                    onAction = line => {
                        ProgramManger.setActions(statisticScreen.getStatistics());
                    }
                },
                new (){
                    validRoles = new Role[]{Role.Afdelingshoofd,Role.Gids},
                    text = "Uitloggen",
                    onAction = line => {
                        ProgramManger.userRole = Role.Bezoeker;
                        ProgramManger.setActions(getStartScreen());
                    }
                },
                new (){
                    validRoles = new Role[]{Role.Bezoeker},
                    text = "Personeel login",
                    onAction = line => {
                        ProgramManger.setActions(new List<Action>{
                            new(){
                                text = "Voer je unieke code in of scan je badge om in te loggen"
                            }
                        }, (line)=>{
                            //Checking if the unique code exists
                            if(employeCodes.Contains(line) || entryTickets.Contains(line)){
                                //Checking if the code is for an Afdelingshoofd
                                if(line.Contains('a')){
                                    ProgramManger.userRole = Role.Afdelingshoofd;
                                    ProgramManger.setActions(getStartScreen());
                                }
                                //Else check if the code is for an Gids
                                else if(line.Contains('g')){
                                    ProgramManger.userRole = Role.Gids;
                                    ProgramManger.setActions(getStartScreen());
                                }
                                else{
                                    ProgramManger.errors.Add("Gebruikers kunnen niet inloggen");
                                }
                            }
                            else{
                                ProgramManger.errors.Add("Unieke code niet gevonden");
                            }
                        }, isPassword: true);
                    }
                },
            });

            return actions;
        }

        public static List<Action> getTours(bool hasActions = true)
        {
            List<Action> actions = new();

            foreach (var tour in tourstoday)
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
                        textType = isFull || isStarted ? TextType.Error : TextType.Normal,
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
            List<Action> actions = new List<Action> { };

            //Attempt to get a ticketID and make a reservation
            if (ProgramManger.userRole == Role.Bezoeker)
            {
                if (!makeReservation.getUsersTicketAndMakeReservation(tour))
                {
                    actions.Add(new()
                    {
                        text = "Dit ticket mag geen reservingen maken",
                        textType = TextType.Error
                    });
                }
            }

            actions.AddRange(new List<Action> {
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

            if (isStarted == false)
            {
                if (isFull == false)
                {
                    actions.Add(
                        new()
                        {
                            validRoles = new Role[] { Role.Bezoeker },
                            text = "Rondleiding reserveren",
                            onAction = line =>
                            {
                                if (!makeReservation.getUsersTicketAndMakeReservation(tour))
                                {
                                    actions.Add(new()
                                    {
                                        text = "Dit ticket mag geen reservingen maken",
                                        textType = TextType.Error
                                    });
                                }
                            }
                        }
                    );
                };
                actions.Add(
                    new()
                    {
                        validRoles = new Role[] { Role.Afdelingshoofd, Role.Gids },
                        text = "Rondleiding starten",
                        onAction = line =>
                        {
                            startTour.startTour.start(tour, 0);
                        }
                    }
                );
            }

            actions.Add(
                new()
                {
                    text = "Terug naar start",
                    onAction = line =>
                    {
                        ProgramManger.setActions(getStartScreen());
                    }
                }
            );

            return actions;
        }
    }
}
