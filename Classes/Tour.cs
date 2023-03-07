public class Tour{
    public Guid id = Guid.NewGuid();
    public DateTime dateTime;

    public int tourDuration = 20;

    public int maxBookingCount = 13;

    public List<Booking> bookings = new();
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