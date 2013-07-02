using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTimeWarping;

namespace DynamicTimeWarpingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Double> seriesA = (new double[] { 4, 5, 6, 7, 8 }).ToList();
            List<Double> seriesB = (new double[] { 4, 5, 6, 7, 8 }).ToList();

            var distance = new EuclidianDistance();
            var cost = DTW<Double>.getDistance(seriesA, seriesB, distance, 4);

            Console.WriteLine("Cost: " + cost);
            Console.ReadLine();
        }
    }
}
