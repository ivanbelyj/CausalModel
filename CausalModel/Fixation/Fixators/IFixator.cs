using CausalModel.Facts;
using CausalModel.Model.Instance;
using CausalModel.Model.Resolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Fixation.Fixators
{
    public delegate void FactFixatedEventHandler<TFactValue>(
        object sender,
        InstanceFact<TFactValue> fixatedFact,
        bool isOccured)
        where TFactValue : class;

    /// <summary>
    /// A component of the causal model fixation process
    /// responsible for fixating the facts and related features
    /// (storing a state, override fixation data, etc.)
    /// </summary>
    /// <typeparam name="TFactValue">The type of the causal model fact value</typeparam>
    public interface IFixator<TFactValue> : IFixatedProvider
        where TFactValue : class
    {
        event FactFixatedEventHandler<TFactValue> FactFixated;

        /// <summary>
        /// Method called from generation process
        /// </summary>
        void HandleFixation(InstanceFact<TFactValue> fact, bool isOccurred);

        void Initialize(IResolvedModelProvider<TFactValue> resolvedModelProvider);
    }
}
