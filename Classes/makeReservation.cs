namespace ReservationSystem;

class makeReservation
{
    public static void ReserveTour(string ticketID, Tour tour)
    {
        Console.WriteLine(jsonManager.LoadEntryTickets());

        List<Action> actions = new List<Action> {
            new() {
            text = "Nog een reservatie maken",
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
}

