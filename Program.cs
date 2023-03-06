using Newtonsoft.Json;
using System.Globalization;

namespace ReservationSystem
{
    class Program
    {
        //Some test data for the tours
        static Tour[] tours = {
            new(){
                dateTime = DateTime.Now
            },
            new(){
                dateTime = DateTime.Now.AddMinutes(20)
            },
            new(){
                dateTime = DateTime.Now.AddMinutes(40)
            }
        };

        static void Main(string[] args)
        {
            ProgramManger.start(getStartScreen());
        }


        //Function to get the home screen elements the start screen
        static List<Action> getStartScreen()
        {
            List<Action> actions = new List<Action>{
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new (){
                    text = "Registratie controleren",
                    onAction = line => {
                        ProgramManger.setActions(new());
                    }
                },
                new (){
                    text = "Statistieken inzien",
                    hasExtraBreak = true,
                    onAction = line => {}
                },
                new (){
                    text = $"Beschikbare rondleidingen ({DateTime.Now.ToShortDateString()})",
                    hasExtraBreak = true
                }
            };

            //Adding the tours
            actions.AddRange(getTours());

            return actions;
        }

        static List<Action> getTours()
        {
            List<Action> actions = new();

            foreach (var tour in tours)
            {
                //Getting the free places from the tour and checking if it is full
                int freePlaces = tour.maxBookingCount - tour.bookings.Count;
                bool isFull = freePlaces == 0;

                //Adding the action items
                actions.Add(
                    new()
                    {
                        text = $"{tour.dateTime.ToShortTimeString()} ({(isFull ? "Volgeboekt" : $"{freePlaces} van de {tour.maxBookingCount} plaatsen vrij")})",
                        onAction = line => { }
                    }
                );
            }

            return actions;
        }
    }
}