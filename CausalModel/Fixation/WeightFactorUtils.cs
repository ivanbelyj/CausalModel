using CausalModel.Factors;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation;
internal static class WeightFactorUtils
{
    /// <summary>
    /// Calculates the total weight based of the given weight factors
    /// </summary>
    public static float TotalWeight(IEnumerable<WeightFactor> factors,
        IFixatedProvider happenedProvider)
    {
        if (factors.Count() == 0)
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
                bool? isHappened = happenedProvider.IsFixated(edge.CauseId);
                if (isHappened != null && isHappened.Value)
                {
                    weightSum += edge.Weight;
                }
            }
        }

        return weightSum;
    }

    /// <summary>
    /// Randomly selects one of the fact variants, taking into account
    /// the weights of the variants.
    /// This method does not consider whether all passed variants are fixed
    /// and have happened
    /// </summary>
    public static Fact<TFactValue>? SelectFactVariant<TFactValue>(
        List<Fact<TFactValue>> variants,
        IFixatedProvider fixatedProvider,
        IRandomProvider randomProvider)
    {
        const float EPSILON = 0.000001f;

        var factsAndWeights = new List<(Fact<TFactValue> fact, float totalWeight)>();
        float weightsSum = 0;
        foreach (var fact in variants)
        {
            float totalWeight = WeightFactorUtils.TotalWeight(fact.Weights!,
                fixatedProvider);
            if (totalWeight >= EPSILON)
            {
                factsAndWeights.Add((fact, totalWeight));
                weightsSum += totalWeight;
            }
        }
        if (weightsSum < double.Epsilon)
            return null;

        // Define the Id of the single implementation
        // Roulette wheel selection algorithm
        double choice = randomProvider.NextDouble(0, weightsSum);
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
}
