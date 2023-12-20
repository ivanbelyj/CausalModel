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
    public CausalModel<TFactValue> MainModel { get; private set; }
    public int? Seed { get; private set; }
    public BlockResolvingMap<TFactValue> Conventions { get; private set; } = new();
    public ModelInstanceFactory<TFactValue> ModelInstanceFactory { get; private set; }
        = new();

    public IBlockResolver<TFactValue> BlockResolver { get; private set; }
    public IFixator<TFactValue> Fixator { get; private set; }

    public IResolvedModelProvider<TFactValue> ResolvedModelProvider
        { get; private set; }
    public CausalGenerator<TFactValue> Generator { get; private set; }

    public FixationFacade(
        CausalModel<TFactValue> mainModel,
        int? seed,
        BlockResolvingMap<TFactValue> conventions,

        IBlockResolver<TFactValue> blockResolver,
        IFixator<TFactValue> fixator,
        
        IResolvedModelProvider<TFactValue> resolvedModelProvider,
        ICausesTree<TFactValue> causesTree)
    {
        MainModel = mainModel;
        Seed = seed;
        Conventions = conventions;
        ModelInstanceFactory = new();

        BlockResolver = blockResolver;
        Fixator = fixator;

        ResolvedModelProvider = resolvedModelProvider;

        Generator = new CausalGenerator<TFactValue>(resolvedModelProvider,
            causesTree, fixator, Seed);
    }
}
