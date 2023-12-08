using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model
{
    public interface IFactProvider<TNodeValue>
    {
        public Fact<TNodeValue> GetFactById(string id);
    }
}
