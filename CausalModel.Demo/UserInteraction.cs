using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo;
public class UserInteraction
{
    private const string FACTS_FILE = "character-facts.json";
    public static string GetFileName()
    {
        Console.WriteLine($"Enter file name or press Enter to use or create {FACTS_FILE} (example)");
        string? fileName = Console.ReadLine();
        if (string.IsNullOrEmpty(fileName))
            fileName = FACTS_FILE;

        return fileName;
    }

    public static void PrintErrorMessage(string message, Exception? ex = null)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = prevColor;

        if (ex != null)
        {
            Console.WriteLine(ex);
        }
    }
}
