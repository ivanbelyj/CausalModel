using CausalModel.Common;
using CausalModel.Facts;
using CausalModel.CausesExpressionTree;
using CausalModel.Model.Providers;

namespace CausalModel.Fixation;

/// <summary>
/// Performs causal model fixation traverse
/// </summary>
/// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
public class CausalGenerator<TFactValue> : IRandomProvider
{
    private readonly IModelProvider<TFactValue> modelProvider;
    private readonly IFixator<TFactValue> fixator;
    private readonly Random random;

    public CausalGenerator(
        IModelProvider<TFactValue> modelProvider,
        IFixator<TFactValue> fixator,
        int? seed = null)
    {
        this.modelProvider = modelProvider;
        this.fixator = fixator;

        random = seed == null ? new Random() : new Random(seed.Value);
    }

    public float NextDouble(float min = 0, float max = 1)
    {
        return (float)random.NextDouble(min, max);
    }

    /// <summary>
    /// Not null, if the causes are determined to evaluate the result
    /// </summary>
    private bool? IsFollowingFromCauses(CausesExpression causesExpression)
        => causesExpression.Evaluate(modelProvider, fixator, this);

    public void Fixate(string factId, bool? isFactHappened = null)
    {
        Fact<TFactValue> fixatingFact = modelProvider.GetFact(factId);

        // If the fact happening is not explicitly specified,
        // the happening is determined based on the causes
        if (isFactHappened == null)  
        {
            bool? isFollowingFromCauses = IsFollowingFromCauses(fixatingFact
                .CausesExpression);

            // The case when there is not enough data (some causes have not been
            // fixated yet)
            if (isFollowingFromCauses == null)
            {
                return;
            }

            isFactHappened = isFollowingFromCauses.Value;
        }

        // Для происшествия обычных фактов достаточно как минимум
        // условия следования из причин
        // (как максимум - явного задания факта происшествия)
        if (fixatingFact.AbstractFactId == null)
        {
            fixator.FixateFact(fixatingFact, isFactHappened.Value);

            FixateNotFixatedConsequences(fixatingFact);
        }
        else
        {
            // Фиксация вариантов реализации абстрактных фактов

            var abstractFact = modelProvider.GetFact(fixatingFact.AbstractFactId);

            var variantsFollowingFromCauses = modelProvider
                .GetAbstractFactVariants(abstractFact)
                .Select(variant => {
                    bool? canHappen = IsFollowingFromCauses(variant.CausesExpression);
                    return (canHappen, variant);
                })
                .Where(x => x.canHappen != null)
                .ToList();

            // Если для какого-либо варианта реализации не хватает данных,
            // выбор единственной реализации откладывается
            if (variantsFollowingFromCauses.Count
                < modelProvider.GetAbstractFactVariants(abstractFact).Count())
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

    private void FixateNotFixatedConsequences(Fact<TFactValue> fixatingFact)
    {
        var consequences = modelProvider.TryGetConsequences(fixatingFact);
        if (consequences == null)
            return;

        foreach (var consequence in consequences)
        {
            if (fixator.IsFixated(consequence.Id) == null)
                Fixate(consequence.Id);
        }
    }

    public void FixateRootCauses()
    {
        foreach (var root in modelProvider.GetRootCauses())
        {
            Fixate(root.Id);
        }
    }
}
