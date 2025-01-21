using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Common.DataProviders
{
    public class FactsProvider<TFactValue> : IFactsProvider<TFactValue>
        where TFactValue : class
    {
        private readonly Dictionary<
            string,
            Dictionary<string, Fact<TFactValue>>> factsByLocalIdByModelId;

        public FactsProvider(IEnumerable<CausalModel<TFactValue>> models)
        {
            factsByLocalIdByModelId = models.ToDictionary(
                model => model.Name,
                model => model.Facts.ToDictionary(fact => fact.Id, fact => fact));
        }

        public Fact<TFactValue> GetFact(string modelName, string factId)
        {
            return factsByLocalIdByModelId[modelName][factId];
        }
    }
}