using ReservationSystem;

namespace startTour;

public static class startTour {
    public static void start(Tour tour)
    {
        ProgramManger.setActions(new {
            new Action() {
                text = "",
                onAction = line => {
                    if (tour.bookings.Contains()){
                        
                    }
                }
            },
        });
    }
    
    // Vragen of iedereen zn barcode scant

    // Checken of iedereen er is

    // Tour updaten naar started
}