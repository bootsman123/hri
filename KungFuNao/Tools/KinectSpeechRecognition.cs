using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using System.Threading;

namespace KungFuNao.Tools
{
    public class KinectSpeechRecognition
    {
        private KinectSensor sensor;
        private SpeechRecognitionEngine speechEngine;
        private RecognizerInfo ri;
        bool firedEvent;
        String recognisedWord;
        private AutoResetEvent speechRecognizedEvent;

        public KinectSpeechRecognition(KinectSensor kinectSensor)
        {
            this.sensor = kinectSensor;
        }

        public void start()
        {
            /*
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                try
                {
                    // Start the sensor!
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }
            }
             * */

            this.speechRecognizedEvent = new AutoResetEvent(false);

            ri = GetKinectRecognizer();

            if (null != ri)
            {

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);


                setGrammar(new string[] { "forward", "backward" });


                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;


                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                System.Console.WriteLine("Recognition started");
            }
            else
            {
                System.Console.WriteLine("No recognizer found");
            }
        }

        public void setGrammar(string[] words)
        {
            speechEngine.UnloadAllGrammars();
            var directions = new Choices();
            foreach (string word in words)
            {

                directions.Add(new SemanticResultValue(word, word));
            }
            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(directions);

            var g = new Grammar(gb);
            speechEngine.LoadGrammar(g);

        }

        public void end()
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            System.Console.WriteLine("Something recognized");
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            // const double ConfidenceThreshold = 0.3;


            //if (e.Result.Confidence >= ConfidenceThreshold)
            //{
            System.Console.WriteLine(e.Result.Semantics.Value.ToString());
            recognisedWord = e.Result.Semantics.Value.ToString();
            firedEvent = true;
            this.speechRecognizedEvent.Set();
            //}
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            System.Console.WriteLine("Speech rejected!");
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        internal string getAnswerToQuestion(string[] options)
        {
            setGrammar(options);
            firedEvent = false;
            while (!firedEvent)
            {

            }
            return recognisedWord;
        }

        internal string askConfirmation()
        {
            speechEngine.UnloadAllGrammars();
            string[] positiveOptions = new string[] { "yes", "sure", "yeah", "please", "ok", "okay" };
            string[] negativeOptions = new string[] { "no", "nope" };
            var options = new Choices();
            foreach (string posive in positiveOptions)
            {
                options.Add(new SemanticResultValue(posive, "YES"));
            }
            foreach (string negative in negativeOptions)
            {
                options.Add(new SemanticResultValue(negative, "NO"));
            }

            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(options);

            var g = new Grammar(gb);
            
            speechEngine.LoadGrammar(g);
            

            firedEvent = false;
            this.speechRecognizedEvent.WaitOne(10000);
           
            return recognisedWord;
        }

        internal string askLeftRight()
        {
            speechEngine.UnloadAllGrammars();
            string[] positiveOptions = new string[] { "left" };
            string[] negativeOptions = new string[] { "right" };
            var options = new Choices();
            foreach (string posive in positiveOptions)
            {
                options.Add(new SemanticResultValue(posive, "LEFT"));
            }
            foreach (string negative in negativeOptions)
            {
                options.Add(new SemanticResultValue(negative, "RIGHT"));
            }

            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(options);

            var g = new Grammar(gb);
            speechEngine.LoadGrammar(g);



            firedEvent = false;
            while (!firedEvent)
            {

            }
            return recognisedWord;
        }
    }
}
