using CausalModel.Factors;
using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel
{
    public class CausalModel<TNodeValue> : IHappenedProvider, IFixingValueProvider
    {
        public event Action<Fact<TNodeValue>>? FactHappened;

        private FactCollection<TNodeValue> causalData;
        private Dictionary<Guid, bool> factsHappened = new Dictionary<Guid, bool>();

        private Dictionary<Fact<TNodeValue>,
            List<Fact<TNodeValue>>> causesAndConsequences;
        private Dictionary<Fact<TNodeValue>,
            List<FactVariant<TNodeValue>>> factsAndVariants;
        private HashSet<Fact<TNodeValue>> rootNodes;

        private Random rnd;

        public CausalModel(FactCollection<TNodeValue> factCollection, int seed) {
            CausalData = factCollection;
            rnd = new Random(seed);
        }

        public FactCollection<TNodeValue> CausalData
        {
            get => causalData;
            private set
            {
                causalData = value;
                Initialize();
            }
        }

        public bool? IsHappened(Guid factId)
            => factsHappened.ContainsKey(factId) ? factsHappened[factId] : null;

        public float GetFixingValue()
        {
            float res = (float)rnd.NextDouble();
            return res;
        }

        public void Fixate(Guid factId, bool? factHappened = null)
        {
            Fact<TNodeValue> fact = causalData.GetFactById(factId);
            // true для выполнения условия следования факта из причин, необходимого,
            // но не всегда достаточного для происшествия
            bool followsFromCauses;

            // Если происшествие факта задается явно (например, событие уже
            // произошло в игре)
            if (factHappened != null)
            {
                followsFromCauses = factHappened.Value;
            } else  // Иначе происшествие определяется на основе причин
            {
                bool? isHappened = fact.ProbabilityNest.CausesExpression
                    .Evaluate(causalData, this, this);
                // Случай, когда недостаточно данных (некоторые причины
                // еще не зафиксированы)
                if (isHappened == null)
                {
                    return;
                }
                followsFromCauses = isHappened.Value;
            }

            // Для происшествия обычных фактов достаточно условия следования из причин
            if (!(fact is FactVariant<TNodeValue>))
            {
                FixateFact(fact, followsFromCauses);

                FixateNotFixedConsequences(fact);
            } else {
                // Вариант факта происходит только после того, как он будет
                // выбран в качестве единственной реализации абстрактного факта.

                var variant = (FactVariant<TNodeValue>)fact;
                var abstractFact = causalData.GetFactById(variant.AbstractFactId);

                var selectedVariant = SelectFactVariant(factsAndVariants[abstractFact]);
                foreach (var factVariant in factsAndVariants[abstractFact])
                {
                    FixateFact(factVariant, ReferenceEquals(factVariant,
                        selectedVariant));

                    FixateNotFixedConsequences(factVariant);
                }
            }
        }

        private void FixateFact(Fact<TNodeValue> fact, bool isHappened)
        {
            factsHappened[fact.Id] = isHappened;
            if (isHappened)
            {
                FactHappened?.Invoke(fact);
            }
        }

        private void FixateNotFixedConsequences(Fact<TNodeValue> fact)
        {
            if (!causesAndConsequences.ContainsKey(fact)) {
                return;
            }
            foreach (var consequence in causesAndConsequences[fact])
            {
                if (!factsHappened.ContainsKey(consequence.Id))
                    Fixate(consequence.Id);
            }
        }

        private void Initialize()
        {
            causesAndConsequences = new Dictionary<Fact<TNodeValue>,
                List<Fact<TNodeValue>>>();
            rootNodes = new HashSet<Fact<TNodeValue>>();
            factsAndVariants = new Dictionary<Fact<TNodeValue>,
                List<FactVariant<TNodeValue>>>();

            foreach (Fact<TNodeValue> fact in causalData.Nodes)
            {
                if (fact.IsRootNode())
                    rootNodes.Add(fact);

                if (fact is FactVariant<TNodeValue> variant)
                {
                    var abstractFact = causalData.GetFactById(variant.AbstractFactId);
                    if (!factsAndVariants.ContainsKey(abstractFact))
                    {
                        factsAndVariants.Add(abstractFact,
                            new List<FactVariant<TNodeValue>> { variant });
                    } else
                    {
                        factsAndVariants[abstractFact].Add(variant);
                    }
                }

                foreach (var edge in fact.GetEdges())
                {
                    if (edge.CauseId.HasValue)
                    {
                        var cause = causalData.GetFactById(edge.CauseId.Value);
                        if (!causesAndConsequences.ContainsKey(cause))
                        {
                            causesAndConsequences.Add(cause,
                                new List<Fact<TNodeValue>>() { fact });
                        }
                        else
                        {
                            causesAndConsequences[cause].Add(fact);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Случайно выбирает одну из реализаций факта, учитывая веса вариантов
        /// </summary>
        private FactVariant<TNodeValue>? SelectFactVariant(
            List<FactVariant<TNodeValue>> variants)
        {
            // Собрать информацию о узлах и их общих весах, собрать сумму весов,
            // а также отбросить узлы с нулевыми весами
            var nodesWeights = new List<(Fact<TNodeValue> node, double totalWeight)>();
            double weightsSum = 0;
            foreach (var node in variants)
            {
                double totalWeight = node.WeightNest.TotalWeight(this);
                if (totalWeight >= double.Epsilon)
                {
                    nodesWeights.Add((node, totalWeight));
                    weightsSum += totalWeight;
                }
            }
            if (weightsSum < double.Epsilon)
                return null;

            // Определить Id единственной реализации
            // Алгоритм Roulette wheel selection
            double choice = rnd.NextDouble(0, weightsSum);
            int curNodeIndex = -1;
            while (choice >= 0)
            {
                curNodeIndex++;
                if (curNodeIndex >= variants.Count)
                    curNodeIndex = 0;

                // choice -= nodes[curNodeIndex].WeightNest.TotalWeight();
                choice -= nodesWeights[curNodeIndex].totalWeight;
            }
            return variants[curNodeIndex];
        }

        public void FixateRoots()
        {
            foreach (var root in rootNodes)
            {
                Fixate(root.Id);
            }
        }
    }
}
