using CausalModel.Fixation;
using CausalModel.Model.Instance;
using CausalModel.Running.Models.Builders;
using CausalModel.Running.Models.FixationResult;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.FixationRunning
{
    public class FixationRunner<TFactValue>
        where TFactValue : class
    {
        private readonly FixationFacade<TFactValue> fixationFacade;
        private readonly FixationResultBuilder<TFactValue> resultBuilder;
        private CausalGenerator<TFactValue>? generator;

        public FixationRunner(FixationFacade<TFactValue> fixationFacade)
        {
            this.fixationFacade = fixationFacade;
            resultBuilder = new FixationResultBuilder<TFactValue>();

            fixationFacade.ModelInstanceFactory.ModelInstantiated += OnModelInstantiated;
            fixationFacade.Fixator.FactFixated += OnFactFixated;
        }

        public FixationResult<TFactValue> Run(int? seed = null)
        {
            generator = fixationFacade.CreateGenerator(seed);

            Stopwatch stopwatch = Stopwatch.StartNew();
            generator.FixateRootFacts();
            stopwatch.Stop();

            return resultBuilder.BuildResult(stopwatch.Elapsed, generator.Seed);
        }

        private void OnModelInstantiated(object sender, ModelInstance<TFactValue> modelInstance)
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
    }
}
