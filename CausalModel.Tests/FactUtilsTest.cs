using CausalModel.FactCollection;
using CausalModel.Factors;
using CausalModel.Nests;
using CausalModel.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CausalModel.Tests
{
    public class FactUtilsTest
    {
        [Fact]
        public void CreateNodeTest()
        {
            string val = "Root node";
            var node = FactUtils.CreateNode(1, val, null);
            Assert.NotEqual(node.Id, default);
            Assert.Equal(val, node.NodeValue);
            Assert.Single(node.ProbabilityNest.GetEdges());
            Assert.Equal(1f, node.ProbabilityNest.GetEdges().ElementAt(0).Probability);
        }

        [Fact]
        public void CreateAbstractFactTest()
        {
            // Arrange
            var model = new FactCollection<string>();
            var abstractNode = new Fact<string>(new ProbabilityNest(1, null),
                "Kemsh (race)");
            var races = new string[] {"Cheaymea", "Emera", "Evoymea",
                "Myeuramea", "Oanei" };

            // Act
            var facts = FactUtils.CreateAbstractFact(abstractNode, races);

            // Assert
            Assert.Equal(races.Length + 1, facts.Count);

            foreach (var node in model.Nodes)
            {
                if (node == abstractNode)
                    continue;
                var nodeEdges = node.GetEdges();
                var weightEdges = nodeEdges.OfType<WeightFactor>();
                var probabilityEdges = nodeEdges.OfType<ProbabilityFactor>();
                Assert.Single(weightEdges);
                Assert.Single(probabilityEdges);
            }
        }
    }
}
