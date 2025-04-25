using CausalModel.Fixation;
using CausalModel.Running.FixationRunning;
using CausalModel.Running.Models.Builders;
using CausalModel.Running.Models.FixationResult;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.ProbabilityEstimation
{
    public class ProbabilityEstimator<TFactValue>
        where TFactValue : class
    {
        private readonly IFixationFacadeFactory<TFactValue> generatorFactory;
        private readonly Random random = new Random();

        public ProbabilityEstimator(IFixationFacadeFactory<TFactValue> generatorFactory)
        {
            this.generatorFactory = generatorFactory;
        }

        public ProbabilityEstimationResult<TFactValue> RunSimulations(int count = 1000)
        {
            var stopwatch = Stopwatch.StartNew();
            var totalResultBuilder = new ProbabilityEstimationResultBuilder<TFactValue>();

            for (int i = 0; i < count; i++)
            {
                var simulationResult = RunSimulation();
                totalResultBuilder.AddNewFixationResult(simulationResult);
            }

            stopwatch.Stop();

            var res = totalResultBuilder.Build();
            res.TotalMilliseconds = stopwatch.ElapsedMilliseconds;
            return res;
        }

        public FixationResult<TFactValue> RunSimulation()
        {
            var fixationFacade = generatorFactory.Create();
            var simulation = new FixationRunner<TFactValue>(fixationFacade);
            return simulation.Run(random.Next());
        }
    }
}
