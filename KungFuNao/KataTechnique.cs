using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Kinect.Toolbox;
using Kinect.Toolbox.Record;
using System.Xml.Serialization;

namespace KungFuNao
{
    class KataTechnique
    {
        private string name { get; set; }
        private string fileName { get; set; }
        //private List<SkeletonFrame> skeletonFrames { get; set; }

        public KataTechnique( string name )
        {
            this.name = name;
        }
    }
}
