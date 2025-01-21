using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Blocks.Resolving
{
    public class BlockImplementationSelector : IBlockImplementationSelector
    {
        private readonly BlockResolvingMap blockResolvingMap;

        public BlockImplementationSelector(BlockResolvingMap blockResolvingMap)
        {
            this.blockResolvingMap = blockResolvingMap;
        }

        public string GetImplementationModelName(DeclaredBlock declaredBlock)
        {
            string? modelNameByConvention = null;

            if (declaredBlock.Convention != null)
            {
                blockResolvingMap
                    .ModelNamesByConventionName
                    .TryGetValue(declaredBlock.Convention, out modelNameByConvention);
            }

            string? modelNameByBlockId;
            blockResolvingMap
                .ModelNamesByDeclaredBlockId
                .TryGetValue(declaredBlock.Id, out modelNameByBlockId);

            if (modelNameByConvention == null && modelNameByBlockId == null)
            {
                throw new BlockResolvingException(
                    "Cannot select block implementation: "
                    + $"no model name mapped for block (id: {declaredBlock.Id}, "
                    + $"convention: {declaredBlock.Convention})");
            }

            return modelNameByBlockId ?? modelNameByConvention!;
        }
    }
}
