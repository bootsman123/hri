using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using KungFuNao.Models;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using KungFuNao.Tools;
using Aldebaran.Proxies;
using System.Windows.Input;
using Microsoft.TeamFoundation.MVVM;
using Kinect.Toolbox;
using Kinect.Toolbox.Record;
using KungFuNao.Models.Nao;
using System.Drawing;
using System.Windows.Media;

namespace KungFuNao.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum ControlMode { Normal, Replay, Record };

        #region Fields.
        public Preferences Preferences { get; private set; }
        public Proxies Proxies { get; private set; }
        public Scenario Scenario { get; private set; }

        public ImageSource image { get; private set; }

        public ControlMode Mode;

        private ColorStreamManager ColorStreamManager;

        private KinectRecorder KinectRecorder;
        private Stream RecordStream;
        private KinectReplay KinectReplay;
        private Stream ReplayStream;

        private NaoTeacher NaoTeacher;
        #endregion

        #region Commands.
        public ICommand PlayCommand { get { return new RelayCommand(Play); } }
        public ICommand StopCommand { get { return new RelayCommand(Stop); } }
        public ICommand RecordCommand { get { return new RelayCommand(Record); } }
        public ICommand RunCommand { get { return new RelayCommand(Run); } }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            this.Preferences = new Preferences();
            this.Proxies = new Proxies(this.Preferences);

            this.LoadData();

            // Create Nao teacher.
            this.NaoTeacher = new NaoTeacher(this.Preferences, this.Proxies, this.Scenario);

            // Color stream.
            this.ColorStreamManager = new ColorStreamManager();
            this.ColorStreamManager.PropertyChanged += this.ColorStreamManagerPropertyChanged;

            this.Proxies.KinectSensor.ColorFrameReady += this.KinectSensorColorFrameReady;

            // Skeleton stream.
            this.Proxies.KinectSensor.SkeletonFrameReady += this.KinectSensorSkeletonFrameReady;
            this.Proxies.KinectSensor.SkeletonFrameReady += this.NaoTeacher.KinectSensorSkeletonFrameReady;


            /*
            this.Scenario.Add(new Scene("Left Hand Punch", "C:\\Users\\bootsman\\Desktop\\data.v1.kinect", new Score(30, 5)));
            this.Scenario.Add(new Scene("Left Hand Punch", "C:\\Users\\bootsman\\Desktop\\data.v2.kinect", new Score(40, 15)));
            this.Scenario.Add(new Scene("Right Hand Punch", "C:\\Users\\bootsman\\Desktop\\data.v3.kinect", new Score(30, 5)));

            IEnumerator<Scene> enumerator = this.Scenario.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Scene scene = enumerator.Current;
                System.Diagnostics.Debug.WriteLine("Name: " + scene.Name);
                System.Diagnostics.Debug.WriteLine("Skeletons: " + scene.Skeletons.Count);
            }
             * */
        }
        
        /// <summary>
        /// Start playing selected scene.
        /// </summary>
        private void Play()
        {
            if (this.KinectReplay == null)
            {
                if (this.ReplayStream == null)
                {
                    this.ReplayStream = new FileStream(this.Preferences.KinectDataFile, FileMode.Open);
                }

                this.KinectReplay = new KinectReplay(this.ReplayStream);
                this.KinectReplay.ColorImageFrameReady += new EventHandler<ReplayColorImageFrameReadyEventArgs>(this.ReplayColorImageFrameReady);
                this.KinectReplay.SkeletonFrameReady += new EventHandler<ReplaySkeletonFrameReadyEventArgs>(this.ReplaySkeletonFrameReady);

                this.KinectReplay.Start();
                this.Mode = ControlMode.Replay;
            }
        }

        /// <summary>
        /// Stop playing selected scene.
        /// </summary>
        private void Stop()
        {
            if (this.Mode == ControlMode.Replay)
            {
                this.KinectReplay.Stop();
                this.Mode = ControlMode.Normal;
            }
        }

        /// <summary>
        /// Record a scene.
        /// </summary>
        private void Record()
        {
            // Stop recording.
            if (this.Mode == ControlMode.Record)
            {
                System.Diagnostics.Debug.WriteLine("Stop recording...");

                this.Mode = ControlMode.Normal;
                this.KinectRecorder.Stop();
            }
            // Start recording.
            else
            {
                System.Diagnostics.Debug.WriteLine("Start recording...");

                this.RecordStream = new BufferedStream(new FileStream(this.Preferences.KinectDataFile, FileMode.Create));
                this.KinectRecorder = new KinectRecorder(KinectRecordOptions.Skeletons, this.RecordStream);
                this.Mode = ControlMode.Record;

                //new KinectRecorder(KinectRecordOptions.Color | KinectRecordOptions.Skeletons, this.RecordStream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Run()
        {
            this.NaoTeacher.Start();
        }

        /// <summary>
        /// Save data.
        /// </summary>
        private void SaveData()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Scenario));

            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t"
            };

            using (var writer = XmlWriter.Create(this.Preferences.ScenariosFile, settings))
            {
                serializer.WriteObject(writer, this.Scenario);
            }
        }

        /// <summary>
        /// Load data.
        /// </summary>
        private void LoadData()
        {
            try
            {
                using (FileStream stream = new FileStream(this.Preferences.ScenariosFile, FileMode.Open))
                {
                    DataContractSerializer deserializer = new DataContractSerializer(typeof(Scenario));
                    this.Scenario = (Scenario)deserializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                this.Scenario = new Scenario();
            }
        }

        /// <summary>
        /// On color stream manager property changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorStreamManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Image = this.ColorStreamManager.Bitmap;
        }

        /// <summary>
        /// On color frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectSensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame == null)
                {
                    return;
                }

                // Record?
                if (this.Mode == ControlMode.Record)
                {
                    //this.kinectRecorder.Record(colorImageFrame);
                }

                // Display real time?
                if (this.Mode != ControlMode.Replay)
                {
                    this.ColorStreamManager.Update(new ReplayColorImageFrame(colorImageFrame));
                }
            }
        }

        /// <summary>
        /// On skeleton frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectSensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }

                // Record?
                if (this.Mode == ControlMode.Record)
                {
                    this.KinectRecorder.Record(skeletonFrame);
                }
            }
        }

        /// <summary>
        /// On replay color image frame event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplayColorImageFrameReady(object sender, ReplayColorImageFrameReadyEventArgs e)
        {
            ReplayColorImageFrame replayColorImageFrame = e.ColorImageFrame;

            if (replayColorImageFrame == null)
            {
                return;
            }

            this.ColorStreamManager.Update(replayColorImageFrame);
        }

        /// <summary>
        /// On replay skeleton event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaySkeletonFrameReady(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame replaySkeletonFrame = e.SkeletonFrame;

            if (replaySkeletonFrame == null)
            {
                return;
            }

            //this.SkeletonDisplayManager.Draw(replaySkeletonFrame.Skeletons, false);
        }

        /// <summary>
        /// On window closing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosed(object sender, EventArgs e)
        {
            // Save data.
            this.SaveData();

            this.Proxies.Stop();
        }

        /// <summary>
        /// On window loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowLoaded(object sender, EventArgs e)
        {
            this.Proxies.Start();
        }

        #region Fields.
        public ImageSource Image
        {
            get { return this.image; }
            set
            {
                this.image = value;
                this.OnPropertyChanged("Image");
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
