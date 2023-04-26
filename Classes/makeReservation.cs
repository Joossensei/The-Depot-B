namespace ReservationSystem;

class makeReservation
{

    //General function if the reservation is invalid or fails to prevent duplicate code
    private static List<Action> invalidReservation(string reason, Tour tour, Action extraAction = default, bool tryAgain = true)
    {
        List<Tour> tours = Program.tourstoday;

        List<Action> actions = new List<Action> { };
        actions.Add(new()
        {
            text = reason,
            textType = TextType.Error
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
                  /*  if (!makeReservation.getUsersTicket(tour))
                    {
                        actions.Add(new()
                        {
                            text = "Dit ticket mag geen reservingen maken"
                        });
                    }*/

                }
            }
            );
        }

        if (extraAction != default)
        {
            actions.Add(extraAction);
        }

        //Always add a return to home as last item on the list
        actions.Add(
            new()
            {
                text = "Terug naar start",
                hasExtraBreak = false,
                onAction = line =>
                {
                    ProgramManger.setActions(Program.getStartScreen());

                }
            }
        );

        return actions;//,automaticClose:true);
    }


    public static bool checkTicketValidity(string ticketID)
    {
        bool validTicket = false;

        List<string> entryTickets = jsonManager.LoadEntryTickets();
        foreach (var ticket in entryTickets)
        {
            if (ticket == ticketID)
            {
                validTicket = true;
            }
        }
        return validTicket;
    }

    public static List<Action> ReserveTour(string ticketID, Tour tour)
    {

        List<Action> actions = new List<Action> { };

        List<Tour> tours = Program.tourstoday;
        if (Tour.tourFreePlaces(tour) > 0 || tour.tourStarted == false)
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
                foreach (var checkTour in tours)
                {
                    foreach (var reservation in checkTour.bookings)
                    {
                        if ((reservation.userId == ticketID) && (reservation.occupationStatus != OccupationStatus.Canceled) && checkTour.id == tour.id)
                        {   //IF current loop's ticket is the same as scanned ticket & current loop's ticket is not cancelled & loop's tour is the same as requested tour, it means the user already has a reservation for this tour
                            hasReservation = true;
                            actions.AddRange(invalidReservation($"U heeft al een reservering voor deze rondleiding", tour, tryAgain: false));
                        }
                        else if ((reservation.userId == ticketID) && (reservation.occupationStatus == OccupationStatus.Joined))
                        {
                            hasReservation = true;

                            Action extraAction = new()
                            {
                                text = "Huidige reservering annuleren en voor deze tour inschrijven",
                                hasExtraBreak = false,
                                onAction = line =>
                                {
                                    changeReservations.moveReservation(tour, checkTour, reservation, ticketID);
                                }
                            };

                            actions = invalidReservation($"U heeft al een reservering staan ({checkTour.dateTime})", tour, extraAction, false);
                        }
                    }
                }

                if (hasReservation == false)
                {
                    foreach (var checkTour in tours)
                    {
                        if (checkTour.id == tour.id)
                        {
                            if (Tour.tourFreePlaces(tour) == 0)
                            {
                                //Just to dubble check
                                invalidReservation("Deze tour zit helaas al vol", tour);
                            }
                            else
                            {
                                //Add the booking/reservation to the current tour
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
                        text = $"Uw reservering is gelukt: ({tour.dateTime})",
                        hasExtraBreak = true,
                        textType = TextType.Success
                    },
                    new() {
                    text = $"Nog een reservering maken voor deze tour ({tour.dateTime})",
                    hasExtraBreak = false,
                    onAction = line => {
                      /*  if (!makeReservation.getUsersTicket(tour))
                        {
                            actions.Add(new()
                            {
                                text = "Dit ticket mag geen reservingen maken"
                            });
                        }*/
                    }
                    },
                    new() {
                    text = "Terug naar start",
                    hasExtraBreak = false,
                    onAction = line => {
                        ProgramManger.setActions(Program.getStartScreen());
                    }
                    }
                };
            }

        }
        else
        {
            actions = invalidReservation("Deze tour zit helaas al vol", tour, tryAgain: false);
        }
        return actions;

    }

    public static string getUsersTicket(Tour tour)
    {
        string ticket = "";
        ProgramManger.start(new List<Action> {
            new (){
                validRoles = new Role[] { Role.Bezoeker },
                text = "Scan uw ticket",
                onAction = line =>
                { ticket = line;
                 }
            },
        });
        return ticket;
        /*
                Console.WriteLine($"Scan nu uw ticket om deze tour te boeken ({tour.dateTime})");
                string ticketID = ProgramManger.readLine();
                if (ticketID != "")
                {
                    if (makeReservation.checkTicketValidity(ticketID))
                    {
                        makeReservation.ReserveTour(ticketID, tour);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }*/

    }
}
