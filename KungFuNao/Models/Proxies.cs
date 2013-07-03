using Aldebaran.Proxies;
using KungFuNao.Tools;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models
{
    public class Proxies : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields.
        private KinectSensor kinectSensor;
        private KinectSpeechRecognition kinectSpeechRecognition;

        private TextToSpeechProxy textToSpeechProxy;
        private BehaviorManagerProxy behaviorManagerProxy;
        #endregion

        private Preferences Preferences;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Preferences"></param>
        public Proxies(Preferences Preferences)
        {
            this.Preferences = Preferences;

            this.KinectSensor = KinectSensor.KinectSensors.FirstOrDefault(e => e.Status == KinectStatus.Connected);
            this.KinectSpeechRecognition = new KinectSpeechRecognition(this.KinectSensor);

            //this.TextToSpeechProxy = new TextToSpeechProxy(this.Preferences.NaoIpAddress, this.Preferences.NaoPort);
            //this.BehaviorManagerProxy = new BehaviorManagerProxy(this.Preferences.NaoIpAddress, this.Preferences.NaoPort);
        }

        /// <summary>
        /// Start all proxies.
        /// </summary>
        public void Start()
        {
            this.KinectSensor.Start();
            this.KinectSpeechRecognition.Start();

            // Enabled streams.
            this.KinectSensor.ColorStream.Enable(this.Preferences.ColorImageFormat);
            this.KinectSensor.SkeletonStream.Enable();
        }

        /// <summary>
        /// Stop all proxies.
        /// </summary>
        public void Stop()
        {
            //this.TextToSpeechProxy.stopAll();
            //this.BehaviorManagerProxy.stopAllBehaviors();

            this.KinectSpeechRecognition.Stop();
            this.KinectSensor.Stop();
        }

        public KinectSensor KinectSensor
        {
            get { return this.kinectSensor; }
            set
            {
                this.kinectSensor = value;
                this.OnPropertyChanged("KinectSensor");
            }
        }

        public KinectSpeechRecognition KinectSpeechRecognition
        {
            get { return this.kinectSpeechRecognition; }
            set
            {
                this.kinectSpeechRecognition = value;
                this.OnPropertyChanged("KinectSpeechRecognition");
            }
        }

        public TextToSpeechProxy TextToSpeechProxy
        {
            get { return this.textToSpeechProxy; }
            set
            {
                this.textToSpeechProxy = value;
                this.OnPropertyChanged("TextToSpeechProxy");
            }
        }

        public BehaviorManagerProxy BehaviorManagerProxy
        {
            get { return this.behaviorManagerProxy; }
            set
            {
                this.behaviorManagerProxy = value;
                this.OnPropertyChanged("BehaviorManagerProxy");
            }
        }

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
