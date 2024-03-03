using CausalModel.Blocks.Resolving;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Resolving
{
    /// <summary>
    /// A component resolving blocks of the causal model and providing causes tree based
    /// on it
    /// </summary>
    public class ResolvedModelWithCausesTree<TFactValue>
        : ResolvedModelProvider<TFactValue>
        where TFactValue : class
    {
        private readonly ModelProvider<TFactValue> initialModelProvider;
        public CausesTree<TFactValue> CausesTree
        {
            get; private set;
        }

        private ResolvedModelWithCausesTree(ModelInstance<TFactValue> modelInstance,
            IBlockResolver<TFactValue> blockResolver,
            CausesTree<TFactValue> causesTree,
            ResolvedModelWithCausesTree<TFactValue>? parent)
            : base(modelInstance, blockResolver, parent)
        {
            CausesTree = causesTree;

            initialModelProvider = new ModelProvider<TFactValue>(this,
                modelInstance.InstanceId);

            InitCausesTree();
        }

        private void InitCausesTree()
        {
            CausesTree.AddModel(initialModelProvider);
        }

        public ResolvedModelWithCausesTree(ModelInstance<TFactValue> modelInstance,
            IBlockResolver<TFactValue> blockResolver)
            : this(modelInstance, blockResolver, new CausesTree<TFactValue>(), null)
        {

        }

        public override ResolvedModelProvider<TFactValue> CreateResolvedModel(
            ModelInstance<TFactValue> resolvedBlock,
            IBlockResolver<TFactValue> blockResolver)
        {
            var res = new ResolvedModelWithCausesTree<TFactValue>(
                resolvedBlock, blockResolver, CausesTree, this);

            return res;
        }
    }
}
