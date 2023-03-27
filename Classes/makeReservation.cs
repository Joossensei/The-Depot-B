namespace ReservationSystem;

class makeReservation
{

    //General function if the reservation is invalid or fails to prevent duplicate code
    private static void invalidReservation(string reason, Tour tour, Action extraAction = default, bool tryAgain = true)
    {
        List<Tour> tours = Program.tours;

        List<Action> actions = new List<Action> { };
        actions.Add(new() {
            text = reason
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

        ProgramManger.start(actions);
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

    public static void ReserveTour(string ticketID, Tour tour)
    {
        List<Tour> tours = Program.tours;
        if (Tour.tourFreePlaces(tour) > 0)
        {

            List<Action> actions = new List<Action>
            {

            };

            bool validTicket = checkTicketValidity(ticketID);

            if (validTicket == false)
            {
                invalidReservation("Deze code is ongeldig. Probeer het opnieuw", tour);
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
                            invalidReservation($"U heeft al een reservering voor deze rondleiding", tour, tryAgain: false);
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

                            invalidReservation($"U heeft al een reservering staan ({checkTour.dateTime})", tour, extraAction, false);
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
                    text = $"Nog een reservering maken voor deze tour ({tour.dateTime})",
                    hasExtraBreak = false,
                    onAction = line => {
                        if (!makeReservation.getUsersTicketAndMakeReservation(tour))
                        {
                            actions.Add(new()
                            {
                                text = "Dit ticket mag geen reservingen maken"
                            });
                        }
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

            ProgramManger.start(actions);
        }
        else
        {
            invalidReservation("Deze tour zit helaas al vol", tour, tryAgain: false);
        }
    }

    public static bool getUsersTicketAndMakeReservation(Tour tour)
    {

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

    }

}
