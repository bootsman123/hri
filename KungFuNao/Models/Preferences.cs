using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models
{
    public class Preferences : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Constants.
        private readonly static ColorImageFormat COLOR_IMAGE_FORMAT = ColorImageFormat.RgbResolution640x480Fps30;
        private readonly static string NAO_IP_ADDRESS = "nao.local";
        private readonly static int NAO_PORT = 9559;

        private readonly static string KINECT_DATA_FILE_PATH = "C:\\Users\\bootsman\\Desktop\\";
        private readonly static string KINECT_DATA_FILE_NAME = "data.kinect";
        private readonly static string KINECT_DATA_FILE = Path.Combine(Preferences.KINECT_DATA_FILE_PATH, Preferences.KINECT_DATA_FILE_NAME);

        private readonly static string SCENARIOS_FILE_PATH = "C:\\Users\\bootsman\\Desktop\\";
        private readonly static string SCENARIOS_FILE_NAME = "scenarios.xml";
        private readonly static string SCENARIOS_FILE = Path.Combine(Preferences.SCENARIOS_FILE_PATH, Preferences.SCENARIOS_FILE_NAME);
        #endregion

        #region Fields.
        private ColorImageFormat colorImageFormat;
        private int imageWidth;
        private int imageHeight;

        private string naoIpAddress;
        private int naoPort;

        private string kinectDataFile;
        private string scenariosFile;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="colorImageFormat"></param>
        public Preferences()
        {
            this.ColorImageFormat = Preferences.COLOR_IMAGE_FORMAT;
            this.NaoIpAddress = Preferences.NAO_IP_ADDRESS;
            this.NaoPort = Preferences.NAO_PORT;
            this.KinectDataFile = Preferences.KINECT_DATA_FILE;
            this.ScenariosFile = Preferences.SCENARIOS_FILE;
        }

        public int ImageWidth
        {
            get { return this.imageWidth; }
            set
            {
                this.imageWidth = value;
                this.OnPropertyChanged("ImageWidth");
            }
        }

        public int ImageHeight
        {
            get { return this.imageHeight; }
            set
            {
                this.imageHeight = value;
                this.OnPropertyChanged("ImageHeight");
            }
        }

        public ColorImageFormat ColorImageFormat
        {
            get { return this.colorImageFormat; }
            set
            {
                this.colorImageFormat = value;

                switch (this.colorImageFormat)
                {
                    case ColorImageFormat.RgbResolution640x480Fps30:
                        this.ImageWidth = 640;
                        this.ImageHeight = 480;
                        break;
                    case ColorImageFormat.RgbResolution1280x960Fps12:
                    default:
                        this.ImageWidth = 1280;
                        this.ImageHeight = 960;
                        break;
                }

                this.OnPropertyChanged("ColorImageFormat");
            }
        }

        public string NaoIpAddress
        {
            get { return this.naoIpAddress; }
            set
            {
                this.naoIpAddress = value;
                this.OnPropertyChanged("NaoIpAddress");
            }
        }

        public int NaoPort
        {
            get { return this.naoPort; }
            set
            {
                this.naoPort = value;
                this.OnPropertyChanged("NaoPort");
            }
        }

        public string KinectDataFile
        {
            get { return this.kinectDataFile; }
            set
            {
                this.kinectDataFile = value;
                this.OnPropertyChanged("KinectDataFile");
            }
        }

        public string ScenariosFile
        {
            get { return this.scenariosFile; }
            set
            {
                this.scenariosFile = value;
                this.OnPropertyChanged("ScenariosFile");
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
