using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Blocks;

/// <summary>
/// Any CM has a clear set of dependencies, blocks that it uses -
/// <b>declared blocks.</b>
/// Each of the declared blocks represents a semantically unique entity.
/// The used block has a unique (within the CM) name, also it refers
/// to some convention. Several declared blocks can use the same conventions
/// (for example, in a story model, several characters can be used,
/// whose personality is represented by generalized model).
/// </summary>
public class DeclaredBlock
{
    /// <summary>
    /// A name unique in the causal model in which the declared block is used
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The name of convention referenced by the declared block
    /// </summary>
    public string ConventionName { get; set; }
}
