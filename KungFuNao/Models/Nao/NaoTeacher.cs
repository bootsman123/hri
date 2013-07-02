﻿using Aldebaran.Proxies;
using DynamicTimeWarping;
using Kinect.Toolbox.Record;
using KungFuNao.Tools;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models.Nao
{
    public class NaoTeacher
    {
        public static int MAXIMUM_AMOUNT_OF_TRIALS = 3;
        public static double PERFORMANCE_TRESHOLD_FOR_FINALIZING_LESSON = 0.3;

        private TextToSpeechProxy textToSpeechProxy;
        private BehaviorManagerProxy behaviorManagerProxy;
        private KinectSpeechRecognition speech;

        private NaoCommenter naoCommenter;
        private Scenario scenario;

        private Stream recordStream;
        private KinectRecorder kinectRecorder;
        private bool isRecording = false;

        public NaoTeacher(TextToSpeechProxy textToSpeechProxy, BehaviorManagerProxy behaviorManagerProxy, KinectSpeechRecognition speech, Scenario scenario)
        {
            this.textToSpeechProxy = textToSpeechProxy;
            this.behaviorManagerProxy = behaviorManagerProxy;
            this.naoCommenter = new NaoCommenter(textToSpeechProxy, behaviorManagerProxy);
            this.speech = speech;
            this.scenario = scenario;
        }

        public void startProgram()
        {
            welcomeUser();
            explainCompleteKata();
            explainEveryKataMotion();
            int trialNumber = 0;
            trainUser(trialNumber);

        }

        public void trainUser(int trial)
        {
            double[] performances = evaluateKata();

            if (goodPerformance(performances))
            {
                naoCommenter.sayGoodbyeGoodPerformance(speech);
            }
            else if (trial > NaoTeacher.MAXIMUM_AMOUNT_OF_TRIALS)
            {
                naoCommenter.sayGoodbyeLongPerformance(speech);
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
            Scene worstScene = this.scenario.ElementAt(location);

            naoCommenter.explainWhileMoving("Your " + worstScene.Name + " needs some improvement, let me explain the " + worstScene.Name + " again");
            worstScene.giveFeedbackToUser(textToSpeechProxy, behaviorManagerProxy);
            worstScene.explainToUser(textToSpeechProxy, behaviorManagerProxy, speech);
        }

        private bool goodPerformance(double[] performances)
        {
            return performances.Sum() > NaoTeacher.PERFORMANCE_TRESHOLD_FOR_FINALIZING_LESSON;
        }

        private void welcomeUser()
        {
            naoCommenter.welcomeUser();
            naoCommenter.explainKarateToUser(speech);
        }

        private double[] evaluateKata()
        {
            var distance = new SkeletonDistance();

            naoCommenter.startEvaluationOfWholeKata();

            double[] performance = new double[3];
            int x = 0;
            foreach (Scene scene in this.scenario)
            {
                this.startRecording();
                scene.performDefault(textToSpeechProxy, behaviorManagerProxy);
                this.stopRecording();

                // Load data.
                List<Skeleton> skeletons = this.loadRecording();
                performance[x++] = DTW<Skeleton>.Distance(skeletons, scene.Skeletons, distance);
            }
            return performance;
        }

        private void explainCompleteKata()
        {
            naoCommenter.explainWhileMoving("Today we are going to focus on a robot technique, it is used to defend against evil robots");

            textToSpeechProxy.say("The complete technique looks like this");

            foreach (Scene scene in this.scenario)
            {
                scene.performDefault(textToSpeechProxy, behaviorManagerProxy);
            }
        }

        private void explainEveryKataMotion()
        {
            textToSpeechProxy.post.say("I will now explain every motion you need to perfom.");
            behaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_EXPLAIN2);
            textToSpeechProxy.say("Please move your body along with my body!");
            textToSpeechProxy.say("I hope you are ready!");
            int movementNumber = 0;
            foreach (Scene scene in this.scenario)
            {
                this.naoCommenter.introduceMovement(movementNumber++);
                scene.explainToUser(textToSpeechProxy, behaviorManagerProxy, speech);
            }
        }

        private void startRecording()
        {
            // Start recording.
            this.recordStream = new BufferedStream(new FileStream(Preferences.KINECT_DATA_FILE, FileMode.Create));
            this.kinectRecorder = new KinectRecorder(KinectRecordOptions.Skeletons, this.recordStream);

            this.isRecording = true;
        }

        private void stopRecording()
        {
            // Stop recording.
            this.isRecording = false;

            this.kinectRecorder.Stop();
        }

        private List<Skeleton> loadRecording()
        {
            using (Stream stream = new BufferedStream(new FileStream(Preferences.KINECT_DATA_FILE, FileMode.Open)))
            {
                return Tools.SkeletonRecordingConverter.FromStream(stream);
            }
        }

        /// <summary>
        /// On skeleton frame ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void kinectSensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }

                // Record?
                if (this.isRecording)
                {
                    this.kinectRecorder.Record(skeletonFrame);
                }
            }
        }
    }
}
