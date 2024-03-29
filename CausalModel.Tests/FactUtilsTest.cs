﻿using CausalModel.Model;
using CausalModel.Factors;
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
            var node = FactBuilding.CreateFact(1, val, null);
            Assert.NotEqual(default, node.Id);
            Assert.Equal(val, node.FactValue);
            Assert.NotNull(node.CausesExpression);
            Assert.Single(node.CausesExpression.GetEdges());
            Assert.Equal(1f, node.CausesExpression.GetEdges().ElementAt(0).Probability);
        }

        [Fact]
        public void CreateAbstractFactTest()
        {
            // Arrange
            var abstractNode = FactBuilding.CreateFact(1, "Kemsh (race)", null);
            var races = new string[] {"Cheaymea", "Emera", "Evoymea",
                "Myeuramea", "Oanai" };

            // Act
            var variants = FactBuilding.CreateAbstractFactVariants(abstractNode, races);

            // Assert
            Assert.Equal(races.Length, variants.Count);

            foreach (var node in variants)
            {
                var nodeEdges = node.GetCauses();
                var weightEdges = nodeEdges.OfType<WeightFactor>();
                var probabilityEdges = nodeEdges.OfType<ProbabilityFactor>();
                Assert.Single(weightEdges);
                Assert.Single(probabilityEdges);
            }
        }
    }
}
