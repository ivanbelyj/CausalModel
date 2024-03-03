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
                var factFromInstanceModel = provider.rootModel
                    .TryGetFact(address.FactId);

                if (factFromInstanceModel != null)
                {
                    return factFromInstanceModel.InstanceFactId;
                }

                // 2. In the second case the fact may be external cause
                // and located in the parent
                var externalIds = provider.rootModel.TryGetExternalCauses();
                if (externalIds != null)
                {
                    var externalCause = externalIds
                        .FirstOrDefault(fact => fact.Fact.Id == address.FactId);

                    if (externalCause != null)
                        return externalCause.InstanceFactId;
                }

                // Otherwise, there is no such a block in the model
                throw new InvalidOperationException($"The fact was not found by address"
                    + $" ({address}) in the resolved model.");
            }

            //private bool IsFactExternal(string modelInstanceId, string factId)
            //{
            //    return rootInstance
            //        .Model
            //        .BlockConventions
            //        ?.SelectMany(x => x.Consequences ?? new List<BaseFact>())
            //        .Select(x => x.Id)
            //        .Contains(id.FactId) ?? false;
            //}
        }
    }
}
