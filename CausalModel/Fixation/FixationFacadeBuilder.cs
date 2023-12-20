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
public class FixationFacadeBuilder<TFactValue> : IFixationFacadeFactory<TFactValue>
{
    private CausalModel<TFactValue>? Model { get; set; }
    public int? Seed { get; private set; }
    private BlockResolvingMap<TFactValue>? Conventions { get; set; }

    public ModelInstanceFactory<TFactValue>? ModelInstanceFactory { get; set; }
    private ModelInstanceCreatedEventHandler<TFactValue>? onModelInstanceCreated;

    private BlockResolver<TFactValue>? BlockResolver { get; set; }
    private BlockImplementedEventHandler<TFactValue>? onBlockImplemented;
    
    private Fixator<TFactValue>? Fixator { get; set; }
    private FactFixatedEventHandler<TFactValue>? onFactFixated;

    public FixationFacadeBuilder() { }

    public FixationFacadeBuilder(CausalModel<TFactValue> model, int? seed = null)
    {
        this.Model = model;
        this.Seed = seed;
    }

    public FixationFacadeBuilder<TFactValue> WithConventions(
        BlockResolvingMap<TFactValue> conventions)
    {
        this.Conventions = conventions;
        return this;
    }

    public FixationFacadeBuilder<TFactValue> WithModelInstanceFactory(
        ModelInstanceFactory<TFactValue> modelInstanceFactory)
    {
        this.ModelInstanceFactory = modelInstanceFactory;
        return this;
    }

    private FixationFacadeBuilder<TFactValue> SetOrAddAndReturnThis<T>(
        ref T? delegateProperty, T value)
        where T: MulticastDelegate
    {
        if (delegateProperty == null)
            delegateProperty = value;
        else delegateProperty = (T)Delegate.Combine(delegateProperty, value);
        return this;
    }

    public FixationFacadeBuilder<TFactValue> AddOnModelInstanceCreated(
        ModelInstanceCreatedEventHandler<TFactValue> eventHandler)
    {
        return SetOrAddAndReturnThis(ref onModelInstanceCreated, eventHandler);
    }

    public FixationFacadeBuilder<TFactValue> WithBlockResolver(
        BlockResolver<TFactValue> blockResolver)
    {
        this.BlockResolver = blockResolver;
        return this;
    }

    public FixationFacadeBuilder<TFactValue> AddOnBlockImplemented(
        BlockImplementedEventHandler<TFactValue> eventHandler)
    {
        return SetOrAddAndReturnThis(ref onBlockImplemented, eventHandler);
    }

    public FixationFacadeBuilder<TFactValue> WithFixator(
        Fixator<TFactValue> fixator)
    {
        this.Fixator = fixator;
        return this;
    }

    public FixationFacadeBuilder<TFactValue> AddOnFactFixated(
        FactFixatedEventHandler<TFactValue> eventHandler)
    {
        return SetOrAddAndReturnThis(ref onFactFixated, eventHandler);
    }

    public FixationFacade<TFactValue> Build()
    {
        if (Model == null)
            throw new InvalidOperationException("Main causal model is required " +
                "for fixation facade building.");

        var modelInstanceFactory = ModelInstanceFactory ?? new();

        if (onModelInstanceCreated != null)
        {
            modelInstanceFactory.ModelInstantiated -= onModelInstanceCreated;
            modelInstanceFactory.ModelInstantiated += onModelInstanceCreated;
        }

        var conventions = Conventions ?? new();

        var blockResolver = BlockResolver ?? new BlockResolver<TFactValue>(
            conventions,
            modelInstanceFactory);

        var modelInstance = modelInstanceFactory.InstantiateModel(Model);

        var resolvedModelProvider = new ResolvedModelWithCausesTree<TFactValue>(
            modelInstance, blockResolver);

        var fixator = Fixator ?? new();
        if (onFactFixated != null)
        {
            fixator.FactFixated -= onFactFixated;
            fixator.FactFixated += onFactFixated;
        }

        var res = new FixationFacade<TFactValue>(Model,
            Seed,
            conventions,
            blockResolver,
            fixator,
            resolvedModelProvider,
            resolvedModelProvider.CausesTree);
        return res;
    }

    FixationFacade<TFactValue> IFixationFacadeFactory<TFactValue>.Create()
    {
        return Build();
    }
}