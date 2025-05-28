using CausalModel.Blocks;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving.ResolvingNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    partial class ResolvedModelProvider<TFactValue>
    {
        /// <summary>
        /// Resolving InstanceFactAddress to actual unique InstanceFactId
        /// </summary>
        internal class InstanceFactAddressResolver
        {
            private readonly ResolvedModelProvider<TFactValue> resolvedModelProvider;

            public InstanceFactAddressResolver(
                ResolvedModelProvider<TFactValue> resolvedModelProvider)
            {
                this.resolvedModelProvider = resolvedModelProvider;
            }

            public InstanceFactId Resolve(InstanceFactAddress address)
            {
                var provider = resolvedModelProvider.GetInstanceProvider(
                    address.ModelInstanceId);

                return TryGetFactId(provider, address)
                    ?? throw new InvalidOperationException(
                        $"The fact was not found by address"
                        + $" ({address}) in the resolved model.");
            }

            /// <param name="address">
            /// External cause (addressed locally for us), consequence
            /// of one of our blocks (addressed mapped for us and local for the block)
            /// or a fact from the root model
            /// </param>
            private InstanceFactId? TryGetFactId(
                ResolvedModelProvider<TFactValue> provider,
                InstanceFactAddress address)
            {
                return (
                    provider.rootModel.TryGetCauseFromParent(address)
                    ?? provider.rootModel.TryGetFact(address.FactId)
                    ?? provider.rootModel.TryGetConsequenceFromBlocks(address))
                    ?.InstanceFactId;
            }
        }
    }
}
