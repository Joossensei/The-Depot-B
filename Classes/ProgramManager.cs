using Newtonsoft.Json;

public static class ProgramManger
{
    public static bool isActive = true;
    public static List<Action> actions = new();
    public static System.Action<string>? onOtherValue;
    public static List<string> errors = new();

    public static Role userRole = Role.Bezoeker;

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
        Console.ForegroundColor = ConsoleColor.White;
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
            if (action.onAction != null && action.validRoles.Contains(userRole))
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
            //Checking if you have the rights to have this role
            if (action.validRoles.Contains(userRole))
            {
                //Checking if the actions has an action id
                if (action.id != null)
                {
                    styleWrite(action.id + ": ", ConsoleColor.DarkBlue);
                }

                styleWriteLine(
                    action.text + (action.hasExtraBreak ? "\n" : ""), action.textType switch
                    {
                        TextType.Normal => ConsoleColor.White,
                        TextType.Error => ConsoleColor.Red,
                        TextType.Success => ConsoleColor.Green,
                        _ => ConsoleColor.White
                    }
                );
            }
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
        styleWriteLine("------------------------------------------------------------", ConsoleColor.DarkBlue);
    }

    private static void renderErrors()
    {
        if (errors.Any())
        {
            Console.WriteLine("");
            foreach (var error in errors)
            {
                styleWriteLine($"Error: {error}", ConsoleColor.Red);
            }
            errors.Clear();
        }
    }

    public static void styleWrite(string value, ConsoleColor color)
    {
        ConsoleColor oldColor = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.Write(value);
        Console.ForegroundColor = oldColor;
    }

    public static void styleWriteLine(string value, ConsoleColor color)
    {
        styleWrite(value + "\n", color);
    }
}

public class Action
{
    public string text = "";
    public bool hasExtraBreak;

    public System.Action<string>? onAction;

    public TextType textType = TextType.Normal;

    public int? id { get; private set; }

    public Role[] validRoles = new Role[] { Role.Afdelingshoofd, Role.Bezoeker, Role.Gids };

    public void setActionId(int id)
    {
        this.id = id;
    }
}

public enum TextType
{
    Normal,
    Error,
    Success,
}


public enum Role
{
    Bezoeker,
    Gids,
    Afdelingshoofd
}