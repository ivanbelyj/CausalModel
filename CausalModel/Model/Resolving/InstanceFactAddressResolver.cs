using CausalModel.Blocks;
using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var provider = resolvedModelProvider
                    .GetInstanceProvider(address.ModelInstanceId);

                // 1. Trying to get fact from the addressed model instance directly
                var factFromInstanceModel = TryGetFromRootModel(provider, address);

                // 2. In the second case the fact may be external cause
                // and located in the parent
                var externalFact = TryGetFromParent(provider, address);

                return factFromInstanceModel
                    ?? externalFact
                    // Otherwise, there is no such a block in the model
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

            private InstanceFactId? TryGetFromParent(
                ResolvedModelProvider<TFactValue> provider,
                InstanceFactAddress address)
            {
                address = GetActualExternalAddress(address, provider.DeclaredBlock);

                var externalIds = provider.rootModel.TryGetExternalCauses().ToList();
                if (externalIds != null)
                {
                    var externalCause = externalIds
                        .FirstOrDefault(fact => fact.Fact.Id == address.FactId);

                    return externalCause?.InstanceFactId;
                }
                return null;
            }

            private InstanceFactAddress GetActualExternalAddress(
                InstanceFactAddress address,
                DeclaredBlock? declaredBlock)
            {
                if (declaredBlock != null)
                {
                    var actualId = declaredBlock.TryGetActualExternalFactId(address.FactId);
                    if (actualId != null)
                    {
                        address = new InstanceFactAddress(actualId, address.ModelInstanceId);
                    }
                }
                return address;
            }
        }
    }
}
