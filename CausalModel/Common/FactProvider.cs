using CausalModel.Blocks.Resolving;
using CausalModel.Facts;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Common
{
    public class FactProvider<TFactValue>
        where TFactValue : class
    {
        private readonly IEnumerable<CausalModel<TFactValue>> models;

        public FactProvider(CausalModel<TFactValue> model,
            BlockResolvingMap<TFactValue> blockResolvingMap)
        {
            models = blockResolvingMap
                .ModelsByDeclaredBlockId
                .Values
                .Concat(new[] { model })
                .Concat(blockResolvingMap.ModelsByConventionName.Values);
        }

        public FactProvider(IEnumerable<CausalModel<TFactValue>> models)
        {
            this.models = models;
        }

        public Fact<TFactValue> GetFact(string modelName, string factId)
        {
            var forDebug = models.ToList();

            return models
                .First(model => model.Name == modelName)
                .Facts
                .First(fact => fact.Id == factId);
        }
    }
}