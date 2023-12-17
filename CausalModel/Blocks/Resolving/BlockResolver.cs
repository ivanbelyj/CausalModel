using CausalModel.Facts;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks.Resolving;
public class BlockResolver<TFactValue> : BlockResolverBase<TFactValue>
{
    private readonly BlockResolvingMap<TFactValue> conventionsMap;

    public BlockResolver(BlockResolvingMap<TFactValue> conventionsMap,
        ModelInstanceFactory<TFactValue> modelInstanceFactory)
        : base(modelInstanceFactory)
    {
        this.conventionsMap = conventionsMap;
    }

    public override CausalModel<TFactValue> GetConventionImplementation(
        DeclaredBlock block,
        BlockConvention? convention)
    {
        CausalModel<TFactValue>? modelByConv = null;
        if (convention != null)
            conventionsMap
                .ModelsByConventionName
                .TryGetValue(convention.Name, out modelByConv);

        CausalModel<TFactValue>? modelByBlock;
        conventionsMap
            .ModelsByDeclaredBlockId
            .TryGetValue(block.Id, out modelByBlock);

        if (modelByConv == null && modelByBlock == null )
            throw new BlockResolvingException("Cannot select block implementation: "
                + $"no model provided for block (id: {block.Id}, "
                + $"convention: {convention?.Name})");

        return modelByBlock ?? modelByConv!;
    }
}
