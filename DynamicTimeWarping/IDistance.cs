using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTimeWarping
{
    public interface IDistance<T>
    {
        double Distance(T a, T b);
    }
}
