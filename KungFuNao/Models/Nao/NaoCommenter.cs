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

        public void introduceMovement(int numberOfCurrentMovementInList)
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

        public void goodPerformanceCompleteKata()
        {
            this.Proxies.TextToSpeechProxy.say("That was a very good performance!");
        }

        public void badPerformanceCompleteKata()
        {
            this.Proxies.TextToSpeechProxy.say("Boy am I lucky, we will spend more time together practicing!");
        }

        public void goodPerformanceMotion()
        {
            this.Proxies.TextToSpeechProxy.say("This motion was performed very good!");
        }

        public void badPerformanceMotion()
        {
            this.Proxies.TextToSpeechProxy.say("Well, we need to do this again, but that is no problem for me!");
        }

        public void welcomeUser()
        {
            this.Proxies.BehaviorManagerProxy.runBehavior("karate/stand");
            this.Proxies.TextToSpeechProxy.post.say("Welcome to my robodojo.");
            this.Proxies.BehaviorManagerProxy.post.runBehavior(NaoBehaviors.BEHAVIOR_WELCOME);
            this.Proxies.TextToSpeechProxy.post.say("I hope you are ready for your karate training!");
        }

        public void explainKarateToUser()
        {
            this.Proxies.TextToSpeechProxy.say("Karate is a martial art where the user performs motions to defend himself");
            this.explainWhileShowingMuscles("When performing a lot of karate you will become stronger");
            this.Proxies.TextToSpeechProxy.say("I will try to learn you how to perform some karate motions!");
            this.Proxies.TextToSpeechProxy.say("Do you have any experience with Karate?");

            string choice = this.Proxies.KinectSpeechRecognition.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);
            switch (choice)
            {
                case KinectSpeechRecognition.CHOICE_POSITIVE:
                    this.Proxies.TextToSpeechProxy.say("In that case this will be an easy lesson!");
                    break;
                case KinectSpeechRecognition.CHOICE_NEGATIVE:
                    this.Proxies.TextToSpeechProxy.say("In that case I will explain the behaviours extra good");
                    break;
                default:
                    this.explainWhileMoving("I'm sorry, I didn't hear you. Feel free to speak to me!");
                    break;
            }
        }

        public void sayGoodbyeGoodPerformance()
        {
            this.Proxies.TextToSpeechProxy.say("You performed very well! So much for todays lesson");
            this.Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.GOOD_PERFORMANCE1);
            this.Proxies.TextToSpeechProxy.say("Keep on this level of practice");
            this.askUserForFeedback();
        }

        public void sayGoodbyeLongPerformance()
        {
            this.Proxies.TextToSpeechProxy.say("You still need some more practice, but you have to do this in your own time");
            this.explainWhileShowingMuscles("Perform these kata's everyday to become stronger");
            
            askUserForFeedback();
        }

        public void askUserForFeedback()
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
        
        public void explainWhileMoving(String toExplainText)
        {   
            Random rnd = new Random();
            int random = rnd.Next(NaoBehaviors.EXPLAINING_MOVEMENTS.Count);
            SpeakAndMove(toExplainText, NaoBehaviors.EXPLAINING_MOVEMENTS[random]);
        }
        public void explainWhileShowingMuscles(String toExplainText)
        {
            Random rnd = new Random();
            int random = rnd.Next(NaoBehaviors.MUSCLE_MOVEMENTS.Count);
            SpeakAndMove(toExplainText, NaoBehaviors.MUSCLE_MOVEMENTS[random]);
        }

        public void explainWhileStandingWhileWaiting(String toExplainText)
        {
            this.Proxies.TextToSpeechProxy.say(toExplainText);
        }

        public void explainWhileStandingWhitoutWaiting(String toExplainText)
        {
            this.Proxies.TextToSpeechProxy.post.say(toExplainText);
        }

        public void explainWithMovement(String toExplainText, String nameOfMovement)
        {
            this.Proxies.TextToSpeechProxy.post.say(toExplainText);
            this.Proxies.BehaviorManagerProxy.runBehavior(nameOfMovement);
        }

        public void startEvaluationOfWholeKata()
        {
            this.Proxies.TextToSpeechProxy.post.say("Hopefully you understand the whole technique, now let's see what you are able to do");
            this.Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_ACT_EXCITED);
            this.explainWhileMoving("Please follow along while we both perform the complete technique");
            this.explainWhileMoving("I will watch you closely to determine how well you perform the technique, So make sure you make the same movements as I do.");
            
        }
    }
}
