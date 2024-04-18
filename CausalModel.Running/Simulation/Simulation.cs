using CausalModel.Fixation;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.Simulation
{
    public class Simulation<TFactValue>
        where TFactValue : class
    {
        private readonly FixationFacade<TFactValue> fixation;
        private CausalGenerator<TFactValue>? generator;
        private readonly SimulationResultBuilder<TFactValue> resultBuilder;

        public Simulation(FixationFacade<TFactValue> fixation)
        {
            this.fixation = fixation;
            resultBuilder = new SimulationResultBuilder<TFactValue>();

            fixation.ModelInstanceFactory.ModelInstantiated += OnModelInstantiated;
            fixation.Fixator.FactFixated += OnFactFixated;

            // Todo: add data about not fixated facts ?
        }

        private void OnModelInstantiated(object sender,
            ModelInstance<TFactValue> modelInstance)
        {
            resultBuilder.AddModelInstantiated(modelInstance);
        }

        private void OnFactFixated(
            object sender,
            InstanceFact<TFactValue> fixatedFact,
            bool isOccurred)
        {
            resultBuilder.AddFact(fixatedFact, isOccurred);
        }

        public SimulationResult Run(int? seed = null)
        {
            generator = fixation.CreateGenerator(seed);

            Stopwatch stopwatch = Stopwatch.StartNew();
            generator.FixateRootFacts();
            stopwatch.Stop();

            var res = resultBuilder
                .Build(stopwatch.ElapsedMilliseconds);

            return res;
        }
    }
}
