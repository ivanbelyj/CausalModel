using CausalModel.Blocks.Resolving;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving;
internal interface IResolvedModelProviderFactory<TFactValue>
{
    ResolvedModelProvider<TFactValue> CreateResolvedBlock(
        ModelInstance<TFactValue> resolvedBlock,
        IBlockResolver<TFactValue> blockResolver);
}
