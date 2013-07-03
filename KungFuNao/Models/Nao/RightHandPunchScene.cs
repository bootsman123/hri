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

        public override void PerformDefault(Proxies Proxies)
        {
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_RIGHT_HAND_CONTINUOUS_PUNCH);
        }

        public override void Explain(Proxies Proxies)
        {
            this.NumberOfTimesExplained++;

            //Proxies.TextToSpeechProxy.say("This motion is called the " + this.Name);

            // First step.
            Proxies.TextToSpeechProxy.post.say("You move your right hand forward");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_RIGHT_HAND_PUNCH_FORWARD);

            // Second step .
            Proxies.TextToSpeechProxy.post.say("And then move your right hand back");
            Proxies.BehaviorManagerProxy.runBehavior(NaoBehaviors.BEHAVIOR_RIGHT_HAND_PUNCH_BACKWARD);
        }

        public override void GiveFeedback(Proxies Proxies)
        {
            Proxies.TextToSpeechProxy.say("Pay attention to your right hand, you need to stretch it out completely");
        }
    }
}
