public class Tour
{
    public Guid id = Guid.NewGuid();
    public DateTime dateTime;

    public int tourDuration = 20;

    public bool tourStarted = false;

    public int maxBookingCount = 13;

    public List<Booking> bookings = new();

    //v1 write all tours
    /*public static void writeAllTours()
    {
        bool hasFoundToday = false;
        List<Tour> tours = ReservationSystem.jsonManager.LoadTours();
        DateTime today = DateTime.Now;
        today = new DateTime(today.Year, today.Month, today.Day);


        foreach (Tour tour in tours)
        {
            if (new DateTime(tour.dateTime.Year, tour.dateTime.Month, tour.dateTime.Day) == today)
            {
                {
                    hasFoundToday = true;
                    break;
                }
            }

            if (!hasFoundToday)
            {
                // Get the current date and time
                DateTime now = DateTime.Now;

                // Set the opening and closing times for the Depot
                DateTime openingTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
                DateTime closingTime = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0);

                // Set the tour duration to 20 minutes
                int tourDuration = 20;

                // Start time for the first tour
                DateTime startTime = openingTime;

                // Create tours until closing time

                while (startTime.AddMinutes(tourDuration) <= closingTime)
                {
                    // Create a new tour with the current start time
                    Tour tourToday = new Tour { dateTime = startTime };

                    // Add the tour to the list of tours
                    tours.Add(tourToday);

                    // Increment the start time for the next tour
                    startTime = startTime.AddMinutes(tourDuration);
                }
                // Write the list of tours to a JSON file
                var manager = new ReservationSystem.jsonManager();
                manager.writeToJson(tours, @"JsonFiles\tours.json");
            }
        }
    }*/
    public static void writeAllTours()
    {
        List<Tour> tours = ReservationSystem.jsonManager.LoadTours();
        DateTime today = DateTime.Now.Date;
        bool hasFoundToday = false;

        foreach (Tour tour in tours)
        {
            if (tour.dateTime.Date == today)
            {
                hasFoundToday = true;
                break;
            }
        }

        if (!hasFoundToday)
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Set the opening and closing times for the Depot
            DateTime openingTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
            DateTime closingTime = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0);

            // Set the tour duration to 20 minutes
            int tourDuration = 20;

            // Start time for the first tour
            DateTime startTime = openingTime;

            // Create tours until closing time
            while (startTime.AddMinutes(tourDuration) <= closingTime)
            {
                // Create a new tour with the current start time
                Tour tourToday = new Tour { dateTime = startTime };

                // Add the tour to the list of tours
                tours.Add(tourToday);

                // Increment the start time for the next tour
                startTime = startTime.AddMinutes(tourDuration);
            }

            // Write the list of tours to a JSON file
            var manager = new ReservationSystem.jsonManager();
            manager.writeToJson(tours, @"JsonFiles/tours.json");
        }
    }


    static public int tourFreePlaces(Tour tour)
    {
        // Ideally, this gets pulled from maxBookingCount
        int freePlaces = 13;

        foreach (var booking in tour.bookings)
        {
            if (booking.occupationStatus == OccupationStatus.Joined)
            {
                freePlaces--;
            }
        }

        return freePlaces;
    }

    static public int tourAmountBookings(Tour tour)
    {
        // Ideally, this gets pulled from maxBookingCount
        int placesTaken = 0;

        foreach (var booking in tour.bookings)
        {
            if (booking.occupationStatus == OccupationStatus.Joined || booking.occupationStatus == OccupationStatus.Visited)
            {
                placesTaken++;
            }
        }

        return placesTaken;
    }

}
public class Booking
{
    public string userId;
    public Guid tourId;
    public DateTime createData;
    public OccupationStatus occupationStatus = OccupationStatus.Joined;
    
}

public enum OccupationStatus
{
    Joined,
    Canceled,
    Visited
}

