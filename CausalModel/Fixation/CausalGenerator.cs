using CausalModel.Common;
using CausalModel.Facts;
using CausalModel.CausesExpressionTree;
using CausalModel.Model.Instance;
using CausalModel.Model;
using CausalModel.Model.Resolving;

namespace CausalModel.Fixation;

/// <summary>
/// Performs causal model fixation traverse
/// </summary>
/// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
public class CausalGenerator<TFactValue> : IRandomProvider
{
    private readonly IResolvedModelProvider<TFactValue> modelProvider;

    public IResolvedModelProvider<TFactValue> ModelProvider => modelProvider;

    private readonly ICausesTree<TFactValue> causesTree;

    private readonly IFixator<TFactValue> fixator;
    //public IFixator<TFactValue> Fixator => fixator;

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

    /// <summary>
    /// Not null, if the causes are determined to evaluate the result
    /// </summary>
    private bool? IsFollowingFromCauses(string modelInstanceId,
        CausesExpression causesExpression)
        => causesExpression.Evaluate(modelProvider.GetModelProvider(modelInstanceId),
            fixator, this);

    public void Fixate(InstanceFactAddress address, bool? isFactHappened = null)
    {
        InstanceFact<TFactValue> fixatingFact = modelProvider.GetFact(address);
        string modelInstanceId = fixatingFact.InstanceFactId.ModelInstanceId;
        InstanceFactId factId = fixatingFact.InstanceFactId;

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

            var abstractFact = modelProvider.GetFact(
                new InstanceFactAddress(fixatingFact.Fact.AbstractFactId,
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
                modelProvider.GetModelProvider(modelInstanceId));
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
                Fixate(consequence.InstanceFactId.ToAddress());
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
            Fixate(root.InstanceFactId.ToAddress());
        }
    }
}
