using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReservationSystem
{
    public class Statistics
    {
        public static readonly List<Tour> Data = jsonManager.LoadTours();

        public static List<Action> getData()
        {
            int length = Data.Count;
            // make action
            List<Action> actions = new()
            {
                new()
                {
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new()
                {
                    text = $"Totaal aantal rondleidingen tot nu toe {length}",
                    hasExtraBreak = true
                }
            };

            actions.AddRange(new List<Action>
            {
                new()
                {
                    text = "Filteren op datum",
                    onAction = line =>
                    {
                        ProgramManger.setActions(RangeDatum());
                    }
                },
                new()
                {
                    text = "Terug naar start",
                    onAction = line => { ProgramManger.setActions(Program.getStartScreen()); }
                }
            });

            return actions;
        }

        private static List<Action> RangeDatum()
        {
            Console.WriteLine("Voer begin datum in als yyyy/mm/dd bv 2023/12/31\n");
            DateTime d1;
            while (!DateTime.TryParse(Console.ReadLine(), out d1))
            {
                Console.WriteLine("Ongeldige datum, probeer opnieuw.");
            }

            Console.WriteLine("Voer eind datum in als yyyy/mm/dd bv 2023/12/31\n");
            DateTime d2;
            while (!DateTime.TryParse(Console.ReadLine(), out d2))
            {
                Console.WriteLine("Ongeldige datum, probeer opnieuw.");
            }

            var result = Data.Where(c => c.dateTime >= d1 && c.dateTime <= d2);

            int totalBookings = 0;
            foreach (var d in result)
            {
                Console.WriteLine(d.dateTime);
            }

            foreach (var t in result)
            {
                totalBookings += t.bookings.Count();
                Console.WriteLine(totalBookings);
            }

            List<Action> actions = new()
            {
                new() {text = "Voer een actie uit door het nummer voor de actie in te voeren.", hasExtraBreak = true},
                new() {text = $"Totaal aantal rondleidingen tot nu toe {Data.Count}", hasExtraBreak = true},
                new() {text = "Terug naar start", onAction = line => { ProgramManger.setActions(Program.getStartScreen()); }}
            };

            return actions;
        }
    }
}
