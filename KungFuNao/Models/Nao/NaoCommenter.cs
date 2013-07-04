using Aldebaran.Proxies;
using KungFuNao.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models.Nao
{
    class NaoCommenter
    {
        #region Fields.
        private Proxies Proxies;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Proxies"></param>
        /// <param name="KinectSpeechRecognition"></param>
        public NaoCommenter(Proxies Proxies)
        {
            this.Proxies = Proxies;
        }

        public void IntroduceMovement(int numberOfCurrentMovementInList)
        {
            if (numberOfCurrentMovementInList < 1)
            {
                this.Proxies.TextToSpeechProxy.say("Let us start with the first movement");
            }
            else
            {
                this.Proxies.TextToSpeechProxy.say("Now on to the next movement");
            }
        }

        public void GoodPerformanceCompleteScenario()
        {
            this.Proxies.TextToSpeechProxy.say("That was a very good performance!");
        }

        public void BadPerformanceCompleteScenario()
        {
            this.Proxies.TextToSpeechProxy.say("Boy am I lucky, we will spend more time together practicing!");
        }

        public void GoodPerformanceMotion()
        {
            this.Proxies.TextToSpeechProxy.say("This motion was performed very good!");
        }

        public void BadPerformanceMotion()
        {
            this.Proxies.TextToSpeechProxy.say("Well, we need to do this again, but that is no problem for me!");
        }

        public void WelcomeUser()
        {
            this.Proxies.BehaviorManagerProxy.runBehavior("karate/stand");
            this.Proxies.TextToSpeechProxy.post.say("Welcome to my robodojo.");
            this.Proxies.BehaviorManagerProxy.post.runBehavior(NaoBehaviors.BEHAVIOR_WELCOME);
            this.ExplainWhileMoving("I hope you are ready for your robot karate training!");
        }

        public void ExplainScenario()
        {
            this.ExplainWhileMoving("Robot Karate is a special martial art where the user performs movements to defend himself againest evil robots.");
            this.ExplainWhileFlexing("When performing a lot of robot karate you will become stronger");
            this.ExplainWhileMoving("I will try to learn you how to perform some robot karate movements!");
            this.ExplainWhileMoving("Do you have any experience with robot karate?");

            string choice = this.Proxies.KinectSpeechRecognition.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);
            switch (choice)
            {
                case KinectSpeechRecognition.CHOICE_POSITIVE:
                    this.ExplainWhileMoving("In that case this will be an easy lesson!");
                    break;
                case KinectSpeechRecognition.CHOICE_NEGATIVE:
                    this.ExplainWhileMoving("In that case I will explain the behaviors extra good.");
                    break;
                default:
                    this.ExplainWhileMoving("I'm sorry, I didn't hear you. Feel free to speak to me!");
                    break;
            }
        }

        public void GoodbyeGoodPerformance()
        {
            this.Proxies.TextToSpeechProxy.say("You performed very well! So much for todays lesson.");
            this.Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.GOOD_PERFORMANCE1);
            this.Proxies.TextToSpeechProxy.say("Keep on this level of practice.");
            this.AskForFeedback();
        }

        public void GoodbyeLongPerformance()
        {
            this.Proxies.TextToSpeechProxy.say("You still need some more practice, but you have to do this in your own time.");
            this.ExplainWhileFlexing("Perform these robot katas every day to become stronger.");
            this.AskForFeedback();
        }

        public void AskForFeedback()
        {
            this.Proxies.TextToSpeechProxy.say("Did you enjoy this lesson?");
            string choice = this.Proxies.KinectSpeechRecognition.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);
            switch (choice)
            {
                case KinectSpeechRecognition.CHOICE_POSITIVE:
                    this.Proxies.TextToSpeechProxy.say("I am happy to hear this!");
                    break;
                case KinectSpeechRecognition.CHOICE_NEGATIVE:
                default:
                    this.Proxies.TextToSpeechProxy.say("Aw, too bad.");
                    break;
            }
        }

        private void SpeakAndMove(String text, String movement)
        {
            this.Proxies.TextToSpeechProxy.post.say(text);
            this.Proxies.BehaviorManagerProxy.runBehavior(movement);
        }
        
        public void ExplainWhileMoving(String toExplainText)
        {   
            Random random = new Random();
            int randomBehavior = random.Next(NaoBehaviors.EXPLAINING_MOVEMENTS.Count);
            SpeakAndMove(toExplainText, NaoBehaviors.EXPLAINING_MOVEMENTS[randomBehavior]);
        }

        public void ExplainWhileFlexing(String toExplainText)
        {
            Random random = new Random();
            int randomBehavior = random.Next(NaoBehaviors.MUSCLE_MOVEMENTS.Count);
            SpeakAndMove(toExplainText, NaoBehaviors.MUSCLE_MOVEMENTS[randomBehavior]);
        }

        public void ExplainWhileStandingAndWaiting(String toExplainText)
        {
            this.Proxies.TextToSpeechProxy.say(toExplainText);
        }

        public void ExplainWhileStandingWhitoutWaiting(String toExplainText)
        {
            this.Proxies.TextToSpeechProxy.post.say(toExplainText);
        }

        public void ExplainWithMovement(String toExplainText, String nameOfMovement)
        {
            this.Proxies.TextToSpeechProxy.post.say(toExplainText);
            this.Proxies.BehaviorManagerProxy.runBehavior(nameOfMovement);
        }

        public void StartEvaluationOfWholeScenario(int currentTrial = 0)
        {
            String messageGeneral = "Hopefully you understand the whole technique. Now let's see what you are able to do. Please follow along while we both perform the complete technique";
            String messageMoving = "I will watch you closely to determine how well you perform the technique. So make sure you make the same movements as I do.";

            if (currentTrial > 0)
            {
                messageGeneral = "Hopefully you understand the whole technique now. Please follow along while we both perform the complete technique";
                messageMoving = "Remember to make sure you make the same movements as I do.";
            }
            else if (currentTrial > 0)
            {
            }

            this.Proxies.TextToSpeechProxy.post.say(messageGeneral);
            this.Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_ACT_EXCITED);
            this.ExplainWhileMoving(messageMoving);
            this.Proxies.TextToSpeechProxy.say("Get ready.");
            this.Proxies.TextToSpeechProxy.say("");
        }
    }
}
