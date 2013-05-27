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

namespace KungFuNao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string KATAS_FILE = "katas.xml";

        private KinectSensor kinectSensor;

        private SkeletonDisplayManager skeletonDisplayManager;
        private Skeleton[] skeletons;

        private List<Kata> katas;

        public MainWindow()
        {
            InitializeComponent();

            //System.Diagnostics.Debug.WriteLine("Hello World!");

            this.kinectSensor = KinectSensor.KinectSensors.First(e => e.Status == KinectStatus.Connected);

            if (this.kinectSensor == null)
            {
                return;
            }

            this.buttonRecordKata.Click += new RoutedEventHandler(this.buttonRecordKataOnClick);

            // Color stream.
            this.kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            this.kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(this.kinectSensorColorFrameReady);

            // Skeleton stream.
            this.skeletonDisplayManager = new SkeletonDisplayManager(this.kinectSensor, this.imageCanvas);
            this.skeletons = null;

            this.kinectSensor.SkeletonStream.Enable();
            this.kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.kinectSensorSkeletonFrameReady);

            this.kinectSensor.Start();
        }

        private void buttonRecordKataOnClick(object sender, RoutedEventArgs e)
        {
            // http://stackoverflow.com/questions/13615696/obtaining-data-from-kinect-for-specific-time
        }

        /// <summary>
        /// On color frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kinectSensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                this.imageKinect.Source = colorFrame.ToBitmapSource();
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

                if (this.skeletons == null ||
                    this.skeletons.Length != skeletonFrame.SkeletonArrayLength)
                {
                    this.skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                skeletonFrame.CopySkeletonDataTo(this.skeletons);

                this.skeletonDisplayManager.Draw(skeletons, false);
            }
        }

        private void Serialize()
        {
            using (TextReader reader = new StreamReader(MainWindow.KATAS_FILE))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<Kata>));
                this.katas = (List<Kata>)deserializer.Deserialize(reader);
            }
        }

        private void Deserialize()
        {
            using (TextWriter writer = new StreamWriter(MainWindow.KATAS_FILE))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Kata>));
                serializer.Serialize(writer, this.katas);
            }
        }
    }
}
