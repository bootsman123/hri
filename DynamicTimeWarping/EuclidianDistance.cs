using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTimeWarping
{
    public class EuclidianDistance : IDistance<Double>
    {
        public double Distance(Double a, Double b)
        {
            return Math.Sqrt(Math.Pow(a - b, 2));
        }
    }
}
