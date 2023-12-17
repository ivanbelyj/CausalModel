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
    public event ModelInstanceCreatedEventHandler<TFactValue>? ModelInstanceCreated;

    public ModelInstance<TFactValue> InstantiateModel(
        CausalModel<TFactValue> model)
    {
        var res = new ModelInstance<TFactValue>(model);
        ModelInstanceCreated?.Invoke(this, res);
        return res;
    }
}
