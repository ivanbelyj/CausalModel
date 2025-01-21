using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common.DataProviders
{
    public class DataProvidersBuilder<TFactValue>
        where TFactValue : class
    {
        public DataProvidersAggregator<TFactValue> Build(CausalBundle<TFactValue> bundle)
        {
            return new DataProvidersAggregator<TFactValue>(
                modelsProvider: new ModelsProvider<TFactValue>(bundle.CausalModels),
                conventionsProvider: new ConventionsProvider<TFactValue>(
                    bundle.BlockConventions,
                    bundle.BlockCausesConventions),
                factsProvider: new FactsProvider<TFactValue>(bundle.CausalModels));
        }
    }
}
