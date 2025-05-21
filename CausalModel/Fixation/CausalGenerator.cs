using System;
using System.Collections.Generic;
using System.Linq;
using CausalModel.Facts;
using CausalModel.CausesExpressionTree;
using CausalModel.Model.Instance;
using CausalModel.Model;
using CausalModel.Model.Resolving;
using CausalModel.Fixation.Fixators;
using CausalModel.Model.Resolving.ResolvingNode;
using CausalModel.Common.Extensions;
using CausalModel.Fixation.Random;

namespace CausalModel.Fixation
{
    /// <summary>
    /// Performs causal model fixation traverse
    /// </summary>
    /// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
    public class CausalGenerator<TFactValue>
        where TFactValue : class
    {
        private readonly IResolvedModelProvider<TFactValue> modelProvider;

        private readonly ICausesTree<TFactValue> causesTree;

        private readonly IFixator<TFactValue> fixator;

        private readonly RandomProvider randomProvider;

        public int Seed => randomProvider.BaseSeed;

        public IResolvedModelProvider<TFactValue> ModelProvider => modelProvider;

        public CausalGenerator(
            IResolvedModelProvider<TFactValue> modelProvider,
            ICausesTree<TFactValue> causesTree,
            IFixator<TFactValue> fixator,
            int? seed = null)
        {
            randomProvider = new RandomProvider(seed);

            this.modelProvider = modelProvider;
            this.causesTree = causesTree;

            this.fixator = fixator;
            fixator.FactFixated += OnFactFixated;
        }

        private void OnFactFixated(
            object sender,
            InstanceFact<TFactValue> fixatedFact,
            bool isOccured)
        {
            FixateNotFixatedConsequencesAndVariants(fixatedFact);
        }

        /// <summary>
        /// Not null, if the causes are determined to evaluate the result
        /// </summary>
        private bool? IsFollowingFromCauses(
            string modelInstanceId,
            Fact<TFactValue> fact)
        {
            var currentProvider = modelProvider.GetModelProvider(modelInstanceId);

            randomProvider.SetDynamicSeed(modelInstanceId, fact.Id);

            var evaluationResult = fact.CausesExpression.Evaluate(
                currentProvider,
                fixator,
                randomProvider);

            if (evaluationResult == null || !AreAllWeightCausesFixated(fact, currentProvider))
                return null;

            return evaluationResult.Value;
        }

        private bool AreAllWeightCausesFixated(
            Fact<TFactValue> fact,
            IModelProvider<TFactValue> provider)
            => fact.Weights == null || fact.Weights.All(x => x.CauseId == null || IsFixated(provider.GetModelFact(x.CauseId)));

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
                    fixatingFact.Fact);

                bool hasAbstractFactNotFixated =
                    fixatingFact.Fact.AbstractFactId != null
                    && !IsFixated(GetAbstractFact(fixatingFact));

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
            //var variants = causesTree.TryGetAbstractFactVariants(fixatingFact.InstanceFactId);
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
            => IsFixated(fact.InstanceFactId);

        private bool IsFixated(InstanceFactId instanceFactId)
        {
            return fixator.IsFixated(instanceFactId) != null;
        }

        private InstanceFact<TFactValue> GetAbstractFact(
            InstanceFact<TFactValue> fixatingFact)
        {
            if (fixatingFact.Fact.AbstractFactId == null)
                throw new InvalidOperationException();

            string modelInstanceId = fixatingFact.InstanceFactId.ModelInstanceId;
            return modelProvider.GetFact(
                new InstanceFactAddress(
                    fixatingFact.Fact.AbstractFactId,
                    modelInstanceId));
        }

        private void FixateAbstractFactVariants(InstanceFact<TFactValue> fixatingFact)
        {
            // Should never happen
            if (fixatingFact.Fact.AbstractFactId == null)
                throw new InvalidOperationException(
                    $"{nameof(FixateAbstractFactVariants)} is called for a fact that is not " +
                    "an implementation of an abstract fact");

            // Fixation of the abstract fact implementation variants

            var abstractFact = GetAbstractFact(fixatingFact);
            string modelInstanceId = fixatingFact.InstanceFactId.ModelInstanceId;

            var variantsFollowingFromCauses = GetVariantsFollowingFromCausesOrNull(
                abstractFact,
                modelInstanceId);

            // If there is not enough data for any implementation variant,
            // the selection of a single implementation is postponed
            if (variantsFollowingFromCauses == null)
            {
                return;
            }

            var isAbstractFactOccurred = fixator.IsFixated(abstractFact.InstanceFactId)
                ?? throw new InvalidOperationException(
                    $"{nameof(FixateAbstractFactVariants)} is called when the abstract fact " +
                    "of the variant was not fixated."); // Should never happen

            // If abstract fact was not occurred, any of its implementations won't occur too
            var selectedVariant = isAbstractFactOccurred
                ? SelectFactVariant(variantsFollowingFromCauses, modelInstanceId)
                : null;

            FixateAbstractFactVariantsCore(
                variantsFollowingFromCauses.Select(x => x.variant),
                selectedVariant);
        }

        private void FixateAbstractFactVariantsCore(
            IEnumerable<InstanceFact<TFactValue>> variants,
            InstanceFact<TFactValue>? selectedVariant)
        {
            foreach (var factVariant in variants)
            {
                // Also fixating variants not participated in the selection
                // because they obviously won't occur

                if (!IsFixated(factVariant))
                {
                    FixateFactCore(
                        factVariant,
                        ReferenceEquals(factVariant, selectedVariant));
                }
            }
        }

        private InstanceFact<TFactValue>? SelectFactVariant(
            IEnumerable<(InstanceFact<TFactValue> variant, bool canHappen)> variantsFollowingFromCauses,
            string modelInstanceId)
        {
            // The implementation is selected only from the candidates that have occurred
            var variantsCanOccur = variantsFollowingFromCauses
                .Where(x => x.canHappen == true)
                .Select(x => x.variant)
                .ToList();

            return WeightFactorUtils.SelectFactVariant(
                variantsCanOccur,
                fixator,
                randomProvider,
                modelProvider.GetModelProvider(modelInstanceId));
        }

        /// <summary>
        /// Null if there are some variants that were not fixated
        /// </summary>
        /// <returns></returns>
        private List<(InstanceFact<TFactValue> variant, bool canHappen)>? GetVariantsFollowingFromCausesOrNull(
            InstanceFact<TFactValue> abstractFact,
            string modelInstanceId)
        {
            var result = new List<(InstanceFact<TFactValue>, bool)>();
            foreach (var variant in causesTree.GetAbstractFactVariants(abstractFact.InstanceFactId))
            {
                bool? canHappen = IsFollowingFromCauses(
                    modelInstanceId,
                    variant.Fact);
                if (canHappen == null)
                {
                    return null;
                }
                result.Add((variant, canHappen.Value));
            }
            return result;
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
