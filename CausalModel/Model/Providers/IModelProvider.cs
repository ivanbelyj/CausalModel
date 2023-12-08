﻿using CausalModel.Facts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Model.Providers;
public interface IModelProvider<TFactValue> : IFactProvider<TFactValue>
{
    IEnumerable<Fact<TFactValue>> GetAbstractFactVariants(Fact<TFactValue> abstractFact);
    IEnumerable<Fact<TFactValue>> GetConsequences(Fact<TFactValue> fact);
    IEnumerable<Fact<TFactValue>> GetRootCauses();
}