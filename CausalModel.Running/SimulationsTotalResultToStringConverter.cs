using CausalModel.Common;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running
{
    internal class SimulationsTotalResultToStringConverter<TFactValue>
        where TFactValue : class
    {
        private readonly SimulationsTotalResult result;
        private readonly FactProvider<TFactValue> factProvider;

        public SimulationsTotalResultToStringConverter(
            SimulationsTotalResult result,
            FactProvider<TFactValue> factProvider)
        {
            this.result = result;
            this.factProvider = factProvider;
        }

        public string Convert()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CreateHeader("Simulation info"));
            sb.AppendLine($"Simulations count: {result.SimulationsCount.ToString("N0")}");
            sb.AppendLine($"Min time: {result.MinTimeMilliseconds.ToString("N0")} ms");
            sb.AppendLine($"Max time: {result.MaxTimeMilliseconds.ToString("N0")} ms");
            sb.AppendLine($"Average time: {result.AverageTimeMilliseconds.ToString("F2")} ms");
            sb.AppendLine($"Total time: {result.TotalMilliseconds.ToString("N0")} ms");

            sb.Append("\n");
            sb.Append(CreateHeader("Actual probabilities", fullScreen: false));
            //sb.AppendLine("Actual probabilities:");
            foreach (var (modelName, factsAndProbabilities) in
                result.FactActualProbabilitiesByModelName)
            {
                sb.AppendLine($"\tModel Name: {modelName}");

                // Sort facts by probability and color code them
                var sortedFacts = factsAndProbabilities.ToList();
                sortedFacts.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

                foreach (var (factId, probability) in sortedFacts)
                {
                    sb.Append("\t\t");
                    sb.AppendLine(GetFactProbabilityLine(modelName, factId, probability));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GetFactName(Fact<TFactValue> fact)
        {
            const int charsCountToDisplay = 25;

            string res = "";

            if (fact.FactValue != null)
            {
                string factValue = fact.FactValue.ToString() ?? "";

                if (factValue.Length > charsCountToDisplay)
                {
                    res += string.Join("", factValue.Take(charsCountToDisplay - 3))
                        + "...";
                }
                else
                    res += factValue;
            }
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
                int paddingLength = 2; // устанавливаем отступы слева и справа
                sb.AppendLine(new string('-', header.Length + 2 * paddingLength));
                sb.AppendLine(header.PadLeft(paddingLength + header.Length)
                    .PadRight(header.Length + 2 * paddingLength));
                sb.AppendLine(new string('-', header.Length + 2 * paddingLength));
            }

            return sb.ToString();
        }



        private string GetFactProbabilityLine(string modelName,
            string factId, double probability)
        {
            var probabilityColor = GetProbabilityColor(probability);

            var fact = factProvider.GetFact(modelName, factId);

            return $"{probabilityColor}" +
                $"{SimulationsTotalResultToStringConverter<TFactValue>.GetFactName(fact)}" +
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
