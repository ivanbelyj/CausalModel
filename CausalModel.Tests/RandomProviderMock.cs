using CausalModel.Fixation.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Tests;
internal class RandomProviderMock : IRandomProvider
{
    private readonly Random _random = new(123);
    
    public float NextFloat(float min = 0, float max = 1)
    {
        return (float)_random.NextDouble();
    }
}
