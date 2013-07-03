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

        #region Fields.
        private Preferences Preferences;
        private Proxies Proxies;
        private Scenario Scenario;

        private NaoCommenter NaoCommenter;

        private int CurrentTrial;

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

            this.CurrentTrial = 0;

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
           
            this.WelcomeUser();
            this.ExplainCompleteKata();
            this.ExplainEveryKataMotion();
            this.TrainUser();
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

        public void TrainUser()
        {
            List<Double> performances = this.EvaluateScenario();

            if (this.HasGoodPerformance(performances))
            {
                this.NaoCommenter.sayGoodbyeGoodPerformance();
            }
            else if (this.CurrentTrial > NaoTeacher.MAXIMUM_AMOUNT_OF_TRIALS)
            {
                this.NaoCommenter.sayGoodbyeLongPerformance();
            }
            else
            {
                GiveFeedbackOnScene(performances);

                this.CurrentTrial++;
                TrainUser();
            }
        }

        private void GiveFeedbackOnScene(List<Double> performances)
        {
            int index = performances.IndexOf(performances.Min());
            Scene worstScene = this.Scenario.ElementAt(index);

            this.NaoCommenter.explainWhileMoving("Your " + worstScene.Name + " needs some improvement, let me explain the " + worstScene.Name + " again");
            worstScene.giveFeedbackToUser(this.Proxies);
            worstScene.explainToUser(this.Proxies);
        }

        private bool HasGoodPerformance(List<Double> performances)
        {
            double totalPerformance = performances.Sum();
            double totalMinimumScore = this.Scenario.TotalMinimumScore();
            double totalMaximumScore = this.Scenario.TotalMaximumScore();

            System.Diagnostics.Debug.WriteLine("NaoTeacher::HasGoodPerformance() - Total performance: " + totalPerformance);
            System.Diagnostics.Debug.WriteLine("NaoTeacher::HasGoodPerformance() - Total minimum score: " + totalMinimumScore);
            System.Diagnostics.Debug.WriteLine("NaoTeacher::HasGoodPerformance() - Total maximum score: " + totalMaximumScore);

            return (totalPerformance >= totalMinimumScore &&
                    totalPerformance <= totalMaximumScore);
        }

        private void WelcomeUser()
        {
            this.NaoCommenter.welcomeUser();
            this.NaoCommenter.explainKarateToUser();
        }

        private List<Double> EvaluateScenario()
        {
            System.Diagnostics.Debug.WriteLine("NaoTeacher::evaluateKata()");

            this.NaoCommenter.startEvaluationOfWholeKata();

            var performances = new List<Double>();
            var distance = new SkeletonDistance();

            foreach (Scene scene in this.Scenario)
            {
                this.StartRecording();
                scene.performDefault(this.Proxies);
                this.StopRecording();

                // Load data.
                List<Skeleton> skeletons = this.LoadRecording();

                // Calculate performance.
                scene.DeterminePerformance(skeletons);
                performances.Add(scene.Performance);
            }

            return performances;
        }

        private void ExplainCompleteKata()
        {
            this.NaoCommenter.explainWhileMoving("Today we are going to focus on a robot technique, it is used to defend against evil robots");
            this.NaoCommenter.explainWhileStandingWhileWaiting("The complete technique looks like this");

            foreach (Scene scene in this.Scenario)
            {
                scene.performDefault(this.Proxies);
            }
        }

        private void ExplainEveryKataMotion()
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

        /// <summary>
        /// Start recording.
        /// </summary>
        private void StartRecording()
        {
            // Start recording.
            this.RecordStream = new BufferedStream(new FileStream(this.Preferences.KinectDataFile, FileMode.Create));
            this.KinectRecorder = new KinectRecorder(KinectRecordOptions.Skeletons, this.RecordStream);

            this.IsRecording = true;
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        private void StopRecording()
        {
            // Stop recording.
            this.IsRecording = false;

            this.KinectRecorder.Stop();
        }

        /// <summary>
        /// Load recording.
        /// </summary>
        /// <returns></returns>
        private List<Skeleton> LoadRecording()
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
