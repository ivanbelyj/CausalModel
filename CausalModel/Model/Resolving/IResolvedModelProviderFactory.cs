using CausalModel.Blocks.Resolving;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    internal interface IResolvedModelProviderFactory<TFactValue>
        where TFactValue : class
    {
        ResolvedModelProvider<TFactValue> CreateResolvedModel(
            ModelInstance<TFactValue> modelInstance,
            IBlockResolver<TFactValue> blockResolver);
    }
}
