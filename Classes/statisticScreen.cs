namespace ReservationSystem;

public class statisticScreen
{
    public static List<Action> getStatistics()
    {
        List<Tour> tours = Program.tours;
        List<Action> actions = new() { };
        /*
        foreach (Tour tour in tours)
        {
            int totalBooked = 0;
            int totalCancelled = 0;
            int totalJoined = 0;
            int totalVisited = 0;

            foreach (Booking reservation in tour.bookings)
            {
                totalBooked++;

                switch (reservation.occupationStatus)
                {
                    case OccupationStatus.Canceled:
                        totalCancelled++;
                        break;
                    case OccupationStatus.Joined:
                        totalJoined++;
                        break;
                    case OccupationStatus.Visited:
                        totalVisited++;
                        break;
                }
            }
            if (totalBooked <= 3) {
                actions.Add(new() {
                    text = $"De tour om {tour.dateTime} is maar {totalBooked}x geboekt"
                });
            }

            if (totalCancelled >= 5) {
                actions.Add(new() {
                    text = $"De tour om {tour.dateTime} is {totalCancelled}x geanulleerd"
                });
            }

            if (totalJoined >= 8 && totalVisited <= 4) {
                actions.Add(new() {
                    text = $"De tour om {tour.dateTime} is {totalJoined}x gereserveerd en slechts {totalVisited}x bezocht"
                });
            }
        };

        int totalVisitors = jsonManager.LoadEntryTickets().Count();
        int totalReservations = 0;
        int cancelledReservations = 0;
        int joinedReservations = 0;
        int visitedReservations = 0;/*
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
        }*/

        //Tours.count - 1, because you cant calucate the next tour if there is no next tour
        bool skipNext = false;

        for (int i = 0; i < (tours.Count - 1); i++)
        {
            if (skipNext == true)
            {
                skipNext = false;
                continue;
            }

            Tour currentTour = tours[i];

            if (Tour.tourFreePlaces(currentTour) >= 9 && Tour.tourFreePlaces(tours[i+1]) >= 9)
            {

                actions.Add(new()
                {
                    text = $"De tour van {currentTour.dateTime} kan worden samengevoegd met {tours[i + 1].dateTime}",
                });
                skipNext = true;

            }
        }

        actions.AddRange(new List<Action>(){
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new (){
                    text = $"Rondleidingen ({DateTime.Now.ToShortDateString()})",
                    hasExtraBreak = true
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