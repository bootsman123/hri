using Aldebaran.Proxies;
using KungFuNao.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models.Nao
{
    public class LeftHandPunchScene : Scene
    {
        public LeftHandPunchScene(string name, string fileName, Score score)
            : base(name, fileName, score)
        {
        }

        public override void performDefault(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_LEFT_HAND_CONTINUOUS_PUNCH);
        }

        public override void explainToUser(TextToSpeechProxy tts, BehaviorManagerProxy bproxy, KinectSpeechRecognition speech)
        {
            this.NumberOfTimesExplained++;

            tts.say("This is the " + this.Name);
            tts.post.say("Just move your left hand forward");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_LEFT_HAND_PUNCH_FORWARD);
            tts.post.say("And then move it backward");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_LEFT_HAND_PUNCH_BACKWARD);
        }

        public override void giveFeedbackToUser(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {
            tts.say("Pay attention to your left hand, you need to stretch it out completely");
        }
    }
}
