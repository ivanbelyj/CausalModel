using CausalModel.Factors;
using CausalModel.Facts;
using CausalModel.Model;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation.Random
{
    internal static class WeightFactorUtils
    {
        /// <summary>
        /// Randomly selects one of the fact variants, taking into account
        /// the weights of the variants.
        /// This method does not consider whether all passed variants are fixed
        /// and have happened
        /// </summary>
        public static InstanceFact<TFactValue>? SelectFactVariant<TFactValue>(
            List<InstanceFact<TFactValue>> variants,
            IFixationStateProvider fixatedProvider,
            RandomProvider randomProvider,
            IModelProvider<TFactValue> factProvider)
            where TFactValue : class
        {
            const float Epsilon = 0.000001f;

            var factsAndWeights = new List<(InstanceFact<TFactValue> fact, float totalWeight)>();
            float weightsSum = 0;
            foreach (var fact in variants)
            {
                float totalWeight = fact.Fact.Weights != null
                    ? TotalWeight(fact.Fact.Weights, fixatedProvider, factProvider)
                    : 0;
                if (totalWeight >= Epsilon)
                {
                    factsAndWeights.Add((fact, totalWeight));
                    weightsSum += totalWeight;
                }
            }
            if (weightsSum < Epsilon)
                return null;

            // Define the Id of the single implementation
            // Roulette wheel selection algorithm
            double choice = randomProvider.NextFloatSharedRandom(0, weightsSum);
            int curNodeIndex = -1;
            while (choice >= 0)
            {
                curNodeIndex++;
                if (curNodeIndex >= variants.Count)
                    curNodeIndex = 0;

                choice -= factsAndWeights[curNodeIndex].totalWeight;
            }
            return variants[curNodeIndex];
        }

        /// <summary>
        /// Calculates the total weight based of the given weight factors
        /// </summary>
        private static float TotalWeight<TFactValue>(
            IEnumerable<WeightFactor> factors,
            IFixationStateProvider fixatedProvider,
            IModelProvider<TFactValue> factProvider)
            where TFactValue : class
        {
            if (!factors.Any())
                throw new InvalidOperationException("Cannot calculate the total weight on empty weights");

            float weightSum = 0;

            foreach (var edge in factors)
            {
                // If the weight factor has no cause, it is assumed that the weight
                // always affects the choice
                if (edge.CauseId == null)
                {
                    weightSum += edge.Weight;
                    continue;
                }
                else
                {
                    bool? isFixated = fixatedProvider.IsFixated(
                        factProvider.GetModelFact(edge.CauseId).InstanceFactId);
                    if (isFixated != null && isFixated.Value)
                    {
                        weightSum += edge.Weight;
                    }
                }
            }

            return weightSum;
        }
    }
}
