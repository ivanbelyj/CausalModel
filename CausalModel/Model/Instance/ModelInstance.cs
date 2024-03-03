using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Instance
{
    /// <summary>
    /// Causal model realized and fixating in the app.
    /// Declared blocks are also resolved into causal instances -
    /// block convention implementations
    /// </summary>
    public class ModelInstance<TFactValue>
        where TFactValue : class
    {
        public CausalModel<TFactValue> Model { get; private set; }
        public string InstanceId { get; private set; }

        public ModelInstance(CausalModel<TFactValue> causalModel,
            string? instanceId = null)
        {
            Model = causalModel;
            InstanceId = instanceId ?? Guid.NewGuid().ToString();
        }
    }
}
