using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo.Utils;
public class WriteUtils
{
    public static void WriteDefault(string message)
    {
        WriteColoredText(message, ConsoleColor.Gray);
    }

    /// <summary>
    /// The same as WriteDefault
    /// </summary>
    public static void Write(string message)
    {
        WriteDefault(message);
    }

    public static void Line()
    {
        Console.WriteLine();
    }   

    public static void WriteMain(string message)
    {
        WriteColoredText(message, ConsoleColor.Green);
    }

    public static void WriteSubtle(string message)
    {
        WriteColoredText(message, ConsoleColor.DarkGray);
    }

    public static void WriteError(string message, Exception? ex = null)
    {
        WriteColoredText(message, ConsoleColor.Red);

        if (ex != null)
        {
            Console.WriteLine(ex);
        }
    }

    public static void WriteColoredText(string str, ConsoleColor color)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(str);
        Console.ForegroundColor = prevColor;
    }
}
