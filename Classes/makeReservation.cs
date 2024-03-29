namespace ReservationSystem;

class makeReservation
{

    //General function if the reservation is invalid or fails to prevent duplicate code
    private static List<Action> invalidReservation(string reason, Tour tour, Action extraAction = default, bool tryAgain = true, bool forCheckIn = false)
    {
        List<Tour> tours = Program.tours;

        List<Action> actions = new List<Action> { };
        actions.Add(new()
        {
            text = reason,
            textType = TextType.Error,
            hasExtraBreak = true
        });

        if (tryAgain == true)
        {
            actions.Add(
            new()
            {
                text = "Nog eens proberen",
                hasExtraBreak = false,
                onAction = line =>
                {
                    ProgramManger.setActions(new List<Action>()
                        {
                            new ()
                            {
                                text = "Scan uw unieke code om nu te reserveren"
                            }
                        }, line =>
                        {
                            ReserveTour(line, tour, forCheckIn);
                        }
                    );
                }
            }
            );
        }

        if (extraAction != default)
        {
            actions.Add(extraAction);
        }

        //Always add a return to home as last item on the list
        if (forCheckIn == false)
        {
            actions.Add(
                new()
                {
                    text = "Terug naar overzicht",
                    hasExtraBreak = false,
                    onAction = line =>
                    {
                        ProgramManger.setActions(Program.getStartScreen());

                    }
                }
            );
        }else{
            actions.Add(
                new()
                {
                    text = "Terug naar menu",
                    hasExtraBreak = false,
                    onAction = line =>
                    {
                        startTour.start(tour);
                    }
                }
            );
        }

        return actions;
    }


    public static bool checkTicketValidity(string ticketID)
    {
        bool validTicket = false;

        List<string> entryTickets = jsonManager.LoadEntryTickets();
        foreach (var ticket in entryTickets)
        {
            if (ticket.ToLower() == ticketID.ToLower())
            {
                validTicket = true;
            }
        }
        return validTicket;
    }

    public static void ReserveTour(string ticketID, Tour tour, bool forCheckIn = false)
    {

        List<Action> actions = new List<Action> { };

        List<Tour> tours = Program.tours;
        if (Tour.tourFreePlaces(tour) > 0 && tour.tourStarted == false)
        {


            bool validTicket = checkTicketValidity(ticketID);

            if (validTicket == false)
            {
                actions = invalidReservation("Deze code is ongeldig. Probeer het opnieuw", tour);
            }
            else
            {

                //If the ticket is valid, check if the user already has a reservation
                bool hasReservation = false;
                foreach (Tour checkTour in tours)
                {
                    foreach (Booking reservation in checkTour.bookings)
                    {
                        if(reservation.userId == ticketID && reservation.occupationStatus == OccupationStatus.Visited) {
                            Action ExtraAction = new(){
                                text="U heeft al een reservering"
                            };
                        }
                        else if (reservation.userId == ticketID && reservation.occupationStatus != OccupationStatus.Canceled && checkTour.id == tour.id)
                        {   //IF current loop's ticket is the same as scanned ticket & current loop's ticket is not cancelled & loop's tour is the same as requested tour, it means the user already has a reservation for this tour
                            hasReservation = true;
                            ProgramManger.setActions(invalidReservation($"U heeft al een reservering voor deze rondleiding", tour, tryAgain: false));
                            return;
                        }
                        else if (reservation.userId == ticketID && reservation.occupationStatus == OccupationStatus.Joined)
                        {
                            hasReservation = true;

                            Action extraAction = new()
                            {
                                text = "Huidige reservering annuleren en voor deze rondleiding inschrijven",
                                hasExtraBreak = false,
                                onAction = line =>
                                {
                                    changeReservations.moveReservation(tour, checkTour, reservation, ticketID);
                                }
                            };

                            ProgramManger.setActions(invalidReservation($"U heeft al een reservering staan ({checkTour.dateTime.ToString("HH:mm")})", tour, extraAction, false));
                            return;

                        }
                    }
                }

                if (hasReservation == false)
                {
                    foreach (var checkTour in tours)
                    {
                        if (checkTour.id == tour.id)
                        {

                            //Add the booking/reservation to the current tour
                            //Check in right away if called from starttour
                            if (forCheckIn == true)
                            {
                                checkTour.bookings.Add(new Booking
                                {
                                    userId = ticketID,
                                    tourId = tour.id,
                                    createData = DateTime.Now,
                                    occupationStatus = OccupationStatus.Visited
                                });
                            }
                            else
                            {
                                checkTour.bookings.Add(new Booking
                                {
                                    userId = ticketID,
                                    tourId = tour.id,
                                    createData = DateTime.Now,
                                    occupationStatus = OccupationStatus.Joined
                                });
                            }
                        }
                    }
                }
                var manager = new ReservationSystem.jsonManager();
                manager.writeToJson(tours, @"JsonFiles/tours.json");

                actions = new List<Action> {
                    new() {
                        text = $"De rondleiding van ({tour.dateTime.ToString("HH:mm")}) is succesvol gereserveerd.",
                        hasExtraBreak = true,
                        textType = TextType.Success
                    },
                    new() {

                    text = $"Nog een reservering maken voor deze rondleiding ({tour.dateTime.ToString("HH:mm")})",

                    hasExtraBreak = false,
                    onAction = line => {
                        ProgramManger.setActions(new List<Action>()
                            {
                                new ()
                                {
                                    text = "Scan uw unieke code om nu te reservering"
                                }
                            }, line =>
                            {
                                ReserveTour(line, tour);
                            }
                        );
                    }
                    },
                };
                if (forCheckIn == false)
                {
                    actions.Add(new()
                    {
                        text = "Terug naar overzicht",
                        hasExtraBreak = false,
                        onAction = line =>
                        {
                            ProgramManger.setActions(Program.getStartScreen());
                        }
                    });
                }
                else
                {
                    actions.Add(new()
                    {
                        text = "Terug naar menu",
                        hasExtraBreak = false,
                        onAction = line =>
                        {
                            startTour.start(tour);
                        }
                    });
                };
            }

        }
        else
        {
            ProgramManger.setActions(invalidReservation("Deze tour zit helaas al vol", tour, tryAgain: false));
            return;


        }
        ProgramManger.setActions(actions, automaticClose: true);

    }
}
