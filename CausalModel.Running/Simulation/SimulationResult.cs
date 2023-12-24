using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Simulation;
public class SimulationResult
{
    public long ElapsedMilliseconds { get; set; }
    public Dictionary<string, int> SimulationsCountByModelName { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> ModelFactCounts { get; set; }
        = new();
}
