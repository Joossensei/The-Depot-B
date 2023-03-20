public class Tour{
    public Guid id = Guid.NewGuid();
    public DateTime dateTime;

    public int tourDuration = 20;

    public int maxBookingCount = 13;

    public List<Booking> bookings = new();

    public static void  writeAllTours(){
      
        // Get the current date and time
        DateTime now = DateTime.Now;

        // Set the opening and closing times for the Depot
        DateTime openingTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
        DateTime closingTime = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0);

        // Create a list to store the tours
        List<Tour> tours = new List<Tour>();

        // Set the tour duration to 20 minutes
        int tourDuration = 20;

        // Start time for the first tour
        DateTime startTime = openingTime;

        // Create tours until closing time

        while (startTime.AddMinutes(tourDuration) <= closingTime)
        {
            // Create a new tour with the current start time
            Tour tour = new Tour { dateTime = startTime };

            // Add the tour to the list of tours
            tours.Add(tour);

            // Increment the start time for the next tour
            startTime = startTime.AddMinutes(tourDuration);
        }
        // Write the list of tours to a JSON file
        var manager = new ReservationSystem.jsonManager();
        manager.writeToJson(tours, @"JsonFiles\tours.json");
        
    }
}

public class Booking{
    public Guid userId;
    public Guid tourId;
    public DateTime createData;
    public OccupationStatus occupationStatus = OccupationStatus.Joined;
}

public enum OccupationStatus{
    Joined,
    Canceled,
    Visited
}
    	
           