using CausalModel.Factors;
using CausalModel.Nests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Nodes
{
    public class Fact<TNodeValue>
    {
        public Guid Id { get; set; }
        
        public ProbabilityNest ProbabilityNest { get; set; }

        /// <summary>
        /// Если null, то данное звено – только связующее
        /// </summary>
        public TNodeValue? NodeValue { get; set; }

        // Для десериализации
        public Fact() : this(new ProbabilityNest(), default) { }
        public Fact(Guid id, ProbabilityNest probabilityNest,
            TNodeValue? nodeValue)
        {
            Id = id;
            NodeValue = nodeValue;
            ProbabilityNest = probabilityNest;
        }
        public Fact(ProbabilityNest probabilityNest, TNodeValue? value)
            : this(Guid.NewGuid(), probabilityNest, value) { }

        /// <summary>
        /// Все исходящие причинные ребра. В подклассах могут добавиться другие гнезда,
        /// поэтому метод можно переопределять
        /// </summary>
        public virtual IEnumerable<CausalEdge> GetEdges() => ProbabilityNest.GetEdges();
        public virtual bool IsRootNode() => ProbabilityNest.IsRootNest();

        public override string? ToString() => $"{Id} - " + (NodeValue?.ToString() ?? "null");

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
