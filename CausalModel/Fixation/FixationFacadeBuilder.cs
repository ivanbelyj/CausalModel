using CausalModel.Blocks.Resolving;
using CausalModel.Common;
using CausalModel.Common.DataProviders;
using CausalModel.Fixation.Fixators;
using CausalModel.Model.Instance;
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
        private CausalBundle<TFactValue>? CausalBundle { get; set; }

        private ModelInstanceFactory<TFactValue>? ModelInstanceFactory { get; set; }
        private ModelInstanceCreatedEventHandler<TFactValue>? onModelInstanceCreated;

        private BlockResolver<TFactValue>? BlockResolver { get; set; }
        private BlockImplementedEventHandler<TFactValue>? onBlockImplemented;

        private IBlockImplementationSelector? BlockImplementationSelector { get; set; }

        private IFixator<TFactValue>? Fixator { get; set; }

        private FactFixatedEventHandler<TFactValue>? onFactFixated;

        public FixationFacadeBuilder() { }

        public FixationFacadeBuilder(CausalBundle<TFactValue> causalBundle)
        {
            CausalBundle = causalBundle;
        }

        public FixationFacadeBuilder<TFactValue> UseModelInstanceFactory(
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

        public FixationFacadeBuilder<TFactValue> UseBlockResolver(
            BlockResolver<TFactValue> blockResolver)
        {
            BlockResolver = blockResolver;
            return this;
        }

        public FixationFacadeBuilder<TFactValue> UseBlockImplementationSelector(
            IBlockImplementationSelector blockImplementationSelector)
        {
            BlockImplementationSelector = blockImplementationSelector;
            return this;
        }

        public FixationFacadeBuilder<TFactValue> AddOnBlockImplemented(
            BlockImplementedEventHandler<TFactValue> eventHandler)
        {
            return SetOrAddAndReturnThis(ref onBlockImplemented, eventHandler);
        }

        public FixationFacadeBuilder<TFactValue> UseFixator(
            IFixator<TFactValue> fixator)
        {
            Fixator = fixator;
            return this;
        }

        public FixationFacadeBuilder<TFactValue> AddOnFactFixated(
            FactFixatedEventHandler<TFactValue> eventHandler)
        {
            return SetOrAddAndReturnThis(ref onFactFixated, eventHandler);
        }

        public FixationFacade<TFactValue> Build()
        {
            if (CausalBundle == null)
            {
                throw new InvalidOperationException(
                    $"Cannot build {nameof(FixationFacade<TFactValue>)}" +
                    $"with {nameof(CausalBundle)} not set.");
            }

            var modelInstanceFactory = ModelInstanceFactory
                ?? new ModelInstanceFactory<TFactValue>();

            if (onModelInstanceCreated != null)
            {
                modelInstanceFactory.ModelInstantiated -= onModelInstanceCreated;
                modelInstanceFactory.ModelInstantiated += onModelInstanceCreated;
            }

            var dataProviders = new DataProvidersBuilder<TFactValue>().Build(CausalBundle);

            var blockResolver = BlockResolver ?? new BlockResolver<TFactValue>(
                modelInstanceFactory,
                dataProviders.ConventionsProvider,
                dataProviders.ModelsProvider,
                BlockImplementationSelector
                    ?? new BlockImplementationSelector(CausalBundle.BlockResolvingMap));

            var fixator = Fixator ?? new Fixator<TFactValue>();
            if (onFactFixated != null)
            {
                fixator.FactFixated -= onFactFixated;
                fixator.FactFixated += onFactFixated;
            }

            var res = new FixationFacade<TFactValue>(
                modelInstanceFactory,
                blockResolver,
                fixator,
                dataProviders.ModelsProvider,
                CausalBundle.DefaultMainModel
                );
            return res;
        }

        FixationFacade<TFactValue> IFixationFacadeFactory<TFactValue>.Create()
        {
            return Build();
        }
    }
}
