using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common.DataProviders
{
    public class DataProvidersAggregator<TFactValue>
        where TFactValue : class
    {
        public IModelsProvider<TFactValue> ModelsProvider { get; }
        public IConventionsProvider ConventionsProvider { get; }
        public IFactsProvider<TFactValue> FactsProvider { get; }

        public DataProvidersAggregator(
            IModelsProvider<TFactValue> modelsProvider,
            IConventionsProvider conventionsProvider,
            IFactsProvider<TFactValue> factsProvider)
        {
            ModelsProvider = modelsProvider;
            ConventionsProvider = conventionsProvider;
            FactsProvider = factsProvider;
        }
    }
}
