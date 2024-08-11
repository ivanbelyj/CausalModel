using CausalModel.Blocks;
using CausalModel.Common;
using CausalModel.Facts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model
{
    /// <summary>
    /// Represents not resolved causal model, including blocks
    /// </summary>
    public class CausalModel<TFactValue> : IConventionsProvider
        where TFactValue : class
    {
        public string? Name { get; set; }

        /// <summary>
        /// The facts of the causal model, except of children
        /// </summary>
        public List<Fact<TFactValue>> Facts { get; set; }
            = new List<Fact<TFactValue>>();

        // TODO: Will they include external injected blocks?
        public List<DeclaredBlock> DeclaredBlocks { get; set; } = new List<DeclaredBlock>();

        // TODO: move out conventions
        private Dictionary<string, BlockConvention>? conventionByName;
        private Dictionary<string, BlockCausesConvention>? causesConventionByName;

        private Dictionary<string, BlockFact>? blockFactsByName;

        public BlockConvention GetConventionByName(string convName)
        {
            if (conventionByName == null)
                throw new InvalidOperationException("Cannot get convention "
                    + "from model that has no conventions");
            return conventionByName[convName];
        }

        public BlockCausesConvention GetCauseConventionByName(string convName)
        {
            if (causesConventionByName == null)
                throw new InvalidOperationException("Cannot get convention "
                    + "from model that has no causes conventions");
            return causesConventionByName[convName];
        }

        /// <summary>
        /// Block conventions that blocks included in the model can use
        /// </summary>
        public IEnumerable<BlockConvention>? BlockConventions
        {
            get => conventionByName?.Values.ToList();
            // Casting to list for correct serialization
            set
            {
                if (value != null)
                {
                    conventionByName = new Dictionary<string, BlockConvention>();
                    foreach (var conv in value)
                    {
                        if (conv.Name == null)
                            throw new ArgumentException("Block convention name "
                                + "is required.");
                        conventionByName.Add(conv.Name, conv);
                    }
                }
                else
                {
                    conventionByName = null;
                }
            }
        }

        // TODO: move out this logic
        public IEnumerable<BlockCausesConvention>? BlockCausesConventions
        {
            get => causesConventionByName?.Values.ToList();
            // Casting to list for correct serialization
            set
            {
                if (value != null)
                {
                    causesConventionByName = new Dictionary<string, BlockCausesConvention>();
                    foreach (var conv in value)
                    {
                        if (conv.Name == null)
                            throw new ArgumentException("Block causes convention name "
                                + "is required.");
                        causesConventionByName.Add(conv.Name, conv);
                    }
                }
                else
                {
                    causesConventionByName = null;
                }
            }
        }

        [JsonIgnore]
        internal IEnumerable<BlockFact> BlockFacts
        {
            get
            {
                if (blockFactsByName == null)
                {
                    blockFactsByName = CreateBlockFactsByName();
                }

                return blockFactsByName!.Values;
            }
        }

        private Dictionary<string, BlockFact> CreateBlockFactsByName()
        {
            var blockFactsByName = new Dictionary<string, BlockFact>();
            foreach (var block in DeclaredBlocks)
            {
                var conv = block.Convention == null
                    ? null
                    : GetConventionByName(block.Convention);

                var blockFact = new BlockFact(block, conv);
                blockFact.Id = block.Id;

                blockFactsByName.Add(block.Id, blockFact);
            }
            return blockFactsByName;
        }
    }
}
