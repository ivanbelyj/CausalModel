using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks.Resolving;

public delegate void BlockImplementedEventHandler<TFactValue>(
    object sender,
    DeclaredBlock block,
    BlockConvention? convention,
    ModelInstance<TFactValue> implementation);

/// <summary>
/// A component responsible for resolving blocks that occure during the fixation
/// of the causal model. It accepts a block and its parent and returns actual
/// causal model implementing it
/// </summary>
public interface IBlockResolver<TFactValue>
{
    event BlockImplementedEventHandler<TFactValue> BlockImplemented;
    ModelInstance<TFactValue> Resolve(DeclaredBlock block,
        ModelInstance<TFactValue> parentInstance);
}
