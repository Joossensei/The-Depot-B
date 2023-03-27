namespace ReservationSystem;

public class changeReservations
{
    public static void cancelReservation(Tour tour, List<Tour> tours, Booking reservation, bool showOptions = false)
    {

        List<Tour> tempTours = new List<Tour> { };

        foreach (Tour checkTour in tours)
        {
            if (checkTour.id == tour.id)
            {
                foreach (Booking checkReservation in checkTour.bookings)
                {
                    if (checkReservation.userId == reservation.userId)
                    {
                        checkReservation.occupationStatus = OccupationStatus.Canceled;
                    }
                }
            }

            tempTours.Add(checkTour);
        }

        var manager = new ReservationSystem.jsonManager();
        manager.writeToJson(tours, @"JsonFiles/tours.json");



        if (showOptions == true)
        {
            List<Action> actions = new List<Action> {
                new() {
                    text = "U heeft de reservering succesvol geanuleerd",
                    hasExtraBreak = true,
                    },
                    new() {
                    text = "Nog een annulering maken",
                    hasExtraBreak = false,
                    onAction = line => {
                        //changeReservations.cancelReservation(tour: tour, tours: tours);
                    }
                    },
                    new() {
                    text = "Terug naar start",
                    hasExtraBreak = false,
                    onAction = line => {
                        ProgramManger.setActions(Program.getStartScreen());
                    }
                    }
                };
            ProgramManger.start(actions);
        };
    }

    public static void moveReservation(Tour newTour, Tour oldTour, Booking reservation, List<Tour> tours, string ticketID)
    {

        //First we need to cancel the current reservation
        cancelReservation(oldTour, tours, reservation);
        //Then we easily make a new one
<<<<<<< Updated upstream
        makeReservation.ReserveTour(ticketID, newTour, tours);
=======
        makeReservation.ReserveTour(ticketID, newTour);
        
>>>>>>> Stashed changes

    }
};