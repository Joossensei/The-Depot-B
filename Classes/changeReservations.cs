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
                foreach (Booking checkReservation in checkTour.bookings) {
                    if (checkReservation.userId == reservation.userId) {
                        checkReservation.occupationStatus = OccupationStatus.Canceled;
                    }
                }
            }

            tempTours.Add(checkTour);
        }
        var manager = new ReservationSystem.jsonManager();
        manager.writeToJson(tours, @"JsonFiles/tours.json");
    }

    public static void moveReservation(Tour newTour, Tour oldTour, List<Tour> tours) {

    }
};