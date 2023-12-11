using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CausalModel.Model;

namespace CausalModel.Blocks.Resolving;

/// <summary>
/// Maps implementing models with conventions or block names for resolving purposes.
/// </summary>
public class BlockResolvingMap<TFactValue>
{
    public Dictionary<string, CausalModel<TFactValue>> ModelsByConventionName
    {
        get; set;
    } = new();

    public Dictionary<string, CausalModel<TFactValue>> ModelsByDeclaredBlockId
    {
        get; set;
    } = new();
}
