using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.MonteCarlo.Simulation;
public class SimulationResultBuilder<TFactValue>
{
    public void AddFact(InstanceFact<TFactValue> instanceFact)
    {

    }

    public SimulationResult Build()
    {
        return new SimulationResult();
    }
}
