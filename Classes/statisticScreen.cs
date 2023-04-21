namespace ReservationSystem;

public class statisticScreen
{
    public static List<Action> getStatistics()
    {
        List<Tour> tours = Program.tours;
        List<Action> actions = new() { };

        bool skipNext = false;

        //Tours.count - 1, because you cant calucate the next tour if there is no next tour
        for (int i = 0; i < (tours.Count - 1); i++)
        {
            if (skipNext == true || tours[i+1].dateTime.ToString("HH:mm") == "11:00")
            {
                skipNext = false;
                continue;
            }

            Tour currentTour = tours[i];

            int minimumFreePlaces = (currentTour.maxBookingCount - statisticValues.minReservations);

            if (Tour.tourFreePlaces(currentTour) >=  minimumFreePlaces && Tour.tourFreePlaces(tours[i + 1]) >=  minimumFreePlaces)
            {

                actions.Add(new()
                {
                    text = $"De tour op {currentTour.dateTime.ToString("dddd HH:mm")} samenvoegen met {tours[i + 1].dateTime.ToString("HH:mm")} i.v.m weinig aanmeldingen (> {minimumFreePlaces})",
                });
                skipNext = true;

            }else if(1 == 1){

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