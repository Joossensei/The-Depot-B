using ReservationSystem;

namespace startTour;

public class startTour
{
    private int count = 0;
    public static void start(Tour tour)
    {
        // count += 1;
        ProgramManger.setActions(
            new () {
                new () {
                    text = "Voer uw unieke code in",
                    onAction = line => {
                        if (checkCode(line, tour.bookings)) {
                            start(tour);
                        }
                    }
                }
            });
    }

    private static bool checkCode(string code, List<Booking> bookings)
    {
        foreach (var booking in bookings)
        {
            if (booking.userId == code && booking.occupationStatus == OccupationStatus.Joined)
            {
                switch (booking.occupationStatus)
                {
                    case OccupationStatus.Joined:
                    {
                        booking.occupationStatus = OccupationStatus.Visited;
                        return true;
                    }
                    case OccupationStatus.Canceled:
                    {
                        Console.WriteLine("U heeft helaas de boeking gecancelled hierdoor kan u niet starten!");
                        return false;
                    }
                    case OccupationStatus.Visited:
                    {
                        Console.WriteLine("U heeft deze rondleiding al bezocht!");
                        return false;
                    }
                }
                
            }
        }
        
        return false;
    }
}