using CausalModel.Blocks.Resolving;
using CausalModel.Common;
using CausalModel.Common.DataProviders;
using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running
{
    public class SimulationsTotalResult
    {
        public int SimulationsCount { get; set; }

        public long MinTimeMilliseconds { get; set; }
        public long MaxTimeMilliseconds { get; set; }
        public double AverageTimeMilliseconds { get; set; }

        public long TotalMilliseconds { get; set; }

        public Dictionary<string, Dictionary<string, float>>
            FactActualProbabilitiesByModelName {
            get;
            set;
        } = new Dictionary<string, Dictionary<string, float>>();

        public string ToString<TFactValue>(IFactsProvider<TFactValue> factsProvider)
            where TFactValue : class
        {
            var converter = new SimulationsTotalResultToStringConverter<TFactValue>(
                this,
                factsProvider);
            return converter.Convert();
        }
    }
}
