using ReservationSystem;

namespace startTour;

public class startTour
{
    public static void start(Tour tour, int amntStarted)
    {
        List<Tour> tours = Program.tours;
        int placesBooked = Tour.tourAmountBookings(tour);

        ProgramManger.setActions(new List<Action>
        {
            new()
            {
                text = "Code scannen",
                onAction = line =>
                {
                    ProgramManger.setActions(new List<Action>()
                    {
                        new ()
                        {
                            text = "Scan uw unieke code"
                        }
                    }, line => {
                        if (checkCode(line, tour.bookings))
                        {
                            Console.WriteLine("U bent aangemeld voor de rondleiding");
                            if (amntStarted >= tour.maxBookingCount)
                            {
                                ProgramManger.setActions(new List<Action>
                                {
                                    new()
                                    {
                                        text = "Wilt u de tour starten? (Scan uw code)",
                                        onAction = line =>
                                        {
                                            if (line == "gids")
                                            {
                                                Console.WriteLine("Tour is gestart!");
                                                ProgramManger.setActions(Program.getStartScreen());
                                            }
                                            else
                                            {
                                                start(tour, amntStarted);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                amntStarted += 1;
                                
                                Console.WriteLine($"{amntStarted} van de {placesBooked} reserveringen zijn aangemeld.");
                                start(tour, amntStarted);
                            }
                        } else {
                            Console.WriteLine($"Sorry ingevulde code is niet gereserveerd voor deze tour");
                            start(tour, amntStarted);
                        }
                    });
                }
            },
            new()
            {
                text = "Wilt u de tour starten?",
                onAction = line =>
                {
                    ProgramManger.setActions(new List<Action>()
                    {
                        new ()
                        {
                            text = "Scan uw unieke code",
                        }
                    }, (line) =>
                    {
                        if (line == "gids")
                        {

                            tour.tourStarted= true;
                            var manager = new jsonManager();
                            manager.writeToJson(tours, @"JsonFiles/tours.json");

                            ProgramManger.setActions(new List<Action>()
                            {
                                new()
                                {
                                    text = "Tour is gestart! \n   Terug naar start",
                                    onAction = line =>
                                    {
                                        ProgramManger.setActions(Program.getStartScreen());

                                    }
                                }
                            });
                        }
                        else
                        {
                            start(tour, amntStarted);
                        }    
                    });
                }
            }
        });
    }

    private static bool checkCode(string code, List<Booking> bookings)
    {
        List<Tour> tours = Program.tours;

        foreach (var booking in bookings.Where(booking => booking.userId == code && booking.occupationStatus == OccupationStatus.Joined))
        {
            switch (booking.occupationStatus)
            {
                case OccupationStatus.Joined:
                {
                    booking.occupationStatus = OccupationStatus.Visited;
                    var manager = new jsonManager();
                    manager.writeToJson(tours, @"JsonFiles/tours.json");
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
                default:
                    Console.WriteLine("Er ging iets fout probeer het nog een keer!");
                    return false;
            }
        }

        return false;
    }
}