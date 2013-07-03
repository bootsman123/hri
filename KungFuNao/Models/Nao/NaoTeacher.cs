using Aldebaran.Proxies;
using DynamicTimeWarping;
using Kinect.Toolbox.Record;
using KungFuNao.Tools;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models.Nao
{
    public class NaoTeacher
    {
        private readonly BackgroundWorker Worker = new BackgroundWorker();

        public static int MAXIMUM_AMOUNT_OF_TRIALS = 3;
        public static double PERFORMANCE_TRESHOLD_FOR_FINALIZING_LESSON = 0.3;

        #region Fields.
        private Preferences Preferences;
        private Proxies Proxies;
        private Scenario Scenario;

        private NaoCommenter NaoCommenter;

        private Stream RecordStream;
        private KinectRecorder KinectRecorder;
        private bool IsRecording = false;
        #endregion

        public NaoTeacher(Preferences Preferences, Proxies Proxies, Scenario Scenario)
        {
            this.Preferences = Preferences;
            this.Proxies = Proxies;
            this.Scenario = Scenario;

            this.NaoCommenter = new NaoCommenter(this.Proxies);

            // Create thread.
            this.Worker.DoWork += Run;
            this.Worker.WorkerSupportsCancellation = true;
        }

        private void Run(object sender, DoWorkEventArgs e)
        {
            /*
            System.Diagnostics.Debug.WriteLine("NaoTeacher: asking confirmation...");
            String choice = this.speech.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);

            if (KinectSpeechRecognition.CHOICE_POSITIVE.Equals(choice))
            {
                System.Diagnostics.Debug.WriteLine("NaoTeacher: Positive confirmation =)!");
            }
            else if (KinectSpeechRecognition.CHOICE_NEGATIVE.Equals(choice))
            {
                System.Diagnostics.Debug.WriteLine("NaoTeacher: Negative confirmation :(.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("NaoTeacher: No confirmation...");
            }
            */
           
            //welcomeUser();
            //explainCompleteKata();
            //explainEveryKataMotion();
            int trialNumber = 0;
            trainUser(trialNumber);
        }

        /// <summary>
        /// Start Nao teacher.
        /// </summary>
        public void Start()
        {
            this.Worker.RunWorkerAsync();
        }

        /// <summary>
        /// Stop Nao teacher.
        /// </summary>
        public void Stop()
        {
            this.Worker.CancelAsync();
        }

        public void trainUser(int trial)
        {
            double[] performances = evaluateKata();

            if (goodPerformance(performances))
            {
                this.NaoCommenter.sayGoodbyeGoodPerformance();
            }
            else if (trial > NaoTeacher.MAXIMUM_AMOUNT_OF_TRIALS)
            {
                this.NaoCommenter.sayGoodbyeLongPerformance();
            }
            else
            {
                giveSpecificFeedback(performances);
                trainUser(trial + 1);
            }
        }

        private void giveSpecificFeedback(double[] performances)
        {
            double minimum = performances.Min();
            int location = Array.IndexOf(performances, minimum);
            Scene worstScene = this.Scenario.ElementAt(location);

            this.NaoCommenter.explainWhileMoving("Your " + worstScene.Name + " needs some improvement, let me explain the " + worstScene.Name + " again");
            worstScene.giveFeedbackToUser(this.Proxies);
            worstScene.explainToUser(this.Proxies);
        }

        private bool goodPerformance(double[] performances)
        {
            return performances.Sum() > NaoTeacher.PERFORMANCE_TRESHOLD_FOR_FINALIZING_LESSON;
        }

        private void welcomeUser()
        {
            this.NaoCommenter.welcomeUser();
            this.NaoCommenter.explainKarateToUser();
        }

        private double[] evaluateKata()
        {
            var distance = new SkeletonDistance();

            this.NaoCommenter.startEvaluationOfWholeKata();

            double[] performance = new double[3];
            int x = 0;
            foreach (Scene scene in this.Scenario)
            {
                this.startRecording();
                scene.performDefault(this.Proxies);
                this.stopRecording();

                // Load data.
                List<Skeleton> skeletons = this.loadRecording();
                performance[x++] = DTW<Skeleton>.Distance(skeletons, scene.Skeletons, distance);
            }
            return performance;
        }

        private void explainCompleteKata()
        {
            this.NaoCommenter.explainWhileMoving("Today we are going to focus on a robot technique, it is used to defend against evil robots");
            this.NaoCommenter.explainWhileStandingWhileWaiting("The complete technique looks like this");

            foreach (Scene scene in this.Scenario)
            {
                scene.performDefault(this.Proxies);
            }
        }

        private void explainEveryKataMotion()
        {
            this.NaoCommenter.explainWithMovement("I will now explain every motion you need to perfom.", NaoBehaviors.BEHAVIOR_EXPLAIN2);
            this.NaoCommenter.explainWhileStandingWhileWaiting("Please move your body along with my body! Note that you have to mirror my body. ");
            //this.NaoCommenter.explainWhileStandingWhileWaiting("I hope you are ready!");
            int movementNumber = 0;
            foreach (Scene scene in this.Scenario)
            {
                this.NaoCommenter.introduceMovement(movementNumber++);
                scene.explainToUser(this.Proxies);
            }
        }

        private void startRecording()
        {
            // Start recording.
            this.RecordStream = new BufferedStream(new FileStream(this.Preferences.KinectDataFile, FileMode.Create));
            this.KinectRecorder = new KinectRecorder(KinectRecordOptions.Skeletons, this.RecordStream);

            this.IsRecording = true;
        }

        private void stopRecording()
        {
            // Stop recording.
            this.IsRecording = false;

            this.KinectRecorder.Stop();
        }

        private List<Skeleton> loadRecording()
        {
            using (Stream stream = new BufferedStream(new FileStream(this.Preferences.KinectDataFile, FileMode.Open)))
            {
                return Tools.SkeletonRecordingConverter.FromStream(stream);
            }
        }

        /// <summary>
        /// On skeleton frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KinectSensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }

                // Record?
                if (this.IsRecording)
                {
                    this.KinectRecorder.Record(skeletonFrame);
                }
            }
        }
    }
}
