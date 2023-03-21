namespace ReservationSystem;

public class changeReservations
{
    public static void cancelReservation(Tour tour, Booking reservation, List<Tour> tours)
    {

        List<Tour> tempTours = new List<Tour> { };

        foreach (Tour checkTour in tours)
        {
            if (checkTour.id == tour.id)
            {
                checkTour.bookings.Remove(reservation);
            }

            tempTours.Add(checkTour);
        }
        var manager = new ReservationSystem.jsonManager();
        manager.writeToJson(tours, @"JsonFiles/tours.json");
    }
};