using ReservationSystem;

public static class startTour {
    public static void selectTour()
    {
        // if(code == ""){
        List<Action> actions = new List<Action> {
            new() {
                text = "Selecteer welke rondleiding je wilt starten",
                hasExtraBreak = true
            },
            new() {
                text = $"Beschikbare rondleidingen ({DateTime.Now.ToShortDateString()})",
                hasExtraBreak = true
            }
        };
        // }
        // else{
        //     ProgramManger.errors.Add("Oops u heeft geen rechten om dit uit te voerren");
        // }
        };
        
        
        ProgramManger.start(actions);

        // foreach (Tour tour in getTour) {
        //     actions.Add(
        //         new() {
        //             text = "Selecteer welke rondleiding je wilt starten",
        //         }
        //     );
        // }
    }
    
    // Vragen of iedereen zn barcode scant

    // Checken of iedereen er is

    // Tour updaten naar started
}