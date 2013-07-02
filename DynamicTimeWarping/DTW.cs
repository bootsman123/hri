using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTimeWarping
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Dynamic_time_warping
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DTW<T>
    {
        /// <summary>
        /// Get distance between two time series.
        /// </summary>
        /// <param name="seriesA"></param>
        /// <param name="seriesB"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static double Distance(List<T> seriesA, List<T> seriesB, IDistance<T> distance)
        {
            // Initialize.
            var dtw = new double[seriesA.Count + 1, seriesB.Count + 1];

            for (var i = 1; i < seriesA.Count; i++)
            {
                dtw[i, 0] = Double.PositiveInfinity;
            }

            for (var i = 1; i < seriesB.Count; i++)
            {
                dtw[0, i] = Double.PositiveInfinity;
            }

            dtw[0, 0] = 0;

            // Calculate.
            for (var i = 1; i <= seriesA.Count; i++)
            {
                for (var j = 1; j <= seriesB.Count; j++)
                {
                    var cost = distance.Distance(seriesA[i - 1], seriesB[j - 1]);
                    dtw[i, j] = cost + Math.Min(dtw[i - 1, j], Math.Min(dtw[i, j - 1], dtw[i - 1, j - 1]));
                }
            }

            return dtw[seriesA.Count - 1, seriesB.Count - 1];
        }

        /// <summary>
        /// Get distance between two time series given a certain width.
        /// </summary>
        /// <param name="seriesA"></param>
        /// <param name="seriesB"></param>
        /// <param name="distance"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static double Distance(List<T> seriesA, List<T> seriesB, IDistance<T> distance, int width)
        {
            // Initialize.
            var dtw = new double[seriesA.Count, seriesB.Count];

            for (var i = 0; i < seriesA.Count; i++)
            {
                for (var j = 0; j < seriesB.Count; j++)
                {
                    dtw[i, j] = Double.PositiveInfinity;
                }
            }

            dtw[0, 0] = 0;

            width = Math.Max(width, Math.Abs(seriesA.Count - seriesB.Count));

            // Calculate.
            for (var i = 1; i < seriesA.Count; i++)
            {
                for (var j = (int)Math.Max(1, i - width); j < Math.Min(seriesB.Count, i + width); j++)
                {
                    var cost = distance.Distance(seriesA[i], seriesB[j]);
                    dtw[i, j] = cost + Math.Min(dtw[i - 1, j], Math.Min(dtw[i, j - 1], dtw[i - 1, j - 1]));
                }
            }

            return dtw[seriesA.Count - 1, seriesB.Count - 1];
        }
    }
}
