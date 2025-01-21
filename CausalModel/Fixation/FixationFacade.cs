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
using CausalModel.Common;
using CausalModel.Common.DataProviders;

namespace CausalModel.Fixation
{
    public class FixationFacade<TFactValue>
        where TFactValue : class
    {
        public ModelInstanceFactory<TFactValue> ModelInstanceFactory { get; }

        public IBlockResolver<TFactValue> BlockResolver { get; }
        public IFixator<TFactValue> Fixator { get; }
        private IModelsProvider<TFactValue> ModelsProvider { get; }
        private string? DefaultMainModelName { get; }

        public FixationFacade(
            ModelInstanceFactory<TFactValue> modelInstanceFactory,
            IBlockResolver<TFactValue> blockResolver,
            IFixator<TFactValue> fixator,
            IModelsProvider<TFactValue> modelsProvider,
            string? defaultMainModelName)
        {
            ModelInstanceFactory = modelInstanceFactory;

            BlockResolver = blockResolver;
            Fixator = fixator;
            ModelsProvider = modelsProvider;
            DefaultMainModelName = defaultMainModelName;
        }

        public CausalGenerator<TFactValue> CreateGenerator(
            int? seed = null,
            string? mainModelName = null)
        {
            var mainModelInstance = ModelInstanceFactory.InstantiateModel(
                GetMainModel(mainModelName));

            var resolvedModelProvider = new ResolvedModelWithCausesTree<TFactValue>(
                mainModelInstance,
                BlockResolver);
            Fixator.Initialize(resolvedModelProvider);

            var generator = new CausalGenerator<TFactValue>(
                resolvedModelProvider,
                resolvedModelProvider.CausesTree,
                Fixator,
                seed);
            return generator;
        }

        private CausalModel<TFactValue> GetMainModel(string? mainModelName)
        {
            return ModelsProvider.GetModelByName(
                mainModelName
                ?? DefaultMainModelName
                ?? throw new InvalidOperationException($"Main model name is not defined."));
        }
    }
}
