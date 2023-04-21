namespace ReservationSystem;

public class statisticScreen
{
    public static List<Action> getStatistics()
    {
        List<Tour> tours = Program.tours;
        List<Action> actions = new() { };

        // Dictionary<Tour (The first tour), Tour (The tour that is supposed be merged with the first Tour)>
        // This is used to keep track of all tours with recommendations
        var emtpyTours = new Dictionary<Tour, Tour> { };
        var fullTours = new Dictionary<Tour, Tour> { };

        bool skipNext = false;

        //Tours.count - 1, because you cant calucate the next tour if there is no next tour
        for (int i = 0; i < (tours.Count - 1); i++)
        {
            /* skipNext is set when 1 tour and the next tour both are empty or full to prevent the next tour being processed again 
            tours[i + 1].dateTime.ToString("HH:mm") == "11:00" checks if the next tour is the next time to prevent tours over multiple days being merged */
            if (skipNext == true || tours[i + 1].dateTime.ToString("HH:mm") == "11:00")
            {
                skipNext = false;
                continue;
            }

            int minimumFreePlaces = (tours[i].maxBookingCount - statisticValues.minReservationsThreshold);
            int maximumFreePlaces = (tours[i].maxBookingCount - statisticValues.maxReservationsThreshold);

            if ((Tour.tourFreePlaces(tours[i]) > minimumFreePlaces) && (Tour.tourFreePlaces(tours[i + 1]) > minimumFreePlaces))
            {
                emtpyTours.Add(tours[i], tours[i + 1]);
                skipNext = true;

            }
            else if ((Tour.tourFreePlaces(tours[i]) <= maximumFreePlaces) && (Tour.tourFreePlaces(tours[i + 1]) <= maximumFreePlaces))
            {
                fullTours.Add(tours[i], tours[i + 1]);
                skipNext = true;
            }
        }


        //This foreach is not placed in the (else)if above, so it is possible to seperate the tour merges and extra tours
        if (emtpyTours.Count > 0)
        {
            actions.Add(new()
            {
                text = "De volgende rondleidingen kunnen worden samengevoegd, want deze hebben allemaal weinig aanmeldingen:"
            });
            foreach (KeyValuePair<Tour, Tour> entry in emtpyTours)
            {
                Tour firstTour = entry.Key;
                Tour secondTour = entry.Value;

                actions.Add(new()
                {
                    text = $"De rondleiding {firstTour.dateTime.ToString("dddd HH:mm")} en {secondTour.dateTime.ToString("HH:mm")} samenvoegen",
                });
            }
        }


        if (fullTours.Count > 0)
        {
            actions.Add(new()
            {
                text = "Tussen de volgende rondleidingen kan er een extra worden toegevoegd, want deze hebben allemaal veel aanmeldingen:"
            });
            foreach (KeyValuePair<Tour, Tour> entry in fullTours)
            {
                Tour firstTour = entry.Key;
                Tour secondTour = entry.Value;

                actions.Add(new()
                {
                    text = $"Extra rondleiding tussen {firstTour.dateTime.ToString("dddd HH:mm")} en {secondTour.dateTime.ToString("HH:mm")}",
                });
            }
        }


        actions.AddRange(new List<Action>(){
                new() {
                    //Adds extra line after advice
                },
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
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