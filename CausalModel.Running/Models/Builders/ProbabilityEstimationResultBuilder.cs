using CausalModel.Running.Models.Common;
using CausalModel.Running.Models.FixationResult;
using CausalModel.Running.ProbabilityEstimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Models.Builders
{
    public class ProbabilityEstimationResultBuilder<TFactValue>
        where TFactValue : class
    {
        private int runCount = 0;
        private readonly List<TimeSpan> elapsedTimes = new List<TimeSpan>();

        private readonly Dictionary<string, FixationResultModelElement<ProbabilityEstimationResultFactInfo<TFactValue>, TFactValue>> aggregatedResultsByModelName
            = new Dictionary<string, FixationResultModelElement<ProbabilityEstimationResultFactInfo<TFactValue>, TFactValue>>();

        // Todo: improve naming
        public void AddNewFixationResult(FixationResult<TFactValue> fixationResult)
        {
            runCount++;

            elapsedTimes.Add(fixationResult.Elapsed);

            foreach (var (modelName, resultModelElement) in fixationResult.ResultModelElementsByModelName)
            {
                if (!aggregatedResultsByModelName.TryGetValue(modelName, out var aggregated))
                {
                    aggregated = new FixationResultModelElement<ProbabilityEstimationResultFactInfo<TFactValue>, TFactValue>(modelName);
                    aggregatedResultsByModelName.Add(
                        modelName,
                        aggregated);
                }

                foreach (var (factId, factInfo) in resultModelElement.FactsInfoByFactLocalId)
                {
                    if (!aggregated.FactsInfoByFactLocalId.TryGetValue(factId, out var aggregatedFactInfo))
                    {
                        aggregatedFactInfo = new ProbabilityEstimationResultFactInfo<TFactValue>(
                            factInfo.FactLocalId,
                            factInfo.FactValue);
                        aggregated.FactsInfoByFactLocalId.Add(
                            factId,
                            aggregatedFactInfo);
                    }
                    aggregatedFactInfo.FactCount += factInfo.FactCount;
                }

                aggregatedResultsByModelName[modelName].RunCount += resultModelElement.RunCount;
            }
        }

        public ProbabilityEstimationResult<TFactValue> Build()
        {
            return new ProbabilityEstimationResult<TFactValue>()
            {
                SimulationsCount = runCount,
                MinTime = elapsedTimes.Min(),
                MaxTime = elapsedTimes.Max(),
                AverageTime = TimeSpan.FromTicks((long)elapsedTimes.Average(ts => ts.Ticks)),
                FactsInfoByModelName = CalculateFactActualProbabilitiesByModelName(),
            };
        }

        private Dictionary<string, Dictionary<string, ProbabilityEstimationResultFactInfo<TFactValue>>> CalculateFactActualProbabilitiesByModelName()
        {
            var result = new Dictionary<string, Dictionary<string, ProbabilityEstimationResultFactInfo<TFactValue>>>();

            foreach (var (modelName, modelResults) in aggregatedResultsByModelName)
            {
                var factsInfoByFactLocalId = new Dictionary<string, ProbabilityEstimationResultFactInfo<TFactValue>>();
                result.Add(modelName, factsInfoByFactLocalId);

                foreach (var (factId, factInfo) in modelResults.FactsInfoByFactLocalId)
                {
                    float probability = (float)factInfo.FactCount
                        / aggregatedResultsByModelName[modelName].RunCount;
                    factsInfoByFactLocalId.Add(
                        factId,
                        new ProbabilityEstimationResultFactInfo<TFactValue>(
                            factInfo.FactLocalId,
                            factInfo.FactValue)
                    {
                        EstimatedProbability = probability,
                        FactCount = factInfo.FactCount
                    });
                }
            }

            return result;
        }
    }
}
