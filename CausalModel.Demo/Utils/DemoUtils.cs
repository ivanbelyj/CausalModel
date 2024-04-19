using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CausalModel.Demo.Utils.WriteUtils;

namespace CausalModel.Demo.Utils;
internal static class DemoUtils
{
    public static void WriteFactFixated(
        object sender,
        InstanceFact<string> fixatedFact,
        bool isOccurred)
    {
        if (isOccurred)
        {
            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write((fixatedFact.Fact.IsRootCause() ? "" : "\t")
                + fixatedFact.Fact.FactValue);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" ({fixatedFact.InstanceFactId})");

            Console.ForegroundColor = prevColor;
        }
    }

    public static SourceData GetSourceData(string fileName)
    {
        try
        {
            return SourceDataUtils.GetFromFileOrDefault(fileName);
        }
        catch (Exception ex)
        {
            WriteError(
                "An error occured during the model opening.", ex);

            Console.ReadKey(false);

            throw;
        }
    }
}
