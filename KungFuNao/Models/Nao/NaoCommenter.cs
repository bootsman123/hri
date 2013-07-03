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
        private TextToSpeechProxy textToSpeechProxy;
        private BehaviorManagerProxy behaviorManagerProxy;

        public NaoCommenter(TextToSpeechProxy textToSpeechProxy, BehaviorManagerProxy behaviorManagerProxy)
        {
            this.textToSpeechProxy = textToSpeechProxy;
            this.behaviorManagerProxy = behaviorManagerProxy;
        }

        public void introduceMovement(int numberOfCurrentMovementInList)
        {
            if (numberOfCurrentMovementInList < 1)
            {
                textToSpeechProxy.say("Let us start with the first movement");
            }
            else
            {
                textToSpeechProxy.say("Now on to the next movement");
            }
        }

        public void goodPerformanceCompleteKata()
        {
            textToSpeechProxy.say("That was a very good performance!");
        }

        public void badPerformanceCompleteKata()
        {
            textToSpeechProxy.say("Boy am I lucky, we will spend more time together practicing!");
        }

        public void goodPerformanceMotion()
        {
            textToSpeechProxy.say("This motion was performed very good!");
        }

        public void badPerformanceMotion()
        {
            textToSpeechProxy.say("Well, we need to do this again, but that is no problem for me!");
        }
        public void welcomeUser()
        {
            behaviorManagerProxy.runBehavior("karate/stand");
            textToSpeechProxy.post.say("Welcome to my robodojo.");
            behaviorManagerProxy.post.runBehavior(NaoBehaviors.BEHAVIOR_WELCOME);
            textToSpeechProxy.post.say("I hope you are ready for your karate training!");
        }
        public void explainKarateToUser(KinectSpeechRecognition speech)
        {
            textToSpeechProxy.say("Karate is a martial art where the user performs motions to defend himself");
            textToSpeechProxy.say("I will try to learn you how to perform some karate motions!");

            textToSpeechProxy.say("Do you have any experience with Karate?");
            string answer = speech.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);
            switch (answer)
            {
                case "YES":
                    textToSpeechProxy.say("In that case this will be an easy lesson!");
                    break;
                case "NO":
                    textToSpeechProxy.say("In that case I will explain the behaviours extra good");
                    break;
            }
        }

        public void sayGoodbyeGoodPerformance(KinectSpeechRecognition speech)
        {
            textToSpeechProxy.say("You performed very well! So much for todays lesson");
            textToSpeechProxy.say("Keep on this level of practice");
            askUserForFeedback(speech);
        }

        public void sayGoodbyeLongPerformance(KinectSpeechRecognition speech)
        {
            textToSpeechProxy.say("You still need some more practice, keep on doing this everyday");
            askUserForFeedback(speech);
        }

        public void askUserForFeedback(KinectSpeechRecognition speech)
        {
            textToSpeechProxy.say("Did you enjoy this lesson?");
            string answer = speech.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);
            switch (answer)
            {
                case "YES":
                    textToSpeechProxy.say("I am happy to hear this!");
                    break;
                case "NO":
                    textToSpeechProxy.say("Aw, too bad.");
                    break;
            }
        }

        public void explainWhileMoving(String toExplainText)
        {
            textToSpeechProxy.post.say(toExplainText);
            List<String> possibleMovements = new List<String> { NaoBehaviors.BEHAVIOR_EXPLAIN1, NaoBehaviors.BEHAVIOR_EXPLAIN2 };
            Random rnd = new Random();
            int random = rnd.Next(possibleMovements.Count);

            behaviorManagerProxy.runBehavior(possibleMovements[random]);
        }
        public void explainWhileStandingWhileWaiting(String toExplainText)
        {

            textToSpeechProxy.say(toExplainText);
        }

        public void explainWhileStandingWhitoutWaiting(String toExplainText)
        {
            textToSpeechProxy.post.say(toExplainText);
        }
        public void explainWithMovement(String toExplainText, String nameOfMovement){
            this.textToSpeechProxy.post.say(toExplainText);
            this.behaviorManagerProxy.runBehavior(nameOfMovement);
        }

        public void startEvaluationOfWholeKata()
        {
            textToSpeechProxy.post.say("Hopefully you understand the whole technique, now let's see what you are able to do");
            behaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_ACT_EXCITED);
            textToSpeechProxy.say("Please follow along while we both perform the complete technique");
            textToSpeechProxy.say("I will watch you closely to determine how well you perform the technique");
            textToSpeechProxy.say("So make sure you make the same movements as I do");
        }
    }
}
