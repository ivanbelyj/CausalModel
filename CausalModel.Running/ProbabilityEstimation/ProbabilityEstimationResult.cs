using CausalModel.Blocks.Resolving;
using CausalModel.Common;
using CausalModel.Common.DataProviders;
using CausalModel.Model;
using CausalModel.Running.Models.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Running.ProbabilityEstimation
{
    public class ProbabilityEstimationResult<TFactValue>
        where TFactValue : class
    {
        public int SimulationsCount { get; set; }

        public TimeSpan MinTime { get; set; }
        public TimeSpan MaxTime { get; set; }
        public TimeSpan AverageTime { get; set; }

        public long MinTimeMilliseconds { get => MinTime.Milliseconds; }
        public long MaxTimeMilliseconds { get => MaxTime.Milliseconds; }
        public double AverageTimeMilliseconds { get => AverageTime.Milliseconds; }

        public long TotalMilliseconds { get; set; }

        public Dictionary<string, Dictionary<string, ProbabilityEstimationResultFactInfo<TFactValue>>> FactsInfoByModelName
            { get; set; } = new Dictionary<string, Dictionary<string, ProbabilityEstimationResultFactInfo<TFactValue>>>();

        public override string ToString()
        {
            var converter = new ProbabilityEstimationResultToStringConverter<TFactValue>(this);
            return converter.Convert();
        }
    }
}
