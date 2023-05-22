namespace ReservationSystem;

//namespace startTour;

public class startTour
{
    public static void start(Tour tour)
    {
        int checkedInCount = checkedIn(tour);
        int totalBooked = tour.bookings.Count();
        List<Action> actions = new List<Action> { };

        //Show tour info
        actions.AddRange(
            new List<Action> {
                new(){
                    text = $"{tour.dateTime.ToString("HH:mm")} - {tour.dateTime.AddMinutes(tour.tourDuration).ToString("HH:mm")}",
                },new(){
                    text=$"{checkedInCount} ingechecked van de {totalBooked} totale boekingen",
                    hasExtraBreak=true
                }
            }
        );

        //If there are a low amount of reservations, add the ability to reserve more
        if (totalBooked < 6)
        {
            actions.Add(new()
            {

                text = "Extra reservering maken",

                onAction = line =>
                {
                    ProgramManger.setActions(new List<Action>()
                        {
                            new ()
                            {
                                text = "Scan uw unieke code om nu te boeken"
                            }
                        }, line =>
                        {
                            makeReservation.ReserveTour(line, tour, forCheckIn: true);
                        }
                    );
                }
            });
        }

        //Only show the action to checkin of not everyone has checked in
        if (checkedInCount < totalBooked)
        {
            actions.Add(new()
            {
                text = "Tickets scannen om in te checken",
                onAction = line =>
                {
                    scanTickets(tour);
                }
            });
        }


        actions.Add(
            new()
            {
                text = "Rondleiding starten",
                onAction = line =>
                {
                    if (checkedInCount < totalBooked || totalBooked == 0)
                    {
                        ProgramManger.setActions(
                            new List<Action> {
                                new()
                                {
                                    text = "Weet je zeker dat je de rondleiding wilt starten?",
                                    textType=TextType.Error
                                },
                                new()
                                {
                                    text = "Ja",
                                    onAction = s =>
                                    {
                                        tour.tourStarted = true;
                                        var manager = new jsonManager();
                                        manager.writeToJson(Program.tourstoday, @"JsonFiles/tours.json");

                                        //Let guide know the tour started and go back to homescreen
                                        ProgramManger.setActions(
                                            new List<Action> {
                                                new()
                                                {
                                                    text = "Rondleiding is gestart",
                                                    textType=TextType.Success
                                                }
                                            }, line =>
                                            {
                                                ProgramManger.setActions(Program.getStartScreen());
                                            }
                                        );
                                    }
                                },
                                new()
                                {
                                    text = "Nee",
                                    onAction = s => start(tour)
                                }
                            }, line =>
                            {
                                start(tour);
                            }
                        );
                    }
                    else
                    {
                        tour.tourStarted = true;
                        var manager = new jsonManager();
                        manager.writeToJson(Program.tourstoday, @"JsonFiles/tours.json");

                        //Let guide know the tour started and go back to homescreen
                        ProgramManger.setActions(
                            new List<Action> {
                                new()
                                {
                                    text = "Rondleiding is gestart",
                                    textType=TextType.Success
                                }
                            }, line =>
                            {
                                ProgramManger.setActions(Program.getStartScreen());
                            }
                        );
                    }
                }
            }
        );

        ProgramManger.setActions(actions);
    }

    public static int checkedIn(Tour tour)
    {
        int checkedInCount = 0;

        foreach (Booking booking in tour.bookings)
        {
            if (booking.occupationStatus == OccupationStatus.Visited)
            {
                checkedInCount++;
            }
        }

        return checkedInCount;
    }


    private static void scanTickets(Tour tour, bool succes = false)
    {

        List<Action> actions = new List<Action> { };

        if (succes)
        {
            actions.Add(
                new()
                {
                    text = "Ticket is ingecheckt",
                    textType = TextType.Success
                }
            );
        }
        else
        {
            actions.Add(
                new()
                {
                    text = "Ticket is niet ingecheckt",
                    textType = TextType.Error
                }
            );
        }


        actions.AddRange(new List<Action>{
            new(){
                text="Scan een ticket",
                hasExtraBreak=true
            },
            new()
            {
                text = "Terug naar menu",
                onAction = line =>
                {
                    start(tour);
                }
            }
        });


        ProgramManger.setActions(actions, line =>
        {
            bool valid = false;
            //Loop through booking in tour and check if this ticket has a valid reservation. If so, update to joined and write to json
            foreach (Booking booking in tour.bookings)
            {
                if (booking.userId == line && booking.occupationStatus == OccupationStatus.Joined)
                {
                    valid = true;
                    booking.occupationStatus = OccupationStatus.Visited;
                    var manager = new ReservationSystem.jsonManager();
                    manager.writeToJson(Program.tourstoday, @"JsonFiles/tours.json");
                    Console.Beep();
                }
            }
            //If all reservations have checked in, go back to menu, else: checkin again
            if (checkedIn(tour) == tour.bookings.Count())
            {
                start(tour);
            }
            else if (valid == true)
            {
                scanTickets(tour, true);
            }
            else
            {
                scanTickets(tour, false);
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