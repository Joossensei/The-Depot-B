namespace ReservationSystem;

public class statisticScreen
{
    public static List<Action> getStatistics()
    {
        List<Tour> tours = Program.tours;
        int totalVisitors = jsonManager.LoadEntryTickets().Count();
        int totalReservations = 0;
        int cancelledReservations = 0;
        int joinedReservations = 0;
        int visitedReservations = 0;
        foreach (Tour tour in tours)
        {
            foreach (Booking reservation in tour.bookings)
            {
                totalReservations++;
                switch (reservation.occupationStatus)
                {
                    case OccupationStatus.Joined:
                        joinedReservations++;
                        break;
                    case OccupationStatus.Canceled:
                        cancelledReservations++;
                        Console.WriteLine(reservation.userId);
                        break;
                    case OccupationStatus.Visited:
                        joinedReservations++;
                        break;
                }
            }
        }

        List<Action> actions = new(){
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new (){
                    text = $"Rondleidingen ({DateTime.Now.ToShortDateString()})",
                    hasExtraBreak = true
                }
            };



        //Add other statistics
        actions.AddRange(new List<Action>(){
                new (){ text = $"\nBezoekers: {totalVisitors}",},
                new (){ text = $"Rondleiding boekingen: {totalReservations}",},
                new (){ text = $"Rondleiding aanwezigen: {visitedReservations}",},
                new (){ text = $"Rondleiding afwezigen: {joinedReservations}",},
                new (){
                    text = $"Annuleringen: {cancelledReservations}",
                    hasExtraBreak = true,
                },
                new (){
                    text = "Statistieken periode",
                    onAction = line => {}
                },
                new (){
                    text = "Terug naar start",
                    onAction = line => {
                        ProgramManger.setActions(Program.getStartScreen());
                    }
                },
            });

        return actions;
    }
}