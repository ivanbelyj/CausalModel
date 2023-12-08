using CausalModel.Facts;
using CausalModel.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Blocks;
public class BlockResolver<TFactValue>
{
    private readonly ConventionsMap<TFactValue> conventionsMap;

    public BlockResolver(ConventionsMap<TFactValue> conventionsMap)
    {
        this.conventionsMap = conventionsMap;
    }
    public ResolvingModelProvider<TFactValue> Resolve(BlockFact blockFact)
    {
        var model = conventionsMap.ModelsByConventionName[blockFact.ConventionName];
        return new ResolvingModelProvider<TFactValue>(model, this);
    }
}
