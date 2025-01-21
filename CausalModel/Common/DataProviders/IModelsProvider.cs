using CausalModel.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common.DataProviders
{
    public interface IModelsProvider<T> where T : class
    {
        CausalModel<T> GetModelByName(string name);
    }
}
