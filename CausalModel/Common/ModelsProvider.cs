using CausalModel.Model;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common
{
    public class ModelsProvider<T> : IModelProvider<T>
        where T : class
    {
        public IEnumerable<InstanceFact<T>> GetInstanceFacts()
        {
            throw new NotImplementedException();
        }

        public InstanceFact<T> GetModelFact(string factId)
        {
            throw new NotImplementedException();
        }
    }
}
