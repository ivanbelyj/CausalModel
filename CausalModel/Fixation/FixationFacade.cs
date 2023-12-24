using CausalModel.Blocks.Resolving;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation;
public class FixationFacade<TFactValue>
{
    public CausalModel<TFactValue> MainModel { get; }
    //public ModelInstance<TFactValue> MainModelInstance { get; private set; }
    public BlockResolvingMap<TFactValue> Conventions { get; }
    public ModelInstanceFactory<TFactValue> ModelInstanceFactory { get; }

    public IBlockResolver<TFactValue> BlockResolver { get; }
    public IFixator<TFactValue> Fixator { get; }

    //public IResolvedModelProvider<TFactValue> ResolvedModelProvider
    //    { get; private set; }
    //public CausalGenerator<TFactValue> Generator { get; private set; }

    public FixationFacade(
        CausalModel<TFactValue> mainModel,
        //ModelInstance<TFactValue> mainModelInstance,
        BlockResolvingMap<TFactValue> conventions,
        ModelInstanceFactory<TFactValue> modelInstanceFactory,

        IBlockResolver<TFactValue> blockResolver,
        IFixator<TFactValue> fixator
        
        //IResolvedModelProvider<TFactValue> resolvedModelProvider,
        //CausesTree<TFactValue> causesTree
        )
    {
        MainModel = mainModel;
        //MainModelInstance = mainModelInstance;
        Conventions = conventions;
        ModelInstanceFactory = modelInstanceFactory;

        // Todo: make it work
        //ModelInstanceFactory.ModelInstantiated += (sender, model) =>
        //{
        //    Console.WriteLine("!!!model instantiated (callback added" +
        //        "in the facade constructor)");
        //};

        BlockResolver = blockResolver;
        Fixator = fixator;

        //ResolvedModelProvider = resolvedModelProvider;

        //Generator = new CausalGenerator<TFactValue>(resolvedModelProvider,
        //    causesTree, fixator, Seed);
    }

    public CausalGenerator<TFactValue> CreateGenerator(int? seed = null)
    {
        var mainModelInstance = ModelInstanceFactory.InstantiateModel(MainModel);

        var resolvedModelProvider = new ResolvedModelWithCausesTree<TFactValue>(
            mainModelInstance, BlockResolver);

        var generator = new CausalGenerator<TFactValue>(resolvedModelProvider,
            resolvedModelProvider.CausesTree, Fixator, seed);
        return generator;
    }
}
