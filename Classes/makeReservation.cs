namespace ReservationSystem;

class makeReservation
{

    //General function if the reservation is invalid or fails to prevent duplicate code
    public static void invalidReservation(string reason, Tour tour, List<Tour> tours, Action extraAction = default)
    {

        Console.WriteLine(reason);
        List<Action> actions = new List<Action> {
            new() {
            text = "Nog eens proberen",
            hasExtraBreak = false,
            onAction = line => {
                makeReservation.ReserveTour(Console.ReadLine(), tour, tours);
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

        if (extraAction != default)
        {
            actions.Add(extraAction);
        }

        ProgramManger.start(actions);
    }

    public static void ReserveTour(string ticketID, Tour tour, List<Tour> tours)
    {

        if (Tour.tourFreePlaces(tour) > 0)
        {

            List<Action> actions = new List<Action>
            {

            };

            bool validTicket = false;

            List<string> entryTickets = jsonManager.LoadEntryTickets();
            foreach (var ticket in entryTickets)
            {
                if (ticket == ticketID)
                {
                    validTicket = true;
                }
            }

            if (validTicket == false)
            {
                invalidReservation("Deze code is ongeldig. Probeer het opnieuw", tour, tours);
            }
            else
            {

                //If the ticket is valid, check if the user already has a reservation gebruiker niet al een reservering heeft
                bool hasReservation = false;
                foreach (var checkTour in tours)
                {
                    foreach (var reservation in checkTour.bookings)
                    {
                        if ((reservation.userId == ticketID) && (reservation.occupationStatus == OccupationStatus.Joined))
                        {
                            hasReservation = true;

                            Action extraAction = new()
                            {
                                text = "Huidige reservering annuleren en voor deze tour inschrijven",
                                hasExtraBreak = false,
                                onAction = line =>
                                {
                                    changeReservations.moveReservation(tour, checkTour, reservation, tours, ticketID);
                                }
                            };

                            invalidReservation($"U heeft al een reservering staan ({checkTour.dateTime})", tour, tours, extraAction);
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
                                invalidReservation("Deze tour zit helaas al vol", tour, tours);
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
                        makeReservation.ReserveTour(Console.ReadLine(), tour, tours);
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
        }else {
            invalidReservation("Deze tour zit helaas al vol", tour, tours);
        }
    }
}

