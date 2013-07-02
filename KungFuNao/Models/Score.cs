using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models
{
    [DataContract]
    public class Score
    {
        [DataMember]
        public double Mean { get; set; }

        [DataMember]
        public double Deviation { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="deviation"></param>
        public Score(double mean, double deviation)
        {
            this.Mean = mean;
            this.Deviation = deviation;
        }
    }
}
