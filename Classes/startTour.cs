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
                        if (checkCode(line, tour.bookings) /*&& count < tour.maxBookingCount*/) {
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
            if (booking.)
            {
                
            }
        }
        if (bookings.Contains(code))
        {
            return true;
        }

        return false;
    }
    
    // Vragen of iedereen zn barcode scant

    // Checken of iedereen er is

    // Tour updaten naar started
}