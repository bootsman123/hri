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
        private List<Scene> Scenes = new List<Scene>();

        public Scenario()
        {
        }

        public void Add(Scene scene)
        {
            this.Scenes.Add(scene);
        }

        public double TotalMinimumScore()
        {
            return this.Scenes.Sum(scene => scene.Score.Mean - scene.Score.Deviation);
        }

        public double TotalMaximumScore()
        {
            return this.Scenes.Sum(scene => scene.Score.Mean + scene.Score.Deviation);
        }

        public int Size()
        {
            return this.Scenes.Count;
        }

        public IEnumerator<Scene> GetEnumerator()
        {
            return this.Scenes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Scenes.GetEnumerator();
        }
    }
}
