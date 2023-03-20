namespace ReservationSystem;

class makeReservation
{

    public static void invalidReservation(string reason, Tour tour)
    {

        List<Action> actions = new List<Action>
        {

        };

        Console.WriteLine(reason);
        actions = new List<Action> {
            new() {
            text = "Nog eens proberen",
            hasExtraBreak = false,
            onAction = line => {
                makeReservation.ReserveTour(Console.ReadLine(), tour);
            }
            },
            new() {
            text = "Teruggaan naar hoofdmenu",
            hasExtraBreak = false,
            onAction = line => {
                //ProgramManger.setActions(getStartScreen());
            }
            }
        };

        ProgramManger.start(actions);
    }

    public static void ReserveTour(string ticketID, Tour tour)
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
            invalidReservation("Deze code is ongeldig. Probeer het opnieuw", tour);
        }
        else
        {

            //Als de code wel goed is, check dan of er nog plek is in deze tour

            List<Tour> tours = new List<Tour>();

            foreach (var checkTour in jsonManager.LoadTours())
            {
                if (checkTour.id == tour.id)
                {

                    if (checkTour.bookings.Count >= 13)
                    {
                        invalidReservation("Deze tour zit helaas al vol", tour);
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
                else
                { }


                tours.Add(checkTour);
                var manager = new ReservationSystem.jsonManager();
                manager.writeToJson(tours, @"JsonFiles/tours.json");

                actions = new List<Action> {
                    new() {
                    text = "Nog een reservatie",
                    hasExtraBreak = false,
                    onAction = line => {
                        makeReservation.ReserveTour(Console.ReadLine(), tour);
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

        ProgramManger.start(actions);
    }
}

