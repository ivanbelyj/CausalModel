using CausalModel.Blocks;
using CausalModel.Facts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;

/// <summary>
/// Representing not resolved causal model including blocks
/// </summary>
public class CausalModel<TFactValue>
{
    public string? Name { get; set; }

    /// <summary>
    /// The facts of the causal model except for children
    /// </summary>
    public List<Fact<TFactValue>> Facts { get; set; }
        = new List<Fact<TFactValue>>();

    public List<DeclaredBlock> Blocks { get; set; }
        = new List<DeclaredBlock>();

    private Dictionary<string, BlockConvention>? conventionByName;
    private Dictionary<string, BlockFact>? blockFactByName;

    public BlockConvention GetConventionByName(string convName)
    {
        if (conventionByName == null)
            throw new InvalidOperationException("Cannot get convention "
                + "from model that has no conventions");
        return conventionByName[convName];
    }

    /// <summary>
    /// Block conventions that blocks included in the model can use
    /// </summary>
    public IEnumerable<BlockConvention>? BlockConventions {
        get => conventionByName?.Values.ToList();
        // Casting to list for correct serialization
        set {
            if (value != null)
            {
                conventionByName = new();
                foreach (var conv in value)
                {
                    if (conv.Name == null)
                        throw new ArgumentException("Block convention name "
                            + "is required.");
                    conventionByName.Add(conv.Name, conv);
                }
            } else
            {
                conventionByName = null;
            }
        }
    }

    [JsonIgnore]
    internal IEnumerable<BlockFact> BlockFacts
    {
        get
        {
            if (blockFactByName == null)
            {
                blockFactByName = new();
                foreach (var block in Blocks)
                {
                    var conv = block.Convention == null ? null
                        : GetConventionByName(block.Convention);

                    var blockFact = new BlockFact(block, conv);
                    blockFact.Id = block.Id;

                    blockFactByName.Add(block.Id, blockFact);
                }
            }

            return blockFactByName.Values;
        }
    }
}
