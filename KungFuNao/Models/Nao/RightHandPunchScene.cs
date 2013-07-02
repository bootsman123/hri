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
    public class RightHandPunchScene : Scene
    {
        public RightHandPunchScene(string fileName, Score score )
            : base("Right Hand Punch", fileName, score )
        {
        }

        public override void performDefault(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_RIGHT_HAND_CONTINUOUS_PUNCH);
        }

        public override void explainToUser(TextToSpeechProxy tts, BehaviorManagerProxy bproxy, KinectSpeechRecognition speech)
        {
            this.NumberOfTimesExplained++;

            tts.say("This motion is called the " + this.Name);
            //First step
            tts.post.say("You move your right hand forward");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_RIGHT_HAND_PUNCH_FORWARD);
            //Second step 
            tts.post.say("And then move your right hand back");
            bproxy.runBehavior(NaoBehaviors.BEHAVIOR_RIGHT_HAND_PUNCH_BACKWARD);
        }

        public override void giveFeedbackToUser(TextToSpeechProxy tts, BehaviorManagerProxy bproxy)
        {
            tts.say("Pay attention to your right hand, you need to stretch it out completely");
        }
    }
}
