using CausalModel.Common;
using CausalModel.FactCollection;
using CausalModel.Factors;
using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;

public class CausalModel<TNodeValue> : IFixatingValueProvider
{
    private FactCollection<TNodeValue> facts;

    private Dictionary<Fact<TNodeValue>,
        List<Fact<TNodeValue>>> causesAndConsequences;
    private Dictionary<Fact<TNodeValue>,
        List<FactVariant<TNodeValue>>> factsAndVariants;
    private HashSet<Fact<TNodeValue>> rootNodes;

    private readonly IFixator<TNodeValue> fixator;
    private readonly Random random;

    public CausalModel(FactCollection<TNodeValue> factCollection, int seed,
        IFixator<TNodeValue> fixator)
    {
        Facts = factCollection;
        random = new Random(seed);

        this.fixator = fixator;
    }

    public FactCollection<TNodeValue> Facts
    {
        get => facts;
        private set
        {
            facts = value;
            Initialize();
        }
    }

    public float GetFixatingValue()
    {
        float res = (float)random.NextDouble();
        return res;
    }

    /// <summary>
    /// Определены ли причины факта, чтобы можно было говорить о его происшествии?
    /// </summary>
    private bool? IsFollowingFromCauses(Fact<TNodeValue> fact)
        => fact.ProbabilityNest.CausesExpression.Evaluate(facts, fixator, this);

    public void Fixate(Guid factId, bool? isFactHappened = null)
    {
        Fact<TNodeValue> fact = facts.GetFactById(factId);

        // Если происшествие факта не задано явно,
        // происшествие определяется на основе причин
        if (isFactHappened == null)  
        {
            bool? isFollowingFromCauses = IsFollowingFromCauses(fact);

            // Случай, когда недостаточно данных (некоторые причины
            // еще не зафиксированы)
            if (isFollowingFromCauses == null)
            {
                return;
            }

            isFactHappened = isFollowingFromCauses.Value;
        }

        // Для происшествия обычных фактов достаточно как минимум
        // условия следования из причин
        // (как максимум - явного задания факта происшествия)
        if (!(fact is FactVariant<TNodeValue>))
        {
            fixator.FixateFact(fact, isFactHappened.Value);

            FixateNotFixatedConsequences(fact);
        }
        else
        {
            // Фиксация вариантов реализации абстрактных фактов

            var variant = (FactVariant<TNodeValue>)fact;
            var abstractFact = facts.GetFactById(variant.AbstractFactId);

            var variantsFollowingFromCauses = factsAndVariants[abstractFact]
                .Select(variant => {
                    bool? canHappen = IsFollowingFromCauses(variant);
                    return (canHappen, variant);
                })
                .Where(x => x.canHappen != null)
                .ToList();

            // Если для какого-либо варианта реализации не хватает данных,
            // выбор единственной реализации откладывается
            if (variantsFollowingFromCauses.Count
                < factsAndVariants[abstractFact].Count)
            {
                return;
            }

            // В дальнейшем реализация выбирается лишь из произошедших кандидатов
            var variantsCanHappen = variantsFollowingFromCauses
                .Where(x => x.canHappen == true)
                .Select(x => x.variant)
                .ToList();

            //goingToHappen = factsAndVariants[abstractFact];

            var selectedVariant = SelectFactVariant(variantsCanHappen);
            foreach (var factVariant in variantsFollowingFromCauses
                .Select(x => x.variant))
            {
                // Фиксируем также те варианты, которые не участвовали в выборе,
                // т.к. заведомо не произойдут

                if (fixator.IsFixated(factVariant.Id) == null)
                {
                    fixator.FixateFact(factVariant, ReferenceEquals(factVariant,
                        selectedVariant));

                    FixateNotFixatedConsequences(factVariant);
                }
            }
        }
    }

    private void FixateNotFixatedConsequences(Fact<TNodeValue> fact)
    {
        if (!causesAndConsequences.ContainsKey(fact))
        {
            return;
        }
        foreach (var consequence in causesAndConsequences[fact])
        {
            if (fixator.IsFixated(consequence.Id) == null)
                Fixate(consequence.Id);
        }
    }

    private void Initialize()
    {
        causesAndConsequences = new Dictionary<Fact<TNodeValue>,
            List<Fact<TNodeValue>>>();
        rootNodes = new HashSet<Fact<TNodeValue>>();
        factsAndVariants = new Dictionary<Fact<TNodeValue>,
            List<FactVariant<TNodeValue>>>();

        foreach (Fact<TNodeValue> fact in facts.Nodes)
        {
            if (fact.IsRootNode())
                rootNodes.Add(fact);

            if (fact is FactVariant<TNodeValue> variant)
            {
                var abstractFact = facts.GetFactById(variant.AbstractFactId);
                if (!factsAndVariants.ContainsKey(abstractFact))
                {
                    factsAndVariants.Add(abstractFact,
                        new List<FactVariant<TNodeValue>> { variant });
                }
                else
                {
                    factsAndVariants[abstractFact].Add(variant);
                }
            }

            foreach (var edge in fact.GetEdges())
            {
                if (edge.CauseId.HasValue)
                {
                    var cause = facts.GetFactById(edge.CauseId.Value);
                    if (!causesAndConsequences.ContainsKey(cause))
                    {
                        causesAndConsequences.Add(cause,
                            new List<Fact<TNodeValue>>() { fact });
                    }
                    else
                    {
                        causesAndConsequences[cause].Add(fact);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Случайно выбирает одну из реализаций факта, учитывая веса вариантов.
    /// Данный метод не учитывает, все ли переданные варианты зафиксированы
    /// и произошли
    /// </summary>
    public FactVariant<TNodeValue>? SelectFactVariant(
        List<FactVariant<TNodeValue>> variants)
    {
        // Собрать информацию о узлах и их общих весах, собрать сумму весов,
        // а также отбросить узлы с нулевыми весами
        var nodesWeights = new List<(Fact<TNodeValue> node, double totalWeight)>();
        double weightsSum = 0;
        foreach (var node in variants)
        {
            double totalWeight = node.WeightNest.TotalWeight(fixator);
            if (totalWeight >= double.Epsilon)
            {
                nodesWeights.Add((node, totalWeight));
                weightsSum += totalWeight;
            }
        }
        if (weightsSum < double.Epsilon)
            return null;

        // Определить Id единственной реализации
        // Алгоритм Roulette wheel selection
        double choice = random.NextDouble(0, weightsSum);
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

    public void FixateRoots()
    {
        foreach (var root in rootNodes)
        {
            Fixate(root.Id);
        }
    }
}
