using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running;
public class TotalResult
{
    public long MinTimeMilliseconds { get; set; }
    public long MaxTimeMilliseconds { get; set; }
    public double AverageTimeMilliseconds { get; set; }

    public long TotalMilliseconds { get; set; }

    public Dictionary<string, Dictionary<string, float>>
        FactActualProbabilitiesByModelName { get; set; } = new();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"MinTimeMilliseconds: {MinTimeMilliseconds.ToString("N0")} ms");
        sb.AppendLine($"MaxTimeMilliseconds: {MaxTimeMilliseconds.ToString("N0")} ms");
        sb.AppendLine($"AverageTimeMilliseconds: {AverageTimeMilliseconds.ToString("F2")} ms");
        sb.AppendLine($"TotalMilliseconds: {TotalMilliseconds.ToString("N0")} ms");

        sb.AppendLine("FactActualProbabilitiesByModelName:");
        foreach (var modelEntry in FactActualProbabilitiesByModelName)
        {
            sb.AppendLine($"\tModel Name: {modelEntry.Key}");
            foreach (var factEntry in modelEntry.Value)
            {
                sb.AppendLine($"\t\tFact ID: {factEntry.Key}, Probability: {factEntry.Value}");
            }
        }

        return sb.ToString();
    }
}
