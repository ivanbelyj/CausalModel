using CausalModel.Common.DataProviders;
using CausalModel.Facts;
using CausalModel.Running.Extensions;
using CausalModel.Running.ProbabilityEstimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Models.Converters
{
    internal class ProbabilityEstimationResultToStringConverter<TFactValue>
        where TFactValue : class
    {
        private readonly ProbabilityEstimationResult<TFactValue> result;

        public ProbabilityEstimationResultToStringConverter(
            ProbabilityEstimationResult<TFactValue> result)
        {
            this.result = result;
        }

        public string Convert()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CreateHeader("Simulation info"));
            sb.AppendLine($"Simulations count: {result.SimulationsCount:N0}");
            sb.AppendLine($"Min time: {result.MinTimeNanoseconds:N0} ns ({result.MinTimeMilliseconds} ms)");
            sb.AppendLine($"Max time: {result.MaxTimeNanoseconds:N0} ns ({result.MaxTimeMilliseconds} ms)");
            sb.AppendLine($"Average time: {result.AverageTimeNanoseconds:N0} ns ({result.AverageTimeMilliseconds} ms)");
            sb.AppendLine($"Total time: {result.TotalMilliseconds:N0} ms");

            sb.Append("\n");
            sb.Append(CreateHeader("Actual probabilities", fullScreen: false));
            //sb.AppendLine("Actual probabilities:");
            foreach (var (modelName, factsInfo) in result.FactsInfoByModelName)
            {
                sb.AppendLine($"\nModel Name: {modelName}\n");

                // Sort facts by probability and color code them
                var sortedFacts = factsInfo.ToList();
                sortedFacts.Sort((pair1, pair2) => pair2.Value.EstimatedProbability.CompareTo(pair1.Value.EstimatedProbability));

                foreach (var (factId, factInfo) in sortedFacts)
                {
                    sb.Append("\t");
                    sb.AppendLine(GetFactProbabilityLine(modelName, factId, factInfo.FactValue, factInfo.EstimatedProbability));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GetFactName(TFactValue factValue)
        {
            const int charsCountToDisplay = 25;

            string res = "";

            var factValueString = factValue.ToString();

            if (factValueString.Length > charsCountToDisplay)
            {
                res += string.Join("", factValueString.Take(charsCountToDisplay - 3))
                    + "...";
            }
            else
                res += factValueString;
            return res.PadRight(charsCountToDisplay);
        }

        public string CreateHeader(string header, bool fullScreen = false)
        {
            StringBuilder sb = new StringBuilder();

            if (fullScreen)
            {
                int paddingLength = (Console.WindowWidth - header.Length) / 2;
                sb.AppendLine(new string('-', Console.WindowWidth));
                sb.AppendLine(header.PadLeft(paddingLength + header.Length));
                sb.AppendLine(new string('-', Console.WindowWidth));
            }
            else
            {
                int paddingLength = 2; // padding left and right
                sb.AppendLine(new string('-', header.Length + 2 * paddingLength));
                sb.AppendLine(header
                    .PadLeft(paddingLength + header.Length)
                    .PadRight(header.Length + 2 * paddingLength));
                sb.AppendLine(new string('-', header.Length + 2 * paddingLength));
            }

            return sb.ToString();
        }



        private string GetFactProbabilityLine(
            string modelName,
            string factId,
            TFactValue factValue,
            double probability)
        {
            var probabilityColor = GetProbabilityColor(probability);

            return $"{probabilityColor}" +
                $"{ProbabilityEstimationResultToStringConverter<TFactValue>.GetFactName(factValue)}" +
                $"\tp: {probability:F3}" +
                $"\t\u001b[90mId: {factId}\u001b[0m";
        }

        private static string GetProbabilityColor(double probability)
        {
            if (probability == 1)
            {
                // \x1b[32m
                return "\x1b[92m"; // Green
            }
            else if (probability < 0.01)
            {
                return "\x1b[31m"; // Red
            }
            else
            {
                return "\x1b[0m"; // Reset
            }
        }
    }
}
