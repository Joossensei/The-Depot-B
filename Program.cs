﻿using System;
using System.Runtime.InteropServices;
using System.Threading;

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
        public static List<string> entryTickets = new List<string>();

        public static List<string> employeCodes = new() { "g1", "g2", "g3", "a1" };

        static void Main(string[] args)
        {

            //make all the tour for today
            Tour.writeAllTours();

            // Load entry tickets from JSON file
            entryTickets = jsonManager.LoadEntryTickets();

            // load tours from JSON file
            tours = jsonManager.LoadTours();

            // staring the program    
            ProgramManger.start(getStartScreen());
        }

        //Function to get the home screen elements the start screen
        public static List<Action> getStartScreen()
        {

            List<Action> actions = new List<Action> {};

                actions.AddRange(new List<Action>{
                new (){
                    validRoles = new Role[] {Role.Gids, Role.Afdelingshoofd},
                    text = "Typ het nummer wat naast de actie wordt aangegeven.",
                    hasExtraBreak = true
                },
                new (){
                    validRoles = new Role[] {Role.Bezoeker},
                    text = "Typ het nummer wat naast de actie wordt aangegeven.",
                },
                 new (){
                    validRoles = new Role[] {Role.Bezoeker},
                    text = "[Om een reservering te maken vul het getal in van de gewenste tijd]",
                    hasExtraBreak = true,
                    textType = TextType.Success  
                },
                new (){
                    text = $"Beschikbare rondleidingen ({DateTime.Now.ToShortDateString()})",
                    hasExtraBreak = true
                }
            });
            

            if(ProgramManger.userRole == Role.Gids){
                // Adding gids tours
                actions.AddRange(getToursToStart());
            }else{
                //Adding the tours
                actions.AddRange(getTours());
            }

            //Adding actions
            actions.AddRange(new List<Action>(){
                 new (){},
                 new (){
                    validRoles = new Role[]{Role.Bezoeker},
                    text = "Reservering beheren",
                    onAction = line => {
                        ProgramManger.setActions(new List<Action>{
                            new(){
                                text = "Voer je unieke code in of scan je badge om in te loggen"
                            },
                            new() {
                                text= "Terug naar start",
                                onAction = line =>
                                {
                                    ProgramManger.setActions(getStartScreen());
                                }
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
                        List<Action> actions = new List<Action> {};
                        actions.Add(new() {
                            text="Succesvol uitgelogd",
                            textType=TextType.Success,
                            hasExtraBreak = true
                        }); 
                        actions.AddRange(getStartScreen());
                        ProgramManger.setActions(actions);
                    }
                },
                new (){
                    validRoles = new Role[]{Role.Bezoeker},
                    text = "Personeel login",
                    onAction = line => {
                        ProgramManger.setActions(new List<Action>{
                            new(){
                                text = "Voer je unieke ticket in of scan je badge om in te loggen"
                            },
                            new() {
                                text= "Terug naar overzicht",
                                onAction = line =>
                                {
                                    ProgramManger.setActions(getStartScreen());
                                }
                            }
                        }, (line)=>{
                            //Checking if the unique code exists
                            line = line.ToLower();
                            if(employeCodes.Contains(line) || entryTickets.Contains(line)){
                                //Checking if the code is for an Afdelingshoofd
                                if(line.Contains('a')){
                                    ProgramManger.userRole = Role.Afdelingshoofd;
                                    List<Action> actions = new List<Action> ();
                                    actions.Add(new(){
                                        text="Ingelogd als afdelingshoofd",
                                        textType = TextType.Success
                                    });
                                    actions.AddRange(getStartScreen());

                                    ProgramManger.setActions(actions);
                                }
                                //Else check if the code is for an Gids
                                else if(line.Contains('g')){
                                    ProgramManger.userRole = Role.Gids;
                                    List<Action> actions = new List<Action> ();
                                    actions.Add(new(){
                                        text="Ingelogd als gids",
                                        textType = TextType.Success
                                    });
                                    actions.AddRange(getStartScreen());

                                    ProgramManger.setActions(actions);
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
            DateTime today = DateTime.Now;
            List<Tour> tourstoday = tours.Where(t => t.dateTime.Date == today.Date ).ToList();
            foreach (Tour tour in tourstoday)
            {

                //Getting the free places from the tour and checking if it is full
                int freePlaces = Tour.tourFreePlaces(tour);

                bool isFull = freePlaces == 0;
                bool isStarted = tour.tourStarted;

                //Adding the action items
                // Console.WriteLine(isStarted);
                if (!isFull && !isStarted)
                {
                    actions.Add(
                        new()

                        {

                            text = $"{tour.dateTime.ToShortTimeString()} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()} ({(isStarted ? "Rondleiding al gestart" : isFull ? "Volgeboekt" : $"{freePlaces} plaatsen vrij")})",
                            textType = isFull || isStarted ? TextType.Error : TextType.Normal,
                            onAction = hasActions ? line =>
                            {
                                getTour(tour);
                            }
                            : null

                        }
                    );
                }
            }

            return actions;
        }

        public static List<Action> getToursToStart(bool hasActions = true)
        {
            List<Action> actions = new();
            DateTime today = DateTime.Now;
            List<Tour> tourstoday = tours.Where(t => t.dateTime.Date == today.Date ).ToList();
            int teller = 0;

            foreach (Tour tour in tourstoday)
            {

                //Getting the free places from the tour and checking if it is full
                int freePlaces = Tour.tourFreePlaces(tour);

                bool isFull = freePlaces == 0;
                bool isStarted = tour.tourStarted;

                //Adding the action items
                // Console.WriteLine(isStarted);
                if (!isStarted)
                {
                    if(teller < 1){
                        actions.Add(
                            new()

                            {

                                text = $"{tour.dateTime.ToShortTimeString()} - {tour.dateTime.AddMinutes(tour.tourDuration).ToShortTimeString()} ({(isStarted ? "Rondleiding al gestart" : isFull ? "Volgeboekt" : $"{freePlaces} plaatsen vrij")})",
                                textType = isFull || isStarted ? TextType.Error : TextType.Normal,
                                onAction = hasActions ? line =>
                                {
                                    getTour(tour);
                                }
                                : null

                            }
                        );
                        teller++;
                    }
                }
            }

            return actions;
        }

        static void getTour(Tour tour)
        {

            int freePlaces = Tour.tourFreePlaces(tour);
            bool isFull = freePlaces == 0;
            bool isStarted = tour.tourStarted;

            if (ProgramManger.userRole == Role.Bezoeker && isStarted == false && isFull == false)
            {

                ProgramManger.setActions(new List<Action>()
                    {
                        new ()
                        {
                            text = "Voer je ticketnummer in of scan je badge om te reserveren"
                        }
                    }, line =>
                    {
                        makeReservation.ReserveTour(line, tour);
                    }
                );
            }
            else if(ProgramManger.userRole == Role.Gids && isStarted == false){
                startTour.start(tour);
            }
            else if (isStarted == false)
            {
                List<Action> actions = new List<Action> { };

                actions.AddRange(new List<Action> {
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new()
                {

                    text = $"{tour.dateTime.ToString("HH:mm")} - {tour.dateTime.AddMinutes(tour.tourDuration).ToString("HH:mm")}\n{(isStarted? "Rondleiding is al gestart" : isFull ? "Volgeboekt" : $"{freePlaces} plaatsen vrij")}",
                    hasExtraBreak = true,

                },
                new()
                {
                    validRoles = new Role[] { Role.Afdelingshoofd, Role.Gids },
                    text = "Rondleiding starten",
                    onAction = line =>
                    {
                        startTour.start(tour);
                    }
                }
                });


                if (isFull == false)
                {
                    actions.Add(
                        new()
                        {
                          validRoles = new Role[] { Role.Bezoeker },
                            text = "Rondleiding reserveren",
                            onAction = line =>
                            {
                                ProgramManger.setActions(new List<Action>()
                                    {
                                        new ()
                                        {
                                            text = "Voer je ticketnummer in of scan je badge om nu te reserveren"
                                        }
                                    }, line =>
                                    {
                                        makeReservation.ReserveTour(line, tour);
                                    }
                                );
                            }
                        }
                    );
                }
                ProgramManger.setActions(actions);
            }
        }
    }

}