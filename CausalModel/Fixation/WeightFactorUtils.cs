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
    public static double TotalWeight(IEnumerable<WeightFactor> factors,
        IFixatedProvider happenedProvider)
    {
        if (factors.Count() == 0)
            throw new InvalidOperationException("Cannot calculate the total weight on empty weights");

        double weightSum = 0;

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
    /// Случайно выбирает одну из реализаций факта, учитывая веса вариантов.
    /// Данный метод не учитывает, все ли переданные варианты зафиксированы
    /// и произошли
    /// </summary>
    public static Fact<TFactValue>? SelectFactVariant<TFactValue>(
        List<Fact<TFactValue>> variants,
        IFixatedProvider fixatedProvider,
        IRandomProvider randomProvider)
    {
        // Собрать информацию о узлах и их общих весах, собрать сумму весов,
        // а также отбросить узлы с нулевыми весами
        var nodesWeights = new List<(Fact<TFactValue> node, double totalWeight)>();
        double weightsSum = 0;
        foreach (var fact in variants)
        {
            double totalWeight = WeightFactorUtils.TotalWeight(fact.Weights!,
                fixatedProvider);
            if (totalWeight >= double.Epsilon)
            {
                nodesWeights.Add((fact, totalWeight));
                weightsSum += totalWeight;
            }
        }
        if (weightsSum < double.Epsilon)
            return null;

        // Определить Id единственной реализации
        // Алгоритм Roulette wheel selection
        double choice = randomProvider.NextDouble(0, weightsSum);
        int curNodeIndex = -1;
        while (choice >= 0)
        {
            curNodeIndex++;
            if (curNodeIndex >= variants.Count)
                curNodeIndex = 0;

            // choice -= nodes[curNodeIndex].WeightNest.TotalWeight();
            choice -= nodesWeights[curNodeIndex].totalWeight;
        }
        return variants[curNodeIndex];
    }
}
