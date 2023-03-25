namespace ReservationSystem;

public class statisticScreen
{
    public static List<Action> getStatistics()
    {


        

        List<Action> actions = new(){
                new (){
                    text = "Voer een actie uit door het nummer voor de actie in te voeren.",
                    hasExtraBreak = true
                },
                new (){
                    text = $"Rondleidingen ({DateTime.Now.ToShortDateString()})",
                    hasExtraBreak = true
                }
            };



        //Add other statistics
        actions.AddRange(new List<Action>(){
                new (){ text = "\nBezoekers: 3242",},
                new (){ text = "Rondleiding boekingen: 120",},
                new (){ text = "Rondleiding aanwezigen: 112",},
                new (){ text = "Rondleiding afwezigen: 8",},
                new (){
                    text = "Annuleringen: 34",
                    hasExtraBreak = true,
                },
                new (){
                    text = "Statistieken periode",
                    onAction = line => {}
                },
                new (){
                    text = "Terug naar start",
                    onAction = line => {
                        ProgramManger.setActions(Program.getStartScreen());
                    }
                },
            });

        return actions;
    }
}