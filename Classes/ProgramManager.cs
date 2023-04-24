using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ReservationSystem;
public static class ProgramManger
{

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

    const int VK_RETURN = 0x0D;
    const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    const uint KEYEVENTF_KEYUP = 0x0002;

    public static bool isActive = true;
    public static List<Action> actions = new();
    public static System.Action<string>? onOtherValue;
    public static List<string> errors = new();

    public static bool isPassword;

    public static Role userRole = Role.Bezoeker;

    static int openedPageCount = 0;


    public static string readLine()
    {
        string line = "";
        if (isPassword)
            line = readPassword();
        else
            line = Console.ReadLine() ?? "";
        if (line == "exit")
        {
            isActive = false;
            Console.WriteLine("Programma successvol gesloten.");
        }
        return line;


    }
    private static string readPassword()
    {
        string line = "";
        ConsoleKey key;
        do
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && line.Length > 0)
            {
                Console.Write("\b \b");
                line = line[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                line += keyInfo.KeyChar;
            }
        }
        // Stops Receving Keys Once Enter is Pressed
        while (key != ConsoleKey.Enter);
        Console.Write("\n");

        return line;
    }

    public static void delayedReturnToHome(int openedPageCount)
    {
        Task.Delay(new TimeSpan(0, 0, 5)).ContinueWith(task =>
        {

            if (openedPageCount == ProgramManger.openedPageCount)
            {
                Console.WriteLine(openedPageCount);
                Console.WriteLine(ProgramManger.openedPageCount);

                ProgramManger.setActions(Program.getStartScreen());

                userRole = Role.Bezoeker;
            }
        });
    }

    public static async void start(List<Action>? actions)
    {

        openedPageCount = 0;
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

            var line = readLine();

            //Checking if the application has not exitted yet
            if (isActive)
            {
                //Validating the current action
                validateActions(line);
            }
        }
    }

    public static void setActions(List<Action> actions, System.Action<string>? onOtherValue = null, bool isPassword = false)
    {
        //Adding an id to the actions that can run an action
        int currentId = 1;
        openedPageCount++;
        delayedReturnToHome(openedPageCount);
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
        ProgramManger.isPassword = isPassword;
    }


    public static void renderActions(List<Action> actions)
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