using CausalModel.Blocks.Resolving;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Fixation.Fixators;

namespace CausalModel.Fixation
{
    public class FixationFacade<TFactValue>
        where TFactValue : class
    {
        public CausalModel<TFactValue> MainModel { get; }
        public BlockResolvingMap<TFactValue> Conventions { get; }
        public ModelInstanceFactory<TFactValue> ModelInstanceFactory { get; }

        public IBlockResolver<TFactValue> BlockResolver { get; }
        public IFixator<TFactValue> Fixator { get; }

        public FixationFacade(
            CausalModel<TFactValue> mainModel,
            BlockResolvingMap<TFactValue> conventions,
            ModelInstanceFactory<TFactValue> modelInstanceFactory,

            IBlockResolver<TFactValue> blockResolver,
            IFixator<TFactValue> fixator
            )
        {
            MainModel = mainModel;
            Conventions = conventions;
            ModelInstanceFactory = modelInstanceFactory;

            BlockResolver = blockResolver;
            Fixator = fixator;
        }

        public CausalGenerator<TFactValue> CreateGenerator(int? seed = null)
        {
            var mainModelInstance = ModelInstanceFactory.InstantiateModel(MainModel);

            var resolvedModelProvider = new ResolvedModelWithCausesTree<TFactValue>(
                mainModelInstance, BlockResolver);
            Fixator.Initialize(resolvedModelProvider);

            var generator = new CausalGenerator<TFactValue>(resolvedModelProvider,
                resolvedModelProvider.CausesTree, Fixator, seed);
            return generator;
        }
    }
}
