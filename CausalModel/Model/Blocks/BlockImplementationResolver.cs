using CausalModel.Facts;
using CausalModel.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Blocks;
public class BlockImplementationResolver<TFactValue>
{
    private readonly BlockConventionMap<TFactValue> conventionsMap;

    public BlockImplementationResolver(BlockConventionMap<TFactValue> conventionsMap)
    {
        this.conventionsMap = conventionsMap;
    }
    public ResolvingModelProvider<TFactValue> Resolve(BlockFact blockFact)
    {
        var model = conventionsMap.ModelsByConventionName[blockFact.ConventionName];
        return new ResolvingModelProvider<TFactValue>(model, this);
    }
}
