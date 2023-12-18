using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation;
public class CausalGeneratorBuilder<TFactValue>
{
    public CausalModel<TFactValue> Model { get; private set; }
    public BlockResolvingMap<TFactValue> Conventions { get; private set; } = new();
    public ModelInstanceFactory<TFactValue> ModelInstanceFactory
        { get; private set; } = new();
    public BlockResolver<TFactValue>? BlockResolver { get; private set; }
    public Fixator<TFactValue> Fixator { get; private set; } = new();
    public int? Seed { get; private set; }

    public ResolvedModelWithCausesTree<TFactValue>? ResolvedModelProvider
    { get; private set; }

    public CausalGeneratorBuilder(CausalModel<TFactValue> model, int? seed = null)
    {
        this.Model = model;
        this.Seed = seed;
    }

    public CausalGeneratorBuilder<TFactValue> WithConventions(
        BlockResolvingMap<TFactValue> conventions)
    {
        this.Conventions = conventions;
        return this;
    }

    public CausalGeneratorBuilder<TFactValue> WithModelInstanceFactory(
        ModelInstanceFactory<TFactValue> modelInstanceFactory)
    {
        this.ModelInstanceFactory = modelInstanceFactory;
        return this;
    }

    public CausalGeneratorBuilder<TFactValue> WithBlockResolver(
        BlockResolver<TFactValue> blockResolver)
    {
        this.BlockResolver = blockResolver;
        return this;
    }

    public CausalGeneratorBuilder<TFactValue> WithFixator(
        Fixator<TFactValue> fixator)
    {
        this.Fixator = fixator;
        return this;
    }

    public CausalGenerator<TFactValue> Build()
    {
        this.BlockResolver ??= new BlockResolver<TFactValue>(
            Conventions,
            ModelInstanceFactory);
        var modelInstance = ModelInstanceFactory.InstantiateModel(Model);

        ResolvedModelProvider = new ResolvedModelWithCausesTree<TFactValue>(
            modelInstance, BlockResolver);

        return new CausalGenerator<TFactValue>(ResolvedModelProvider,
            ResolvedModelProvider.CausesTree, Fixator, Seed);
    }
}
