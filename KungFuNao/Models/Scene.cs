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
using System.ComponentModel;

namespace KungFuNao
{
    [KnownType(typeof(LeftHandPunchScene))]
    [KnownType(typeof(GedanBaraiScene))]
    [KnownType(typeof(RightHandPunchScene))]
    [DataContract]
    public abstract class Scene : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public Score Score { get; set; }

        private double performance;

        public double Performance
        {
            get { return this.performance; }
            set
            {
                this.performance = value;
                this.OnPropertyChanged("Performance");
            }
        }

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
            try
            {
                this.Performance = DTW<Skeleton>.Distance(this.Skeletons, skeletons, new SkeletonDistance());
            }
            catch (Exception)
            {
                this.Performance = 100;
            }
        }

        public abstract void PerformDefault(Proxies Proxies);
        public abstract void Explain(Proxies Proxies);
        public abstract void GiveFeedback(Proxies Proxies);


        /// <summary>
        /// On property changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
