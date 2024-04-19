using CausalModel.Blocks;
using CausalModel.Blocks.Resolving;
using CausalModel.Fixation.Fixators;
using CausalModel.Model;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation
{
    public class FixationFacadeBuilder<TFactValue> : IFixationFacadeFactory<TFactValue>
        where TFactValue : class
    {
        private CausalModel<TFactValue>? MainModel { get; set; }
        private BlockResolvingMap<TFactValue>? ResolvingMap { get; set; }

        private ModelInstanceFactory<TFactValue>? ModelInstanceFactory { get; set; }
        private ModelInstanceCreatedEventHandler<TFactValue>? onModelInstanceCreated;

        private BlockResolver<TFactValue>? BlockResolver { get; set; }
        private BlockImplementedEventHandler<TFactValue>? onBlockImplemented;

        private IFixator<TFactValue>? Fixator { get; set; }
        private FactFixatedEventHandler<TFactValue>? onFactFixated;

        public FixationFacadeBuilder() { }

        public FixationFacadeBuilder(CausalModel<TFactValue> mainModel)
        {
            this.MainModel = mainModel;
        }

        public FixationFacadeBuilder<TFactValue> WithResolvingMap(
            BlockResolvingMap<TFactValue> resolvingMap)
        {
            this.ResolvingMap = resolvingMap;
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
            where T : MulticastDelegate
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
            IFixator<TFactValue> fixator)
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
            if (MainModel == null)
                throw new InvalidOperationException("Main causal model is required " +
                    "for fixation facade building.");

            var modelInstanceFactory = ModelInstanceFactory
                ?? new ModelInstanceFactory<TFactValue>();

            if (onModelInstanceCreated != null)
            {
                modelInstanceFactory.ModelInstantiated -= onModelInstanceCreated;
                modelInstanceFactory.ModelInstantiated += onModelInstanceCreated;
            }

            var conventions = ResolvingMap ?? new BlockResolvingMap<TFactValue>();

            var blockResolver = BlockResolver ?? new BlockResolver<TFactValue>(
                conventions,
                modelInstanceFactory);

            var fixator = Fixator ?? new Fixator<TFactValue>();
            if (onFactFixated != null)
            {
                fixator.FactFixated -= onFactFixated;
                fixator.FactFixated += onFactFixated;
            }

            var res = new FixationFacade<TFactValue>(
                MainModel,
                conventions,
                modelInstanceFactory,
                blockResolver,
                fixator
                );
            return res;
        }

        FixationFacade<TFactValue> IFixationFacadeFactory<TFactValue>.Create()
        {
            return Build();
        }
    }
}
