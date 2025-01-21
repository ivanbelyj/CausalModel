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

                return
                    TryGetFromRootModel(provider, address)
                    ?? TryGetExternalFactId(provider, address)
                    ?? throw new InvalidOperationException(
                        $"The fact was not found by address"
                        + $" ({address}) in the resolved model.");
            }

            private InstanceFactId? TryGetFromRootModel(
                ResolvedModelProvider<TFactValue> provider,
                InstanceFactAddress address)
            {
                var factFromInstanceModel = provider.rootModel.TryGetFact(address.FactId);
                return factFromInstanceModel?.InstanceFactId;
            }

            /// <param name="address">
            /// External cause (addressed locally for us) or a consequence
            /// of one of our blocks (addressed mapped for us and local for the block)
            /// </param>
            private InstanceFactId? TryGetExternalFactId(
                ResolvedModelProvider<TFactValue> provider,
                InstanceFactAddress address)
            {
                return (provider.rootModel.TryGetConsequenceFromBlocks(address)
                    ?? provider.rootModel.TryGetCauseFromParent(address))
                    ?.InstanceFactId;
            }
        }
    }
}
