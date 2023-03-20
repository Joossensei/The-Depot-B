namespace ReservationSystem;

class makeReservation
{
    public static void ReserveTour(string ticketID, Tour tour)
    {
        List<Action> actions = new List<Action> {

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

            //Als de code niet in de beschikbare codes zit, laat de gebruiker het opnieuw proberen

            Console.WriteLine("Deze code is niet valide. Probeer het opnieuw");
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
        }
        else
        {

            //Als de code wel goed is, check dan of er nog plek is in deze tour
            Console.WriteLine(tour.id);
            foreach (var checkTour in jsonManager.LoadTours())
            {
                if (checkTour.id == tour.id) {
                    Console.WriteLine(tour.id);
                }
            }
        }

        ProgramManger.start(actions);
    }
}

