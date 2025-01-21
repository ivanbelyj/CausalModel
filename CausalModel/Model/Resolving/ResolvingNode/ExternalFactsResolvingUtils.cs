using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Model.Resolving.ResolvingNode
{
    internal static class ExternalFactsResolvingUtils<TFactValue>
        where TFactValue : class
    {
        public static InstanceFact<TFactValue>?
            MappedLocalConsequenceIdToExternalLocalAndGetFactInRootInstance(
            ResolvedModelProvider<TFactValue> block,
            string externalId)
        {
            if (block.DeclaredBlock == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(block.DeclaredBlock)} property must be defined for every block"
                );
            }

            // In terms relative to the block
            var consequenceLocalId = block.DeclaredBlock.MappedExternalToLocal(externalId);
            if (consequenceLocalId == null)
            {
                return null;
            }
            return block.TryGetFactInRootInstance(consequenceLocalId);
        }
    }
}
