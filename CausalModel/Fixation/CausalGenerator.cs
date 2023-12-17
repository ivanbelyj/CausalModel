using CausalModel.Common;
using CausalModel.Facts;
using CausalModel.CausesExpressionTree;
using CausalModel.Model.Providers;
using CausalModel.Model.Instance;
using CausalModel.Model;
using CausalModel.Model.ResolvingModelProvider;

namespace CausalModel.Fixation;

/// <summary>
/// Performs causal model fixation traverse
/// </summary>
/// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
public class CausalGenerator<TFactValue> : IRandomProvider
{
    private readonly IResolvedModelProvider<TFactValue> modelProvider;
    private readonly ICausesTree<TFactValue> causesTree;

    private readonly IFixator<TFactValue> fixator;
    private readonly Random random;

    public CausalGenerator(
        IResolvedModelProvider<TFactValue> modelProvider,
        ICausesTree<TFactValue> causesTree,
        IFixator<TFactValue> fixator,
        int? seed = null)
    {
        this.modelProvider = modelProvider;
        this.causesTree = causesTree;

        this.fixator = fixator;

        random = seed == null ? new Random() : new Random(seed.Value);
    }

    // Todo: make seed not sensitive to fixation order?
    public float NextDouble(float min = 0, float max = 1)
    {
        return (float)random.NextDouble(min, max);
    }

    private readonly Dictionary<string, ModelProvider<TFactValue>>
        factProviderByInstanceId = new();
    private ModelProvider<TFactValue> GetFactProviderByInstanceId(string instanceId)
    {
        if (!factProviderByInstanceId.ContainsKey(instanceId))
        {
            var factProvider = new ModelProvider<TFactValue>(modelProvider,
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

    public void Fixate(InstanceFactId factId, bool? isFactHappened = null)
    {
        InstanceFact<TFactValue> fixatingFact = modelProvider.GetFact(factId);
        string modelInstanceId = fixatingFact.InstanceFactId.ModelInstanceId;

        // If the fact happening is not explicitly specified,
        // the happening is determined based on the causes
        if (isFactHappened == null)
        {
            bool? isFollowingFromCauses = IsFollowingFromCauses(
                factId.ModelInstanceId,
                fixatingFact.Fact.CausesExpression);

            // The case when there is not enough data (some causes have not been
            // fixated yet)
            if (isFollowingFromCauses == null)
            {
                return;
            }

            isFactHappened = isFollowingFromCauses.Value;
        }

        // For the happening of ordinary facts, at least
        // the condition of following from the causes is sufficient
        // (as the most, an explicit defined the fact of happening)
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

            var variantsFollowingFromCauses = causesTree
                .GetAbstractFactVariants(abstractFact.InstanceFactId)
                .Select(variant => {
                    bool? canHappen = IsFollowingFromCauses(modelInstanceId,
                        variant.Fact.CausesExpression);
                    return (canHappen, variant);
                })
                .Where(x => x.canHappen != null)
                .ToList();

            // If there is not enough data for any implementation variant,
            // the selection of a single implementation is postponed
            if (variantsFollowingFromCauses.Count < causesTree
                .GetAbstractFactVariants(abstractFact.InstanceFactId)
                .Count())
            {
                return;
            }

            // In the future, the implementation is selected only
            // from the candidates that have happened
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

                // Also fixating variants not participated in the selection
                // because they obviously won't happen

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
        var consequences = causesTree
            .TryGetConsequences(fixatingFact.InstanceFactId);
        if (consequences == null)
            return;

        foreach (var consequence in consequences)
        {
            if (fixator.IsFixated(consequence.InstanceFactId) == null)
                Fixate(consequence.InstanceFactId);
        }
    }

    private IEnumerable<InstanceFact<TFactValue>> GetRootFacts()
    {
        return modelProvider
            .GetResolvedFacts()
            .Where(instanceFact => instanceFact.Fact.IsRootCause());
    }

    public void FixateRootCauses()
    {
        foreach (var root in GetRootFacts())
        {
            Fixate(root.InstanceFactId);
        }
    }
}
