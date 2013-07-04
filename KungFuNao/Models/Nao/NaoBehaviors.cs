using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Models.Nao
{
    public class NaoBehaviors
    {
        public static String BEHAVIOR_WARMING_UP = "karate/warming-up";
        public static String BEHAVIOR_WELCOME = "naos-life-channel/stand_interested1";

        public static String BEHAVIOR_GEDAN_BARAI = "karate/gedan-barai";
        public static String BEHAVIOR_GEDAN_BARAI_PART1 = "karate/gedan-barai-part1";
        public static String BEHAVIOR_GEDAN_BARAI_PART1_LEFT_ARM = "karate/gedan-barai-part1-left-arm";
        public static String BEHAVIOR_GEDAN_BARAI_PART1_RIGHT_ARM = "karate/gedan-barai-part1-right-arm";
        public static String BEHAVIOR_GEDAN_BARAI_PART2 = "karate/gedan-barai-part2";
        public static String BEHAVIOR_GEDAN_BARAI_PART2_LEFT_ARM = "karate/gedan-barai-part2-left-arm";
        public static String BEHAVIOR_GEDAN_BARAI_PART2_RIGHT_ARM = "karate/gedan-barai-part2-right-arm";
        public static String BEHAVIOR_GEDAN_BARAI_PART3 = "karate/gedan-barai-part3";

        public static String BEHAVIOR_LEFT_HAND_CONTINUOUS_PUNCH = "karate/left-hand-continuous-punch";
        public static String BEHAVIOR_LEFT_HAND_PUNCH_FORWARD = "karate/left-hand-punch-forward";
        public static String BEHAVIOR_LEFT_HAND_PUNCH_BACKWARD = "karate/left-hand-punch-backward";

        public static String BEHAVIOR_RIGHT_HAND_CONTINUOUS_PUNCH = "karate/right-hand-continuous-punch";
        public static String BEHAVIOR_RIGHT_HAND_PUNCH_FORWARD = "karate/right-hand-punch-forward";
        public static String BEHAVIOR_RIGHT_HAND_PUNCH_BACKWARD = "karate/right-hand-punch-backward";

        public static String BEHAVIOR_ACT_EXCITED = "naos-life-channel/stand_excited1";
        public static String BEHAVIOR_EXPLAIN1 = "naos-life-channel/stand_lookHand1";
        public static String BEHAVIOR_EXPLAIN2 = "naos-life-channel/stand_lookHand2";
        public static String BEHAVIOR_EXPLAIN3 = "naos-life-channel/stand_annoyed1";
        public static String BEHAVIOR_EXPLAIN4 = "naos-life-channel/stand_waddle1";
        public static String BEHAVIOR_EXPLAIN5 = "naos-life-channel/stand_waddle2";
        public static String BEHAVIOR_EXPLAIN6 = "naos-life-channel/stand_scratchHand1";
        public static String BEHAVIOR_EXPLAIN7 = "naos-life-channel/stand_scratchHead1";        
        public static String BEHAVIOR_EXPLAIN8 = "naos-life-channel/stand_proud1";
        public static String BEHAVIOR_EXPLAIN9 = "naos-life-channel/stand_proud2";
        public static String BEHAVIOR_EXPLAIN10 = "naos-life-channel/stand_proud3";

        public static List<String> EXPLAINING_MOVEMENTS = new List<String> { 
            NaoBehaviors.BEHAVIOR_EXPLAIN1, 
            NaoBehaviors.BEHAVIOR_EXPLAIN2,
            NaoBehaviors.BEHAVIOR_EXPLAIN3,
            NaoBehaviors.BEHAVIOR_EXPLAIN4,
            NaoBehaviors.BEHAVIOR_EXPLAIN5,
            NaoBehaviors.BEHAVIOR_EXPLAIN6,
            NaoBehaviors.BEHAVIOR_EXPLAIN7,
            NaoBehaviors.BEHAVIOR_EXPLAIN8,
            NaoBehaviors.BEHAVIOR_EXPLAIN9,
            NaoBehaviors.BEHAVIOR_EXPLAIN10
        };
            

        public static String BEHAVIOR_SHOW_MUSCLES1 = "naos-life-channel/stand_showMuscles1";
        public static String BEHAVIOR_SHOW_MUSCLES2 = "naos-life-channel/stand_showMuscles2";
        public static String BEHAVIOR_SHOW_MUSCLES3 = "naos-life-channel/stand_showMuscles3";
        public static String BEHAVIOR_SHOW_MUSCLES4 = "karate/show-muscles";
        public static String BEHAVIOR_SHOW_MUSCLES5 = "naos-life-channel/stand_showMuscles5";
        public static String BEHAVIOR_SHOW_MUSCLES6 = "naos-life-channel/stand_kungFu1";

        public static List<String> MUSCLE_MOVEMENTS = new List<String> { 
            NaoBehaviors.BEHAVIOR_SHOW_MUSCLES1, 
            NaoBehaviors.BEHAVIOR_SHOW_MUSCLES2, 
            NaoBehaviors.BEHAVIOR_SHOW_MUSCLES3, 
          //  NaoBehaviors.BEHAVIOR_SHOW_MUSCLES4, 
            NaoBehaviors.BEHAVIOR_SHOW_MUSCLES5, 
            NaoBehaviors.BEHAVIOR_SHOW_MUSCLES6
        };

        public static String BEHAVIOR_SEE_SOMETHING6 = "naos-life-channel/stand_seeSomething6";
        public static String BEHAVIOR_SEE_SOMETHING7 = "naos-life-channel/stand_seeSomething7";
        public static String BEHAVIOR_SEE_SOMETHING8 = "naos-life-channel/stand_seeSomething8";
        


        public static String BEHAVIOR_THINK1 = "naos-life-channel/stand_think1";
        public static String BEHAVIOR_THINK2 = "naos-life-channel/stand_think2";
        public static String BEHAVIOR_THINK3 = "naos-life-channel/stand_think3";
        public static String BEHAVIOR_THINK4 = "naos-life-channel/stand_think4";

        public static List<String> THINKING_MOVEMENTS = new List<String> { 
            NaoBehaviors.BEHAVIOR_THINK1,
            NaoBehaviors.BEHAVIOR_THINK2,
            NaoBehaviors.BEHAVIOR_THINK3,
            NaoBehaviors.BEHAVIOR_THINK4
        };

        public static String GOOD_PERFORMANCE1 = "naos-life-channel/stand_proud1";

        public static String BAD_PERFORMANCE1 = "naos-life-channel/stand_mocker1";
    }
}
