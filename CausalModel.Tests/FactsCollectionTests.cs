using CausalModel.Factors;
using CausalModel.Nests;
using CausalModel.Nodes;
using System.Linq;

namespace CausalModel.Tests
{
    public class FactsCollectionTests
    {
        [Fact]
        public void CreateTest()
        {
            // Arrange
            var facts = new List<Fact<string>>();
            for (int i = 0; i < 5; i++)
            {
                facts.Add(FactUtils.CreateNode(1, "Root node " + i, null));
            }
            string lastVal = "last fact";
            var lastFact = FactUtils.CreateNode(1, lastVal, null);
            facts.Add(lastFact);

            // Act
            var collection = new FactCollection<string>(facts);

            // Assert
            Assert.Equal(facts.Count, collection.Nodes.Count());
            Assert.Equal(lastFact.NodeValue,
                collection.GetFactById(lastFact.Id).NodeValue);
        }
    }
}
