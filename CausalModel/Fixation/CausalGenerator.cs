using System;
using System.Collections.Generic;
using System.Linq;
using CausalModel.Common;
using CausalModel.Facts;
using CausalModel.CausesExpressionTree;
using CausalModel.Model.Instance;
using CausalModel.Model;
using CausalModel.Model.Resolving;
using CausalModel.Fixation.Fixators;

namespace CausalModel.Fixation
{
    /// <summary>
    /// Performs causal model fixation traverse
    /// </summary>
    /// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
    public class CausalGenerator<TFactValue> : IRandomProvider
        where TFactValue : class
    {
        private readonly IResolvedModelProvider<TFactValue> modelProvider;

        public IResolvedModelProvider<TFactValue> ModelProvider => modelProvider;

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
            fixator.FactFixated += OnFactFixated;

            random = seed == null ? new Random() : new Random(seed.Value);
        }

        private void OnFactFixated(
            object sender,
            InstanceFact<TFactValue> fixatedFact,
            bool isOccured)
        {
            FixateNotFixatedConsequencesAndVariants(fixatedFact);
        }

        // Todo: make seed not sensitive to fixation order?
        public float NextFloat(float min = 0, float max = 1)
        {
            float res = (float)random.NextDouble(min, max);
            return res;
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
            InstanceFactId factId = fixatingFact.InstanceFactId;

            // If the fact happening is not explicitly specified,
            // the happening is determined based on the causes
            if (isFactHappened == null)
            {
                bool? isFollowingFromCauses = IsFollowingFromCauses(
                    factId.ModelInstanceId,
                    fixatingFact.Fact.CausesExpression);

                bool hasAbstractFactNotFixated =
                    fixatingFact.Fact.AbstractFactId != null &&
                    !IsFixated(GetAbstractFact(fixatingFact));

                // The case when there is not enough data (some causes have not been
                // fixated yet)
                if (isFollowingFromCauses == null || hasAbstractFactNotFixated)
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
                FixateFactCore(fixatingFact, isFactHappened.Value);
            }
            else
            {
                FixateAbstractFactVariants(fixatingFact);
            }
        }

        private void FixateFactCore(
            InstanceFact<TFactValue> fixatingFact,
            bool isOccurred)
        {
            fixator.HandleFixation(fixatingFact, isOccurred);
        }

        private void FixateNotFixatedConsequencesAndVariants(
            InstanceFact<TFactValue> fixatingFact)
        {
            var descendants = new List<InstanceFact<TFactValue>>();
            var consequences = causesTree
                .TryGetConsequences(fixatingFact.InstanceFactId);
            if (consequences != null)
            {
                descendants.AddRange(consequences);
            }

            // Todo: ?
            //var variants = causesTree
            //    .TryGetAbstractFactVariants(fixatingFact.InstanceFactId);
            //if (variants != null)
            //{
            //    descendants.AddRange(variants);
            //}

            foreach (var descendant in descendants)
            {
                if (!IsFixated(descendant))
                    Fixate(descendant.InstanceFactId.ToAddress());
            }
        }

        private bool IsFixated(InstanceFact<TFactValue> fact)
        {
            return fixator.IsFixated(fact.InstanceFactId) != null;
        }

        private InstanceFact<TFactValue> GetAbstractFact(
            InstanceFact<TFactValue> fixatingFact)
        {
            if (fixatingFact.Fact.AbstractFactId == null)
                throw new InvalidOperationException();

            string modelInstanceId = fixatingFact.InstanceFactId.ModelInstanceId;
            return modelProvider.GetFact(
                new InstanceFactAddress(fixatingFact.Fact.AbstractFactId,
                    modelInstanceId));
        }

        private void FixateAbstractFactVariants(InstanceFact<TFactValue> fixatingFact)
        {
            if (fixatingFact.Fact.AbstractFactId == null)
                throw new InvalidOperationException();

            // Fixation of the abstract fact implementation variants

            var abstractFact = GetAbstractFact(fixatingFact);
            string modelInstanceId = fixatingFact.InstanceFactId.ModelInstanceId;

            var variantsFollowingFromCauses = causesTree
                .GetAbstractFactVariants(abstractFact.InstanceFactId)
                .Select(variant =>
                {
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
            // from the candidates that have occurred
            var variantsCanOccur = variantsFollowingFromCauses
                .Where(x => x.canHappen == true)
                .Select(x => x.variant)
                .ToList();

            var selectedVariant = WeightFactorUtils.SelectFactVariant(
                variantsCanOccur,
                fixator,
                this,
                modelProvider.GetModelProvider(modelInstanceId));
            foreach (var factVariant in variantsFollowingFromCauses
                .Select(x => x.variant))
            {
                // Also fixating variants not participated in the selection
                // because they obviously won't occur

                if (!IsFixated(factVariant))
                {
                    FixateFactCore(factVariant,
                        ReferenceEquals(factVariant, selectedVariant));
                }
            }
        }

        private IEnumerable<InstanceFact<TFactValue>> GetRootFacts()
        {
            return modelProvider
                .GetResolvedFacts()
                .Where(instanceFact => instanceFact.Fact.IsRootCause());
        }

        public void FixateRootFacts()
        {
            foreach (var root in GetRootFacts())
            {
                Fixate(root.InstanceFactId.ToAddress());
            }
        }
    }
}
