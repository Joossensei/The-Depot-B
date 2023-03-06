using Newtonsoft.Json;

public static class ProgramManger
{
    public static bool isActive = true;
    public static List<Action> actions = new List<Action>();
    public static string readLine()
    {
        string line = Console.ReadLine() ?? "";

        if (line == "exit")
        {
            isActive = false;
            Console.WriteLine("Programma successvol gesloten.");
        }

        return line;
    }

    public static void start(List<Action>? actions)
    {
        //Adding start options
        if (actions != null)
        {
            setActions(actions);
        }

        //The loop od the programm
        while (isActive)
        {
            //Rendering the current actions
            renderActions(ProgramManger.actions);
            //Reading a line
            var line = readLine();

            //Checking if the application has not exitted yet
            if(isActive){
                //Validating the current action
                validateActions(line);
            }
        }
    }

    public static void setActions(List<Action> actions)
    {
        //Adding an id to the actions that can run an action
        int currentId = 1;
        foreach (var action in actions)
        {
            //Checking id the action has an action function
            if (action.onAction != null)
            {
                //Setting the id
                action.setActionId(currentId);
                currentId++;
            }
        }
        ProgramManger.actions = actions;
    }

    private static void renderActions(List<Action> actions)
    {
        //Looping through the actions and rendering them
        renderLine();
        foreach (var action in actions)
        {
            if(action.id != null){
                Console.Write(action.id + ": ");
            }

            Console.WriteLine(action.text + (action.hasExtraBreak ? "\n" : ""));
        }
        renderLine();
    }

    private static void validateActions(string line){
        foreach (var action in actions)
        {
            //Checking if an action has been selected
            if(action.id != null && line == action.id.ToString()){
                action.onAction(line);
                return;
            }
        }

        //If no action has been selected display 
        Console.WriteLine("Geen juiste actie geselecteerd");
    }

    private static void renderLine(){
        Console.WriteLine("----------------------------------------------");
    }

    public static T readJson<T>(string fileLocation){
        string text = System.IO.File.ReadAllText(fileLocation);
        return JsonConvert.DeserializeObject<T>(text);
    }
}

public class Action
{
    public string text = "";
    public bool hasExtraBreak;

    public System.Action<string> onAction;

    public int? id { get; private set; }

    public void setActionId(int id)
    {
        this.id = id;
    }
}