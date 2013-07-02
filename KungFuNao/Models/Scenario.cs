using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Kinect.Toolbox;
using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace KungFuNao
{
    [DataContract]
    public class Scenario : IEnumerable<Scene>
    {
        [DataMember]
        private List<Scene> scenes = new List<Scene>();

        public Scenario()
        {
        }

        public void Add(Scene scene)
        {
            this.scenes.Add(scene);
        }

        public int Size()
        {
            return this.scenes.Count;
        }

        public IEnumerator<Scene> GetEnumerator()
        {
            return this.scenes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.scenes.GetEnumerator();
        }
    }
}
