﻿using Aldebaran.Proxies;
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
        private LedsProxy ledsProxy;
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

            this.TextToSpeechProxy = new TextToSpeechProxy(this.Preferences.NaoIpAddress, this.Preferences.NaoPort);
            this.BehaviorManagerProxy = new BehaviorManagerProxy(this.Preferences.NaoIpAddress, this.Preferences.NaoPort);
            this.LedsProxy = new LedsProxy(this.Preferences.NaoIpAddress, this.Preferences.NaoPort);
        }

        /// <summary>
        /// Start all proxies.
        /// </summary>
        public void Start()
        {
            System.Diagnostics.Debug.WriteLine("Proxies::Start()");

            this.KinectSensor.Start();

            // Enabled streams.
            this.KinectSensor.ColorStream.Enable(this.Preferences.ColorImageFormat);
            this.KinectSensor.SkeletonStream.Enable();

            this.KinectSpeechRecognition.Start();
        }

        /// <summary>
        /// Stop all proxies.
        /// </summary>
        public void Stop()
        {
            System.Diagnostics.Debug.WriteLine("Proxies::Stop()");

            this.TextToSpeechProxy.stopAll();
            this.BehaviorManagerProxy.stopAllBehaviors();
            //this.LedsProxy.stop(int id);

            this.KinectSpeechRecognition.Stop();
            this.KinectSensor.Stop();
        }

        #region Properties.
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

        public LedsProxy LedsProxy
        {
            get { return this.ledsProxy; }
            set
            {
                this.ledsProxy = value;
                this.OnPropertyChanged("LedsProxy");
            }
        }
        #endregion

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
