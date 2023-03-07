using Newtonsoft.Json;

public static class ProgramManger
{
    public static bool isActive = true;
    public static List<Action> actions = new();
    public static System.Action<string>? onOtherValue;
    public static List<string> errors = new();

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

        //The loop of the programm
        while (isActive)
        {
            renderLine();
            //Rendering the current actions
            renderActions(ProgramManger.actions);

            //Rendering errors if present.
            renderErrors();
            renderLine();

            //Reading a line
            var line = readLine();

            //Checking if the application has not exitted yet
            if (isActive)
            {
                //Validating the current action
                validateActions(line);
            }
        }
    }

    public static void setActions(List<Action> actions, System.Action<string>? onOtherValue = null)
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
        ProgramManger.onOtherValue = onOtherValue;
    }

    private static void renderActions(List<Action> actions)
    {
        //Looping through the actions and rendering them
        foreach (var action in actions)
        {
            if (action.id != null)
            {
                Console.Write(action.id + ": ");
            }

            Console.WriteLine(action.text + (action.hasExtraBreak ? "\n" : ""));
        }
    }

    private static void validateActions(string line)
    {
        foreach (var action in actions)
        {
            //Checking if an action has been selected
            if (action.id != null && line == action.id.ToString())
            {
                action.onAction(line);
                return;
            }
        }


        //If the on othervalue method is present call it with the inputed value else trough error
        if (onOtherValue != null)
        {
            onOtherValue(line);
        }
        else
        {
            errors.Add("Geen juiste actie geselecteerd");
        }
    }

    private static void renderLine()
    {
        Console.WriteLine("------------------------------------------------------------");
    }

    private static void renderErrors()
    {
        if (errors.Any())
        {
            Console.WriteLine("");
            foreach (var error in errors)
            {
                Console.WriteLine($"Error: {error}");
            }
            errors.Clear();
        }
    }

    public static T readJson<T>(string fileLocation)
    {
        string text = System.IO.File.ReadAllText(fileLocation);
        return JsonConvert.DeserializeObject<T>(text);
    }
}

public class Action
{
    public string text = "";
    public bool hasExtraBreak;

    public System.Action<string>? onAction;

    public int? id { get; private set; }

    public void setActionId(int id)
    {
        this.id = id;
    }
}