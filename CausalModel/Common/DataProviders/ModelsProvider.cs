using CausalModel.Model;
using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CausalModel.Common.DataProviders
{
    public class ModelsProvider<TFactValue> : IModelsProvider<TFactValue>
        where TFactValue : class
    {
        private readonly Dictionary<string, CausalModel<TFactValue>> modelsByName;

        public ModelsProvider(IEnumerable<CausalModel<TFactValue>> models)
        {
            modelsByName = models.ToDictionary(x => x.Name, x => x);
        }

        public CausalModel<TFactValue> GetModelByName(string name)
        {
            return modelsByName[name];
        }
    }
}
