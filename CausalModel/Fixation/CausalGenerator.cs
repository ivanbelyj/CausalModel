using CausalModel.Common;
using CausalModel.Model;
using CausalModel.Factors;
using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation;

public class CausalGenerator<TNodeValue> : IRandomProvider
{
    private FactCollection<TNodeValue> facts;

    private Dictionary<Fact<TNodeValue>,
        List<Fact<TNodeValue>>> causesAndConsequences;
    private Dictionary<Fact<TNodeValue>,
        List<Fact<TNodeValue>>> factsAndVariants;
    private HashSet<Fact<TNodeValue>> rootNodes;

    private readonly IFixator<TNodeValue> fixator;
    private readonly Random random;

    public CausalGenerator(
        // FactCollection<TNodeValue> factCollection,
        CausalModel<TNodeValue> model,
        int seed,
        IFixator<TNodeValue> fixator)
    {
        //Facts = factCollection;
        Facts = model.Facts;  // Todo
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

    public double NextDouble(double min = 0, double max = 1)
    {
        return random.NextDouble(min, max);
    }

    /// <summary>
    /// Определены ли причины факта, чтобы можно было говорить о его происшествии?
    /// </summary>
    private bool? IsFollowingFromCauses(Fact<TNodeValue> fact)
        => fact.CausesExpression.Evaluate(facts, fixator, this);

    public void Fixate(string factId, bool? isFactHappened = null)
    {
        Fact<TNodeValue> fixatingFact = facts.GetFactById(factId);

        // Если происшествие факта не задано явно,
        // происшествие определяется на основе причин
        if (isFactHappened == null)  
        {
            bool? isFollowingFromCauses = IsFollowingFromCauses(fixatingFact);

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
        if (!(fixatingFact is Fact<TNodeValue>))
        {
            fixator.FixateFact(fixatingFact, isFactHappened.Value);

            FixateNotFixatedConsequences(fixatingFact);
        }
        else
        {
            // Фиксация вариантов реализации абстрактных фактов

            var abstractFact = facts.GetFactById(fixatingFact.AbstractFactId);

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

            var selectedVariant = WeightFactorUtils.SelectFactVariant(
                variantsCanHappen, fixator, this);
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

    private void FixateNotFixatedConsequences(Fact<TNodeValue> fixatingFact)
    {
        if (!causesAndConsequences.ContainsKey(fixatingFact))
        {
            return;
        }
        foreach (var consequence in causesAndConsequences[fixatingFact])
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
            List<Fact<TNodeValue>>>();

        foreach (Fact<TNodeValue> fact in facts)
        {
            if (fact.IsRootCause())
                rootNodes.Add(fact);

            if (fact.AbstractFactId != null)
            {
                var abstractFact = facts.GetFactById(fact.AbstractFactId);
                if (!factsAndVariants.ContainsKey(abstractFact))
                {
                    factsAndVariants.Add(abstractFact,
                        new List<Fact<TNodeValue>> { fact });
                }
                else
                {
                    factsAndVariants[abstractFact].Add(fact);
                }
            }

            foreach (var edge in fact.GetCauses())
            {
                if (edge.CauseId != null)
                {
                    var cause = facts.GetFactById(edge.CauseId);
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

    public void FixateRoots()
    {
        foreach (var root in rootNodes)
        {
            Fixate(root.Id);
        }
    }
}
