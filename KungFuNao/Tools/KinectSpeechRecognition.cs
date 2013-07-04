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
        public const double CONFIDENCE_THRESHOLD = 0.7;

        public const String CHOICE_POSITIVE = "POSITIVE";
        public const String CHOICE_NEGATIVE = "NEGATIVE";
        public const String CHOICE_LEFT = "LEFT";
        public const String CHOICE_RIGHT = "RIGHT";

        public static readonly List<String> CHOICES_POSITIVE_NEGATIVE = new List<String> {
            KinectSpeechRecognition.CHOICE_POSITIVE,
            KinectSpeechRecognition.CHOICE_NEGATIVE };
        public static readonly List<String> CHOICES_LEFT_RIGHT = new List<String> {
            KinectSpeechRecognition.CHOICE_LEFT,
            KinectSpeechRecognition.CHOICE_RIGHT };

        public Dictionary<String, List<String>> Dictionary { get; private set; }

        private KinectSensor KinectSensor;
        private RecognizerInfo RecognizerInfo;
        private SpeechRecognitionEngine SpeechRecognitionEngine;

        private AutoResetEvent SpeechRecognizedEvent;
        private String RecognizedChoice;
        private List<String> RecognizableChoices;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="kinectSensor"></param>
        public KinectSpeechRecognition(KinectSensor kinectSensor)
        {
            this.KinectSensor = kinectSensor;
            this.RecognizerInfo = this.FindRecognizerInfo();
            this.SpeechRecognitionEngine = new SpeechRecognitionEngine(this.RecognizerInfo); // this.RecognizerInfo.Id ?

            // Build and load grammar.
            this.Dictionary = this.BuildDictionary();
            this.SpeechRecognitionEngine.LoadGrammar(this.BuildGrammar(this.Dictionary));

            this.RecognizedChoice = "";
            this.RecognizableChoices = new List<String>();
        }

        /// <summary>
        /// Start speech recognition.
        /// </summary>
        public void Start()
        {
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::Start()");

            // Attach events.
            this.SpeechRecognitionEngine.SpeechRecognized += this.SpeechRecognized;

            this.SpeechRecognitionEngine.SetInputToAudioStream(
                this.KinectSensor.AudioSource.Start(),
                new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.SpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Stop speech recognition.
        /// </summary>
        public void Stop()
        {
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::Stop()");

            if (null != this.SpeechRecognitionEngine)
            {
                this.SpeechRecognitionEngine.SpeechRecognized -= this.SpeechRecognized;
                this.SpeechRecognitionEngine.RecognizeAsyncStop();
            }

            if (null != this.KinectSensor)
            {
                this.KinectSensor.AudioSource.Stop();
            }
        }

        /// <summary>
        /// Builds a dictionary of choices.
        /// </summary>
        /// <returns></returns>
        private Dictionary<String, List<String>> BuildDictionary()
        {
            Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
            dictionary.Add(KinectSpeechRecognition.CHOICE_POSITIVE, new List<String> { "yes", "sure", "yeah", "please", "ok", "okay", "little", "a bit" });
            dictionary.Add(KinectSpeechRecognition.CHOICE_NEGATIVE, new List<String> { "no", "nope", "never" });
            dictionary.Add(KinectSpeechRecognition.CHOICE_LEFT, new List<String> { "left", "left hand" });
            dictionary.Add(KinectSpeechRecognition.CHOICE_RIGHT, new List<String> { "right", "right hand" });

            return dictionary;
        }

        /// <summary>
        /// Builds the grammar.
        /// </summary>
        /// <param name="dictionary"></param>
        /*
        private Grammar BuildGrammar(Dictionary<String,List<String>> dictionary)
        {
            var grammarBuilder = new GrammarBuilder { Culture  = this.RecognizerInfo.Culture };

            foreach (KeyValuePair<String,List<String>> entry in dictionary)
            {
                var choice = new SemanticResultKey(entry.Key, new GrammarBuilder(new Choices(entry.Value.ToArray())));

                grammarBuilder.Append(choice);
            }

            var grammar = new Grammar(grammarBuilder);

            return grammar;
        }
        */

        /// <summary>
        /// Builds the grammar.
        /// </summary>
        /// <param name="dictionary"></param>
        private Grammar BuildGrammar(Dictionary<String, List<String>> dictionary)
        {
            var choices = new Choices();

            foreach (KeyValuePair<String, List<String>> entry in dictionary)
            {
                foreach (String choice in entry.Value)
                {
                    choices.Add(new SemanticResultValue(choice, entry.Key));
                }
            }

            var grammarBuilder = new GrammarBuilder { Culture = this.RecognizerInfo.Culture };
            grammarBuilder.Append(choices);

            return new Grammar(grammarBuilder);
        }

        /// <summary>
        /// Speech recognized event handler.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("KinectSpeechRecognition::SpeechRecognized - " + e.Result.Semantics.Value.ToString());

            if (e.Result.Confidence >= KinectSpeechRecognition.CONFIDENCE_THRESHOLD)
            {
                var value = e.Result.Semantics.Value.ToString();

                // Check if the recognized choice should match.
                if (this.RecognizableChoices.Contains(value))
                {
                    this.SpeechRecognizedEvent.Set();
                    this.RecognizedChoice = value;
                }
            }
        }

        /// <summary>
        /// Waits a certain amount of time for a choice. 
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="timeOut">Time out in seconds.</param>
        /// <returns></returns>
        public string WaitForChoice(List<String> choices, int timeOut = 10)
        {
            this.RecognizableChoices = choices;

            this.SpeechRecognizedEvent = new AutoResetEvent(false);
            this.SpeechRecognizedEvent.WaitOne(timeOut * 1000);

            var choice = this.RecognizedChoice;

            // Reset the recognized choice.
            this.RecognizedChoice = "";
            this.RecognizableChoices = new List<String>();

            return choice;
        }

        /// <summary>
        /// Finds recognizer info.
        /// </summary>
        /// <returns></returns>
        private RecognizerInfo FindRecognizerInfo()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);

                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) &&
                    "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }
    }
}
