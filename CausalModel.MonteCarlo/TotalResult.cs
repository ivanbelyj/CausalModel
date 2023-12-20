using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.MonteCarlo;
public class TotalResult
{
    public long MinTimeMilliseconds { get; set; }
    public long MaxTimeMilliseconds { get; set; }
    public double AverageTimeMilliseconds { get; set; }

    public long TotalMilliseconds { get; set; }

    public override string ToString()
    {
        return $"MinTimeMilliseconds: {MinTimeMilliseconds.ToString("N0")} ms\n" +
               $"MaxTimeMilliseconds: {MaxTimeMilliseconds.ToString("N0")} ms\n" +
               $"AverageTimeMilliseconds: {AverageTimeMilliseconds.ToString("F2")} ms\n" +
               $"TotalMilliseconds: {TotalMilliseconds.ToString("N0")} ms";
    }

}
