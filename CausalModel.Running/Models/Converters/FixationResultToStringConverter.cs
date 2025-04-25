using CausalModel.Common.DataProviders;
using CausalModel.Model.Instance;
using CausalModel.Running.Models.Common;
using CausalModel.Running.Models.FixationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CausalModel.Running.Models.Converters
{
    internal class FixationResultToStringConverter<TFactValue>
        where TFactValue : class
    {
        private readonly FixationResult<TFactValue> result;
        private readonly ConverterOptions options;

        public FixationResultToStringConverter(
            FixationResult<TFactValue> result,
            ConverterOptions options)
        {
            this.result = result;
            this.options = options;
        }

        public string Convert()
        {
            var sb = new StringBuilder();

            AppendHeader(sb, "Fixation Result");
            AppendExecutionInfo(sb);
            AppendModelInstances(sb);
            AppendModelStatistics(sb);

            return sb.ToString();
        }

        private void AppendHeader(StringBuilder sb, string header)
        {
            if (options.UseColors)
            {
                sb.AppendLine(ColoredText(new string('-', header.Length + 2), ConsoleColor.DarkGray));
                sb.AppendLine(ColoredText($"  {header}".PadRight(header.Length + 2), ConsoleColor.White));
                sb.AppendLine(ColoredText(new string('-', header.Length + 2), ConsoleColor.DarkGray));
            }
            else
            {
                sb.AppendLine(new string('-', header.Length + 4));
                sb.AppendLine($"  {header}");
                sb.AppendLine(new string('-', header.Length + 4));
            }
            sb.AppendLine();
        }

        private void AppendExecutionInfo(StringBuilder sb)
        {
            sb.AppendLine($"Seed: {result.Seed}");
            sb.AppendLine($"Execution time: {result.ElapsedMilliseconds} ms");
            sb.AppendLine();
        }

        private void AppendModelInstances(StringBuilder sb)
        {
            foreach (var instanceElement in result.ModelInstanceElementsByModelInstanceId.Values)
            {
                AppendModelInstanceInfo(sb, instanceElement);
                AppendOccurredFacts(sb, instanceElement);
                AppendNotOccurredFacts(sb, instanceElement);
                sb.AppendLine();
            }
        }

        private void AppendModelInstanceInfo(
            StringBuilder sb,
            FixationResultModelInstanceElement<TFactValue> instanceElement)
        {
            var instance = instanceElement.ModelInstance;
            var modelInfo = $"Model: {instance.ModelName} (Instance: {instance.InstanceId})";

            if (options.UseColors)
            {
                sb.AppendLine(ColoredText(modelInfo, ConsoleColor.Cyan));
            }
            else
            {
                sb.AppendLine(modelInfo);
            }
        }

        private void AppendOccurredFacts(
            StringBuilder sb,
            FixationResultModelInstanceElement<TFactValue> instanceElement)
        {
            if (instanceElement.OccurredFacts.Any())
            {
                sb.AppendLine("\nOccurred facts:");
                foreach (var factInfo in instanceElement.OccurredFacts)
                {
                    AppendFact(sb, factInfo, isOccurred: true);
                }
            }
        }

        private void AppendNotOccurredFacts(
            StringBuilder sb,
            FixationResultModelInstanceElement<TFactValue> instanceElement)
        {
            if (instanceElement.NotOccurredFacts.Any() && options.ShowNotOccurredFacts)
            {
                sb.AppendLine("\nNot occurred facts:");
                foreach (var factInfo in instanceElement.NotOccurredFacts)
                {
                    AppendFact(sb, factInfo, isOccurred: false);
                }
            }
        }

        private void AppendFact(
            StringBuilder sb,
            FixationResultInstanceFactInfo<TFactValue> factInfo,
            bool isOccurred)
        {
            var infix = isOccurred ? "V" : " ";
            var factValue = factInfo.FactValue?.ToString() ?? "null";
            var idInfo = $"(id: {factInfo.InstanceFactId})";

            var line = new StringBuilder();
            line.Append("\t");

            if (options.UseColors)
            {
                // Prefix
                line.Append("[");
                line.Append( ColoredText(infix, isOccurred ? ConsoleColor.Green : ConsoleColor.DarkGray));
                line.Append("] ");

                // Fact value
                line.Append(ColoredText(factValue, isOccurred ? ConsoleColor.White : ConsoleColor.Gray));
                line.Append(" ");

                // ID info
                line.Append(ColoredText(idInfo, ConsoleColor.DarkGray));
            }
            else
            {
                line.Append($"[{infix}] {factValue} {idInfo}");
            }

            sb.AppendLine(line.ToString());
        }

        private void AppendModelStatistics(StringBuilder sb)
        {
            if (result.ResultModelElementsByModelName.Any())
            {
                AppendHeader(sb, "Model Statistics");

                foreach (var modelElement in result.ResultModelElementsByModelName.Values)
                {
                    AppendModelStatistics(sb, modelElement);
                }
            }
        }

        private void AppendModelStatistics(
            StringBuilder sb,
            FixationResultModelElement<FactInfo<TFactValue>, TFactValue> modelElement)
        {
            var modelNameLine = $"Model: {modelElement.ModelName}";
            var runCountLine = $"Run count: {modelElement.RunCount}";

            if (options.UseColors)
            {
                sb.AppendLine(ColoredText(modelNameLine, ConsoleColor.Cyan));
                sb.AppendLine(ColoredText(runCountLine, ConsoleColor.White));
            }
            else
            {
                sb.AppendLine(modelNameLine);
                sb.AppendLine(runCountLine);
            }

            if (modelElement.FactsInfoByFactLocalId.Any())
            {
                sb.AppendLine("Fact occurrences:");

                foreach (var (factId, factInfo) in modelElement.FactsInfoByFactLocalId)
                {
                    var factLine = $"\t{factId}: {factInfo.FactCount} times";
                    if (options.UseColors)
                    {
                        sb.AppendLine(ColoredText(factLine, ConsoleColor.DarkGray));
                    }
                    else
                    {
                        sb.AppendLine(factLine);
                    }
                }
            }

            sb.AppendLine();
        }

        private static string ColoredText(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            // Map ConsoleColor to ANSI foreground codes
            string GetForegroundCode(ConsoleColor color) => color switch
            {
                ConsoleColor.Black => "30",
                ConsoleColor.DarkBlue => "34",
                ConsoleColor.DarkGreen => "32",
                ConsoleColor.DarkCyan => "36",
                ConsoleColor.DarkRed => "31",
                ConsoleColor.DarkMagenta => "35",
                ConsoleColor.DarkYellow => "33",
                ConsoleColor.Gray => "37",
                ConsoleColor.DarkGray => "90",
                ConsoleColor.Blue => "94",
                ConsoleColor.Green => "92",
                ConsoleColor.Cyan => "96",
                ConsoleColor.Red => "91",
                ConsoleColor.Magenta => "95",
                ConsoleColor.Yellow => "93",
                ConsoleColor.White => "97",
                _ => "39" // default foreground
            };

            // Map ConsoleColor to ANSI background codes (foreground code + 10)
            string GetBackgroundCode(ConsoleColor color) => color switch
            {
                ConsoleColor.Black => "40",
                ConsoleColor.DarkBlue => "44",
                ConsoleColor.DarkGreen => "42",
                ConsoleColor.DarkCyan => "46",
                ConsoleColor.DarkRed => "41",
                ConsoleColor.DarkMagenta => "45",
                ConsoleColor.DarkYellow => "43",
                ConsoleColor.Gray => "47",
                ConsoleColor.DarkGray => "100",
                ConsoleColor.Blue => "104",
                ConsoleColor.Green => "102",
                ConsoleColor.Cyan => "106",
                ConsoleColor.Red => "101",
                ConsoleColor.Magenta => "105",
                ConsoleColor.Yellow => "103",
                ConsoleColor.White => "107",
                _ => "49" // default background
            };

            var fgCode = GetForegroundCode(foreground);
            var bgCode = background.HasValue ? GetBackgroundCode(background.Value) : null;

            var colorCodes = string.IsNullOrEmpty(bgCode)
                ? fgCode
                : $"{fgCode};{bgCode}";

            return $"\x1b[{colorCodes}m{text}\x1b[0m";
        }
    }

    public class ConverterOptions
    {
        public bool ShowNotOccurredFacts { get; set; } = true;
        public bool UseColors { get; set; } = true;
    }
}