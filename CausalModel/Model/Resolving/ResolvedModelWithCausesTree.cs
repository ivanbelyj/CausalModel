using CausalModel.Blocks.Resolving;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving;
public class ResolvedModelWithCausesTree<TFactValue>
    : ResolvedModelProvider<TFactValue>
{
    public CausesTree<TFactValue> CausesTree { get; }

    private ResolvedModelWithCausesTree(ModelInstance<TFactValue> modelInstance,
        IBlockResolver<TFactValue> blockResolver, CausesTree<TFactValue> causesTree,
        ResolvedModelWithCausesTree<TFactValue>? parent)
        : base(modelInstance, blockResolver, parent)
    {
        CausesTree = causesTree;
    }

    public ResolvedModelWithCausesTree(ModelInstance<TFactValue> modelInstance,
        IBlockResolver<TFactValue> blockResolver)
        : this(modelInstance, blockResolver, new CausesTree<TFactValue>(), null)
    {

    }

    protected override ResolvedModelProvider<TFactValue> CreateResolvedBlock(
        ModelInstance<TFactValue> resolvedBlock,
        IBlockResolver<TFactValue> blockResolver)
    {
        var res = new ResolvedModelWithCausesTree<TFactValue>(
            resolvedBlock, blockResolver, CausesTree, this);
        CausesTree.AddModel(new ModelProvider<TFactValue>(res,
            resolvedBlock.InstanceId));
        return res;
    }
}
