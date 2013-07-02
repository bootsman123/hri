using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Serialization;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using Kinect.Toolbox;
using Kinect.Toolbox.Record;
using System.IO;
using DynamicTimeWarping;
using KungFuNao.Models;
using KungFuNao.ViewModels;
using KungFuNao.Tools;
using Aldebaran.Proxies;
using System.Runtime.Serialization;
using System.Xml;
using KungFuNao.Models.Nao;

namespace KungFuNao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum Mode { Normal, Replay, Record };

        private KinectSensor kinectSensor;
        private Mode mode;

        private Preferences preferences;
        private Scenario scenario;

        private ColorStreamManager colorStreamManager;
        private SkeletonDisplayManager skeletonDisplayManager;
        private Skeleton[] skeletons;

        private KinectRecorder kinectRecorder;
        private Stream recordStream;
        private KinectReplay kinectReplay;
        private Stream replayStream;

        private KinectSpeechRecognition speechRecognition;
        private TextToSpeechProxy textToSpeechProxy;
        private BehaviorManagerProxy behaviorManagerProxy;

        private NaoTeacher naoTeacher;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.Closed += this.OnWindowClosed;
            this.DataContext = this;

            this.Preferences = new Preferences();

            this.LoadData();

            this.Scenario.Add(new LeftHandPunchScene("C:\\Users\\bootsman\\Desktop\\data.v1.kinect", new Score(30, 5)));
            this.Scenario.Add(new GedanBaraiScene("C:\\Users\\bootsman\\Desktop\\data.v2.kinect", new Score(40, 15)));
            this.Scenario.Add(new RightHandPunchScene("C:\\Users\\bootsman\\Desktop\\data.v3.kinect", new Score(30, 5)));

            // Find Kinect sensor.
            this.kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(e => e.Status == KinectStatus.Connected);

            if (this.kinectSensor == null)
            {
                return;
            }

            this.mode = Mode.Normal;
            this.recordStream = null;
            this.replayStream = null;

            // Add button listeners.
            this.buttonRecord.Click += new RoutedEventHandler(this.onClickButtonRecord);
            this.buttonPlay.Click += new RoutedEventHandler(this.onClickButtonPlay);
            this.buttonStop.Click += new RoutedEventHandler(this.onClickButtonStop);

            // Color stream.
            this.colorStreamManager = new ColorStreamManager();
            this.colorStreamManager.PropertyChanged += this.colorStreamManagerPropertyChanged;

            this.kinectSensor.ColorStream.Enable(Preferences.COLOR_IMAGE_FORMAT);
            this.kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(this.kinectSensorColorFrameReady);

            // Skeleton stream.
            this.skeletonDisplayManager = new SkeletonDisplayManager(this.kinectSensor, this.imageCanvas);
            this.skeletons = null;

            this.kinectSensor.SkeletonStream.Enable();
            this.kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.kinectSensorSkeletonFrameReady);

            this.kinectSensor.Start();

            // Initialize proxies.
            this.textToSpeechProxy = new TextToSpeechProxy(Preferences.NaoIpAddress, this.Preferences.NaoPort);
            this.behaviorManagerProxy = new BehaviorManagerProxy(this.Preferences.NaoIpAddress, this.Preferences.NaoPort);
            this.speechRecognition = new KinectSpeechRecognition(this.kinectSensor);

            this.naoTeacher = new NaoTeacher(this.textToSpeechProxy, this.behaviorManagerProxy, this.speechRecognition, this.scenario);
            this.kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.naoTeacher.kinectSensorSkeletonFrameReady);
        }

        /// <summary>
        /// On color stream manager property changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void colorStreamManagerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.imageKinect.Source = this.colorStreamManager.Bitmap;
        }

        /// <summary>
        /// On click button record.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClickButtonRecord(object sender, RoutedEventArgs e)
        {
            // Stop recording.
            if (this.mode == Mode.Record)
            {
                System.Diagnostics.Debug.WriteLine("Stop recording...");

                this.mode = Mode.Normal;
                this.kinectRecorder.Stop();
            }
            // Start recording.
            else
            {
                System.Diagnostics.Debug.WriteLine("Start recording...");

                this.recordStream = new BufferedStream(new FileStream(Preferences.KINECT_DATA_FILE, FileMode.Create));
                this.kinectRecorder = new KinectRecorder(KinectRecordOptions.Skeletons, this.recordStream);
                this.mode = Mode.Record;

                //KinectRecordOptions.Color | 
            }
        }

        /// <summary>
        /// On click button play.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClickButtonPlay(object sender, RoutedEventArgs e)
        {
            if (this.kinectReplay == null)
            {
                if (this.replayStream == null)
                {
                    this.replayStream = new FileStream(Preferences.KINECT_DATA_FILE, FileMode.Open);
                }

                this.kinectReplay = new KinectReplay(this.replayStream);
                this.kinectReplay.ColorImageFrameReady += new EventHandler<ReplayColorImageFrameReadyEventArgs>(this.replayColorImageFrameReady);
                this.kinectReplay.SkeletonFrameReady += new EventHandler<ReplaySkeletonFrameReadyEventArgs>(this.replaySkeletonFrameReady); 

                this.kinectReplay.Start();
                this.mode = Mode.Replay;
            }
        }

        /// <summary>
        /// On click button stop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClickButtonStop(object sender, RoutedEventArgs e)
        {
            if (this.mode == Mode.Replay)
            {
                this.kinectReplay.Stop();
                this.mode = Mode.Normal;
            }
        }

        private void replayColorImageFrameReady(object sender, ReplayColorImageFrameReadyEventArgs e)
        {
            ReplayColorImageFrame replayColorImageFrame = e.ColorImageFrame;

            if (replayColorImageFrame == null)
            {
                return;
            }

            colorStreamManager.Update(replayColorImageFrame);
        }

        private void replaySkeletonFrameReady(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame replaySkeletonFrame = e.SkeletonFrame;

            if (replaySkeletonFrame == null)
            {
                return;
            }

            this.skeletonDisplayManager.Draw(replaySkeletonFrame.Skeletons, false);
        }

        /// <summary>
        /// On color frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kinectSensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame == null)
                {
                    return;
                }

                // Record?
                if (this.mode == Mode.Record)
                {
                    //this.kinectRecorder.Record(colorImageFrame);
                }

                // Display real time?
                if (this.mode != Mode.Replay)
                {
                    this.colorStreamManager.Update(new ReplayColorImageFrame(colorImageFrame));
                }
            }
        }

        /// <summary>
        /// On skeleton frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kinectSensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }

                // Record?
                if (this.mode == Mode.Record)
                {
                    this.kinectRecorder.Record(skeletonFrame);
                }

                // Display real time?
                if (this.mode != Mode.Replay)
                {
                    Kinect.Toolbox.Tools.GetSkeletons(skeletonFrame, ref this.skeletons);
                    this.skeletonDisplayManager.Draw(this.skeletons, false);
                }
            }
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

            using (var writer = XmlWriter.Create(Preferences.SCENARIOS_FILE, settings))
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
                using (FileStream stream = new FileStream(Preferences.SCENARIOS_FILE, FileMode.Open))
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
        /// On window closing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosed(object sender, EventArgs e)
        {
            // Save data.
            this.SaveData();
        }

        #region Getters and setters.
        public Preferences Preferences
        {
            get { return this.preferences; }
            set { this.preferences = value; }
        }

        public Scenario Scenario
        {
            get { return this.scenario; }
            set { this.scenario = value; }
        }
        #endregion
    }
}

/*
  String fileNameA = "data.v1.kinect";
  String fileNameB = "data.v3.kinect";

  Stream streamA = new FileStream(Path.Combine(KINECT_DATA_FILE_PATH, fileNameA), FileMode.Open);
  Stream streamB = new FileStream(Path.Combine(KINECT_DATA_FILE_PATH, fileNameB), FileMode.Open);

  List<Skeleton> skeletonsA = SkeletonRecordingConverter.FromStream(streamA);
  List<Skeleton> skeletonsB = SkeletonRecordingConverter.FromStream(streamB);

  double distance = DTW<Skeleton>.Distance(skeletonsA, skeletonsB, new SkeletonDistance(), (int)(Math.Min(skeletonsA.Count, skeletonsB.Count) * 0.4));

  System.Diagnostics.Debug.WriteLine("[A] Skeletons size: " + skeletonsA.Count);
  System.Diagnostics.Debug.WriteLine("[B] Skeletons size: " + skeletonsB.Count);
  System.Diagnostics.Debug.WriteLine("Distance A - B: " + distance);
  */