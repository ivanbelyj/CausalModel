using CausalModel.Blocks.Resolving;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Demo;
public class SourceData
{
    public CausalModel<string> CausalModel { get; init; }
    public BlockResolvingMap<string> BlockResolvingMap { get; init; }

    public SourceData(
        CausalModel<string> causalModel,
        BlockResolvingMap<string> blockResolvingMap)
    {
        CausalModel = causalModel;
        BlockResolvingMap = blockResolvingMap;
    }
}
