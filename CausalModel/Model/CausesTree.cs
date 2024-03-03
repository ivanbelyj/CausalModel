using CausalModel.Facts;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model
{

    /// <summary>
    /// Represents given models as a tree that allows to traverse from causes
    /// to consequences
    /// </summary>
    public class CausesTree<TFactValue> : ICausesTree<TFactValue>
        where TFactValue : class
    {
        public Dictionary<InstanceFactId, List<InstanceFact<TFactValue>>>
            ConsequencesByCauseIds { get; private set; }
            = new Dictionary<InstanceFactId, List<InstanceFact<TFactValue>>>();
        public Dictionary<InstanceFactId, List<InstanceFact<TFactValue>>>
            VariantsByAbstractFactIds { get; private set; }
            = new Dictionary<InstanceFactId, List<InstanceFact<TFactValue>>>();

        public void AddModel(IModelProvider<TFactValue> modelProvider)
        {
            AddVariants(modelProvider);
            AddCausesAndConsequences(modelProvider);
        }

        private void AddVariants(IModelProvider<TFactValue> modelProvider)
        {
            foreach (InstanceFact<TFactValue> instanceFact in modelProvider
                .GetInstanceFacts())
            {
                var fact = instanceFact.Fact;
                if (fact.AbstractFactId != null)
                {
                    var abstractFact = modelProvider.GetModelFact(fact.AbstractFactId);
                    if (!VariantsByAbstractFactIds
                        .ContainsKey(abstractFact.InstanceFactId))
                    {
                        VariantsByAbstractFactIds.Add(abstractFact.InstanceFactId,
                            new List<InstanceFact<TFactValue>> { instanceFact });
                    }
                    else
                    {
                        VariantsByAbstractFactIds[abstractFact.InstanceFactId]
                            .Add(instanceFact);
                    }
                }
            }
        }

        private void AddCausesAndConsequences(IModelProvider<TFactValue> modelProvider)
        {
            foreach (InstanceFact<TFactValue> instanceFact in modelProvider
                .GetInstanceFacts())
            {
                var fact = instanceFact.Fact;
                foreach (var edge in fact.GetCauses())
                {
                    if (edge.CauseId != null)
                    {
                        ProcessCauseAndConsequence(edge.CauseId,
                            instanceFact, modelProvider);
                    }
                }
            }
        }

        private void ProcessCauseAndConsequence(string causeLocalId,
            InstanceFact<TFactValue> consequence,
            IModelProvider<TFactValue> modelProvider)
        {
            InstanceFact<TFactValue> causeFact = modelProvider.GetModelFact(causeLocalId);

            InstanceFactId causeFactId = causeFact.InstanceFactId;
            if (!ConsequencesByCauseIds.ContainsKey(causeFactId))
            {
                ConsequencesByCauseIds.Add(causeFactId,
                    new List<InstanceFact<TFactValue>>() { consequence });
            }
            else
            {
                ConsequencesByCauseIds[causeFactId].Add(consequence);
            }
        }

        public IEnumerable<InstanceFact<TFactValue>> GetAbstractFactVariants(
            InstanceFactId abstractFactId)
        {
            return VariantsByAbstractFactIds[abstractFactId];
        }

        public IEnumerable<InstanceFact<TFactValue>>? TryGetConsequences(InstanceFactId id)
        {
            ConsequencesByCauseIds.TryGetValue(id, out var res);
            return res;
        }
    }
}
