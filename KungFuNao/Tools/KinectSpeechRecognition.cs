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
        private bool firedEvent;
        private String recognisedWord;

        public KinectSpeechRecognition(KinectSensor kinectSensor)
        {
            this.sensor = kinectSensor;
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

        public void start()
        {
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::start() - Begin");

            this.ri = GetKinectRecognizer();

            if (this.ri == null)
            {
                return;
            }

            this.speechEngine = new SpeechRecognitionEngine(ri.Id);

            this.setGrammar(new string[] { "forward", "backward" });

            this.speechEngine.SpeechRecognized += SpeechRecognized;
            this.speechEngine.SpeechRecognitionRejected += SpeechRejected;

            this.speechEngine.SetInputToAudioStream(sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);

            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::start() - End");
        }

        public void stop()
        {
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::stop() - Begin");

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }

            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor = null;
            }

            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::stop() - End");
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::SpeechRecognized()");

            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            // const double ConfidenceThreshold = 0.3;


            //if (e.Result.Confidence >= ConfidenceThreshold)
            //{
            System.Console.WriteLine(e.Result.Semantics.Value.ToString());
            recognisedWord = e.Result.Semantics.Value.ToString();
            firedEvent = true;
            //}
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::SpeechRejected()");
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
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::askConfirmation()");

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

            /*
            Task task = Task.Factory.StartNew(() => DoTask());
            task.Wait();
             * */


            firedEvent = false;
            while (!firedEvent)
            {
                System.Diagnostics.Debug.WriteLine("Event has not fired...");
                Thread.Sleep(100);
            }

            System.Diagnostics.Debug.WriteLine("RECOGNIZED WORD:" + recognisedWord);

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
