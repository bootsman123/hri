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

namespace KungFuNao.ViewModels
{
    class MainWindowViewModel
    {
        private Preferences preferences;
        private Scenario scenario;

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

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            this.Preferences = new Preferences();

            this.LoadData();

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
    }
}
