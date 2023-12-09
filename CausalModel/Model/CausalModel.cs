using CausalModel.Facts;
using CausalModel.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model;
public class CausalModel<TFactValue>
{
    /// <summary>
    /// Unresolved facts of the causal model (including blocks)
    /// </summary>
    public List<Fact<TFactValue>> Facts { get; set; }
        = new List<Fact<TFactValue>>();

    public List<DeclaredBlock> Blocks { get; set; }
        = new List<DeclaredBlock>();

    private Dictionary<string, BlockConvention>? conventionByName;

    /// <summary>
    /// Block conventions that blocks included in the model can use
    /// </summary>
    public IEnumerable<BlockConvention>? BlockConventions {
        get => conventionByName?.Values.ToList(); // Todo: remove ToList()?
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

    public BlockConvention GetConventionByName(string convName)
    {
        if (conventionByName == null)
            throw new InvalidOperationException("Cannot get convention "
                + "from model that has no conventions");
        return conventionByName[convName];
    }

    public IEnumerable<BlockFact> GetBlockFacts()
    {
        return Blocks.Select(block => {
            var conv = GetConventionByName(block.Convention);
            var res = new BlockFact(block.Convention)
            {
                Id = block.Name
            };
            if (conv.Causes != null)
                res.Causes = conv.Causes;
            if (conv.Consequences != null)
                res.Consequences = conv.Consequences;

            return res;
        });
    }
}
