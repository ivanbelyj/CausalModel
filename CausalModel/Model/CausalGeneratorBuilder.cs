using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Fixation;
using CausalModel.Model.Instance;
using CausalModel.Model.ResolvingModelProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;
public class CausalGeneratorBuilder<TFactValue>
{
    private readonly CausalModel<TFactValue> model;
    private BlockResolvingMap<TFactValue>? conventions;
    private ModelInstanceFactory<TFactValue>? modelInstanceFactory;
    private BlockResolver<TFactValue>? blockResolver;
    private Fixator<TFactValue>? fixator;
    private readonly int? seed;

    public ResolvedModelWithCausesTree<TFactValue>? ResolvedModelProvider
        { get; private set; }

    public CausalGeneratorBuilder(CausalModel<TFactValue> model, int? seed = null)
    {
        this.model = model;
        this.seed = seed;
    }

    public CausalGeneratorBuilder<TFactValue> WithConventions(
        BlockResolvingMap<TFactValue> conventions)
    {
        this.conventions = conventions;
        return this;
    }

    public CausalGeneratorBuilder<TFactValue> WithModelInstanceFactory(
        ModelInstanceFactory<TFactValue> modelInstanceFactory)
    {
        this.modelInstanceFactory = modelInstanceFactory;
        return this;
    }

    public CausalGeneratorBuilder<TFactValue> WithBlockResolver(
        BlockResolver<TFactValue> blockResolver)
    {
        this.blockResolver = blockResolver;
        return this;
    }

    public CausalGeneratorBuilder<TFactValue> WithFixator(
        Fixator<TFactValue> fixator)
    {
        this.fixator = fixator;
        return this;
    }

    public CausalGenerator<TFactValue> Build()
    {
        var modelInstanceFactory = this.modelInstanceFactory
            ?? new ModelInstanceFactory<TFactValue>();
        var blockResolver = this.blockResolver ?? new BlockResolver<TFactValue>(
            conventions ?? new BlockResolvingMap<TFactValue>(),
            modelInstanceFactory);
        var modelInstance = modelInstanceFactory.InstantiateModel(model);

        ResolvedModelProvider = new ResolvedModelWithCausesTree<TFactValue>(
            modelInstance, blockResolver);

        fixator ??= new Fixator<TFactValue>();

        return new CausalGenerator<TFactValue>(ResolvedModelProvider,
            ResolvedModelProvider.CausesTree, fixator, seed);
    }
}
