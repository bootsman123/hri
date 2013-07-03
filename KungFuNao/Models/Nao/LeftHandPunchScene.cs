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
    public class LeftHandPunchScene : Scene
    {
        public LeftHandPunchScene(string fileName, Score score)
            : base("Left Hand Punch", fileName, score)
        {
        }

        public override void PerformDefault(Proxies Proxies)
        {
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_LEFT_HAND_CONTINUOUS_PUNCH);
        }

        public override void Explain(Proxies Proxies)
        {
            this.NumberOfTimesExplained++;

            Proxies.TextToSpeechProxy.post.say("Move your left hand forward.");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_LEFT_HAND_PUNCH_FORWARD);
            Proxies.TextToSpeechProxy.post.say("And then move it backward.");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_LEFT_HAND_PUNCH_BACKWARD);
        }

        public override void GiveFeedback(Proxies Proxies)
        {
            Proxies.TextToSpeechProxy.say("Pay attention to your left hand, you need to stretch it out completely.");
        }
    }
}
