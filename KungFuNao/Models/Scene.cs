using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Kinect.Toolbox;
using Kinect.Toolbox.Record;
using System.Runtime.Serialization;
using KungFuNao.Models;
using System.IO;
using Aldebaran.Proxies;
using KungFuNao.Tools;
using KungFuNao.Models.Nao;
using DynamicTimeWarping;

namespace KungFuNao
{
    [KnownType(typeof(LeftHandPunchScene))]
    [KnownType(typeof(GedanBaraiScene))]
    [KnownType(typeof(RightHandPunchScene))]
    [DataContract]
    public abstract class Scene
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public Score Score { get; set; }

        public double Performance { get; set; }

        public int NumberOfTimesExplained { get; set; }

        public List<Skeleton> Skeletons { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileName"></param>
        /// <param name="score"></param>
        public Scene( string name, string fileName, Score score )
        {
            this.Name = name;
            this.FileName = fileName;
            this.Score = score;

            this.Initialize();
        }

        private void Initialize()
        {
            this.Performance = 0;
            this.NumberOfTimesExplained = 0;

            // Load skeletons.
            this.Skeletons = new List<Skeleton>();

            try
            {
                using (Stream stream = new FileStream(this.FileName, FileMode.Open))
                {
                    this.Skeletons = SkeletonRecordingConverter.FromStream(stream);
                }
            }
            catch (IOException e)
            {
                
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            this.Initialize();
        }

        public void DeterminePerformance(List<Skeleton> skeletons)
        {
            this.Performance = DTW<Skeleton>.Distance(this.Skeletons, skeletons, new SkeletonDistance());
        }

        public abstract void performDefault(Proxies Proxies);
        public abstract void explainToUser(Proxies Proxies);
        public abstract void giveFeedbackToUser(Proxies Proxies);
    }
}
