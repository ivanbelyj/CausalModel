using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CausalModel.Running.Extensions
{
    internal static class TimeSpanExtensions
    {
        public const long NanosecondsPerTick = 100;

        public static double GetTotalNanoseconds(this TimeSpan timeSpan)
        {
            return (double)timeSpan.Ticks * NanosecondsPerTick;
        }
    }
}
