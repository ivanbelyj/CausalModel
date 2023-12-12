using CausalModel.Common;
using CausalModel.Facts;
using CausalModel.CausesExpressionTree;
using CausalModel.Model.Providers;
using CausalModel.Model.Instance;
using CausalModel.Model;

namespace CausalModel.Fixation;

/// <summary>
/// Performs causal model fixation traverse
/// </summary>
/// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
public class CausalGenerator<TFactValue> : IRandomProvider
{
    private readonly IResolvedModelProvider<TFactValue> modelProvider;
    private readonly IFixator<TFactValue> fixator;
    private readonly Random random;

    public CausalGenerator(
        IResolvedModelProvider<TFactValue> modelProvider,
        IFixator<TFactValue> fixator,
        int? seed = null)
    {
        this.modelProvider = modelProvider;
        this.fixator = fixator;

        random = seed == null ? new Random() : new Random(seed.Value);
    }

    // Todo: make seed not sensitive to fixation order?
    public float NextDouble(float min = 0, float max = 1)
    {
        return (float)random.NextDouble(min, max);
    }

    private readonly Dictionary<string, FactProvider<TFactValue>>
        factProviderByInstanceId = new();
    private FactProvider<TFactValue> GetFactProviderByInstanceId(string instanceId)
    {
        if (!factProviderByInstanceId.ContainsKey(instanceId))
        {
            var factProvider = new FactProvider<TFactValue>(modelProvider,
                instanceId);
            factProviderByInstanceId.Add(instanceId, factProvider);
        }

        return factProviderByInstanceId[instanceId];
    }

    /// <summary>
    /// Not null, if the causes are determined to evaluate the result
    /// </summary>
    private bool? IsFollowingFromCauses(string modelInstanceId,
        CausesExpression causesExpression)
        => causesExpression.Evaluate(GetFactProviderByInstanceId(modelInstanceId),
            fixator, this);

    //public void Fixate(string factId, bool? isFactHappened = null)
    //{
    //    Fixate(new InstanceFactId(factId, modelProvider.RootModelInstanceId),
    //        isFactHappened);
    //}

    public void Fixate(InstanceFactId factId, bool? isFactHappened = null)
    {
        InstanceFact<TFactValue> fixatingFact = modelProvider.GetFact(factId);
        string modelInstanceId = fixatingFact.InstanceFactId.CausalInstanceId;

        // If the fact happening is not explicitly specified,
        // the happening is determined based on the causes
        if (isFactHappened == null)
        {
            bool? isFollowingFromCauses = IsFollowingFromCauses(
                factId.CausalInstanceId,
                fixatingFact.Fact.CausesExpression);

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
        if (fixatingFact.Fact.AbstractFactId == null)
        {
            fixator.FixateFact(fixatingFact.InstanceFactId, isFactHappened.Value);

            FixateNotFixatedConsequences(fixatingFact);
        }
        else
        {
            // Fixation of the abstract fact implementation variants

            // Todo: causal models integration via block.Causes
            // and block.Consequences

            // In the current implementation the abstract fact and its
            // implementations are all from the same model instance

            var abstractFact = modelProvider.GetFact(
                new InstanceFactId(fixatingFact.Fact.AbstractFactId,
                    modelInstanceId));

            var variantsFollowingFromCauses = modelProvider
                .GetAbstractFactVariants(abstractFact.InstanceFactId)
                .Select(variant => {
                    bool? canHappen = IsFollowingFromCauses(modelInstanceId,
                        variant.Fact.CausesExpression);
                    return (canHappen, variant);
                })
                .Where(x => x.canHappen != null)
                .ToList();

            // Если для какого-либо варианта реализации не хватает данных,
            // выбор единственной реализации откладывается
            if (variantsFollowingFromCauses.Count < modelProvider
                .GetAbstractFactVariants(abstractFact.InstanceFactId)
                .Count())
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
                variantsCanHappen, fixator, this,
                GetFactProviderByInstanceId(modelInstanceId));
            foreach (var factVariant in variantsFollowingFromCauses
                .Select(x => x.variant))
            {
                // Фиксируем также те варианты, которые не участвовали в выборе,
                // т.к. заведомо не произойдут

                if (fixator.IsFixated(factVariant.InstanceFactId) == null)
                {
                    fixator.FixateFact(factVariant.InstanceFactId,
                        ReferenceEquals(factVariant, selectedVariant));

                    FixateNotFixatedConsequences(factVariant);
                }
            }
        }
    }

    private void FixateNotFixatedConsequences(InstanceFact<TFactValue> fixatingFact)
    {
        var consequences = modelProvider
            .TryGetConsequences(fixatingFact.InstanceFactId);
        if (consequences == null)
            return;

        foreach (var consequence in consequences)
        {
            if (fixator.IsFixated(consequence.InstanceFactId) == null)
                Fixate(consequence.InstanceFactId);
        }
    }

    public void FixateRootCauses()
    {
        foreach (var root in modelProvider.GetRootCauses())
        {
            Fixate(root.InstanceFactId);
        }
    }
}
