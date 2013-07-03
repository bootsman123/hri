using Aldebaran.Proxies;
using KungFuNao.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models.Nao
{
    [DataContract]
    public class GedanBaraiScene : Scene
    {
        public GedanBaraiScene(string fileName, Score score)
            : base("Gedan Barai", fileName, score)
        {
        }

        public override void performDefault(Proxies Proxies)
        {
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI);
        }

        public override void explainToUser(Proxies Proxies)
        {
            this.NumberOfTimesExplained++;

            Proxies.TextToSpeechProxy.post.say("This motion is called " + this.Name + ", it is used to block an incoming kick");
            this.performDefault(Proxies);
            Proxies.TextToSpeechProxy.say("I will break it down for you in three steps");

            explanation(Proxies);

            if (NumberOfTimesExplained > 1)
            {
                Proxies.TextToSpeechProxy.say("I already explained this motion.");
                Proxies.TextToSpeechProxy.post.say("I see you do not yet really get this motion, let me explain both arms seperately");
                Proxies.BehaviorManagerProxy.runBehavior("naos-life-channel/stand_scratchHead1");
                explainLeftArm(Proxies);
                explainRightArm(Proxies);
                Proxies.TextToSpeechProxy.say("By combining these motions you get this.");
                this.performDefault(Proxies);

                Proxies.TextToSpeechProxy.say("Do you understand this motion now?");
                string choicePositiveNegative = Proxies.KinectSpeechRecognition.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);

                switch (choicePositiveNegative)
                {
                    case KinectSpeechRecognition.CHOICE_POSITIVE:
                        Proxies.TextToSpeechProxy.say("Alright, then let's continue");
                        break;
                    case KinectSpeechRecognition.CHOICE_NEGATIVE:
                    default:
                        Proxies.TextToSpeechProxy.say("Too bad, are you having trouble with your left or your right arm?");

                        string choiceLeftRight = Proxies.KinectSpeechRecognition.WaitForChoice(KinectSpeechRecognition.CHOICES_LEFT_RIGHT);
                        switch (choiceLeftRight)
                        {
                            case KinectSpeechRecognition.CHOICE_LEFT:
                                explainLeftArm(Proxies);
                                break;
                            case KinectSpeechRecognition.CHOICE_RIGHT:
                            default:
                                explainRightArm(Proxies);
                                break;
                        }
                        break;
                }
            }
        }

        private void explanation(Proxies Proxies)
        {
            // First step.
            Proxies.TextToSpeechProxy.post.say("Bring your right hand towards your left ear.");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART1);
            Proxies.TextToSpeechProxy.say("");
            Proxies.TextToSpeechProxy.say("While moving your right hand you also need to move your left arm forward.");

            // Second step.
            Proxies.TextToSpeechProxy.say("Then more your right hand forward in a sweeping motion, while bringing your left hand next to your upper body.");
            //Proxies.TextToSpeechProxy.say("Here we go!");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART2);

            // Third step.
          //  Proxies.TextToSpeechProxy.say("There is something important about your right hand.");
            Proxies.TextToSpeechProxy.post.say("Make your to rotate your right hand during this motion.");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART3);

        }

        private void explainLeftArm(Proxies Proxies)
        {
            Proxies.TextToSpeechProxy.post.say("Bring your left hand slightly forward");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART1_LEFT_ARM);
            Proxies.TextToSpeechProxy.post.say("Then bring your left hand next to your upper body");

            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART2_LEFT_ARM);
            Proxies.TextToSpeechProxy.say("It is that simple!");
        }

        private void explainRightArm(Proxies Proxies)
        {
            // First step.
            Proxies.TextToSpeechProxy.post.say("Bring your right hand towards your left ear");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART1_RIGHT_ARM);

            // Second step.
            Proxies.TextToSpeechProxy.say("Now more your right hand forward in a sweeping motion");
          //  Proxies.TextToSpeechProxy.say("Here we go!");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART2_RIGHT_ARM);

            // Third step.
           // Proxies.TextToSpeechProxy.say("There is something important about your right hand");
            Proxies.TextToSpeechProxy.say("Make your to rotate your right hand while doing this");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART3);
        }

        public override void giveFeedbackToUser(Proxies Proxies)
        {
            Proxies.TextToSpeechProxy.say("Pay attention to your right hand, remember that you need to rotate it during the performance");
        }
    }
}
