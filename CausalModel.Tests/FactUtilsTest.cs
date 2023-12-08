using CausalModel.Model;
using CausalModel.Factors;
using CausalModel.Nests;
using CausalModel.Facts;
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
            var node = FactsBuilding.CreateNode(1, val, null);
            Assert.NotEqual(node.Id, default);
            Assert.Equal(val, node.NodeValue);
            Assert.Single(node.CausesExpression.GetEdges());
            Assert.Equal(1f, node.CausesExpression.GetEdges().ElementAt(0).Probability);
        }

        [Fact]
        public void CreateAbstractFactTest()
        {
            // Arrange
            var model = new FactCollection<string>();
            var abstractNode = new ProbabilityFact<string>(new ProbabilityNest(1, null),
                "Kemsh (race)");
            var races = new string[] {"Cheaymea", "Emera", "Evoymea",
                "Myeuramea", "Oanei" };

            // Act
            var facts = FactsBuilding.CreateAbstractFact(abstractNode, races);

            // Assert
            Assert.Equal(races.Length + 1, facts.Count);

            foreach (var node in model)
            {
                if (node == abstractNode)
                    continue;
                var nodeEdges = node.GetCauses();
                var weightEdges = nodeEdges.OfType<WeightFactor>();
                var probabilityEdges = nodeEdges.OfType<ProbabilityFactor>();
                Assert.Single(weightEdges);
                Assert.Single(probabilityEdges);
            }
        }
    }
}
