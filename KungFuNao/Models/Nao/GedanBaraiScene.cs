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

        public override void performDefault(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI);
        }

        public override void explainToUser(TextToSpeechProxy tts, BehaviorManagerProxy bproxy, KinectSpeechRecognition speech)
        {
            this.NumberOfTimesExplained++;

            tts.post.say("This motion is called " + this.Name + ", it is used to block an incoming kick");
            this.performDefault(tts, bproxy);
            tts.say("I will break it down for you in three steps");

            explanation(tts, bproxy);

            if (NumberOfTimesExplained > 1)
            {
                tts.say("I already explained this motion.");
                tts.post.say("I see you do not yet really get this motion, let me explain both arms seperately");
                bproxy.runBehavior("naos-life-channel/stand_scratchHead1");
                explainLeftArm(tts, bproxy);
                explainRightArm(tts, bproxy);
                tts.say("By combining these motions you get this.");
                this.performDefault(tts, bproxy);

                tts.say("Do you understand this motion now?");
                string answer = speech.WaitForChoice(KinectSpeechRecognition.CHOICES_POSITIVE_NEGATIVE);
                switch (answer)
                {
                    case "YES":
                        tts.say("Alright, then let's continue");
                        break;
                    case "NO":
                        tts.say("Too bad, are you having trouble with your left or your right arm?");

                        string answer2 = speech.WaitForChoice(KinectSpeechRecognition.CHOICES_LEFT_RIGHT);
                        switch (answer2)
                        {
                            case "LEFT":
                                explainLeftArm(tts, bproxy);
                                break;
                            case "RIGHT":
                                explainRightArm(tts, bproxy);
                                break;
                        }
                        break;
                }
            }

        }

        private void explanation(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {

            //first step
            tts.post.say("Bring your right hand towards your left ear.");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART1);
            tts.say("");
            tts.say("While moving your right hand you also need to move your left arm forward.");
            //second step
            tts.say("Now more your right hand forward in a sweeping motion, while bringing your left hand next to your upper body.");
            tts.say("Here we go!");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART2);
            //third step
            tts.say("There is something important about your right hand.");
            tts.post.say("Make your to rotate your right hand during this motion.");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART3);
        }
        private void explainLeftArm(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {
            tts.post.say("Bring your left hand slightly forward");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART1_LEFT_ARM);
            tts.post.say("Then bring your left hand next to your upper body");

            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART2_LEFT_ARM);
            tts.say("It is that simple!");

        }

        private void explainRightArm(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {

            //first step
            tts.post.say("Bring your right hand towards your left ear");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART1_RIGHT_ARM);
            //second step
            tts.say("Now more your right hand forward in a sweeping motion");
            tts.say("Here we go!");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART2_RIGHT_ARM);
            //third step
            tts.say("There is something important about your right hand");
            tts.say("Make your to rotate your right hand while doing this");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_GEDAN_BARAI_PART3);
        }
        public override void giveFeedbackToUser(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {
            tts.say("Pay attention to your right hand, remember that you need to rotate it during the performance");
        }
    }
}
