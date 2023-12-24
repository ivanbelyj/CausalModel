using CausalModel.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Instance;

public delegate void ModelInstanceCreatedEventHandler<TFactValue>(
    object sender,
    ModelInstance<TFactValue> modelInstance);

public class ModelInstanceFactory<TFactValue>
{
    private ModelInstanceCreatedEventHandler<TFactValue>? modelInstantiated;
    public event ModelInstanceCreatedEventHandler<TFactValue>? ModelInstantiated
    {
        add
        {
            //Console.WriteLine("Added ModelInstantiated event handler (name: "
            //    + value?.Method.Name + ")");
            
            modelInstantiated += value;
        }
        remove
        {
            //Console.WriteLine("Removed ModelInstantiated event handler (name: "
            //    + value?.Method.Name + ")");
            modelInstantiated -= value;
        }
    }

    public ModelInstance<TFactValue> InstantiateModel(
        CausalModel<TFactValue> model)
    {
        var res = new ModelInstance<TFactValue>(model);
        modelInstantiated?.Invoke(this, res);
        return res;
    }
}
