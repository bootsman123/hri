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
using Microsoft.TeamFoundation.MVVM;

namespace KungFuNao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields.
        private MainWindowViewModel MainWindowViewModel;

        private SkeletonDisplayManager SkeletonDisplayManager;
        private Skeleton[] Skeletons;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.MainWindowViewModel = new MainWindowViewModel();
            this.Loaded += this.MainWindowViewModel.OnWindowLoaded;
            this.Closed += this.MainWindowViewModel.OnWindowClosed;

            this.DataContext = this.MainWindowViewModel;

            // Skeleton stream.
            this.SkeletonDisplayManager = new SkeletonDisplayManager(this.MainWindowViewModel.Proxies.KinectSensor, this.ImageCanvas);
            this.Skeletons = null;

            this.MainWindowViewModel.Proxies.KinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.KinectSensorSkeletonFrameReady);
        }

        /// <summary>
        /// On skeleton frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectSensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame SkeletonFrame = e.OpenSkeletonFrame())
            {
                if (SkeletonFrame == null)
                {
                    return;
                }

                // Display real time?
                if (this.MainWindowViewModel.Mode != KungFuNao.ViewModels.MainWindowViewModel.ControlMode.Replay)
                {
                    Kinect.Toolbox.Tools.GetSkeletons(SkeletonFrame, ref this.Skeletons);
                    this.SkeletonDisplayManager.Draw(this.Skeletons, false);
                }
            }
        }

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