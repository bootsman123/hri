using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao
{
    public class SkeletonManager
    {
        private KinectSensor kinectSensor;
        private Canvas canvas;

        public SkeletonManager(KinectSensor kinectSensor, Canvas canvas)
        {
            this.kinectSensor = kinectSensor;
            this.canvas = canvas;
        }

        private void DrawSkeletonJoints()
        {
        }

        private void DrawSkeletonPosition()
        {
        }

        public void Draw(SkeletonFrame skeletonFrame)
        {
            foreach (Skeleton skeleton in skeletonFrame)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    //skeleton.J
                    skeleton.Joints;
                }
                else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                {
                    skeleton.Position;
                }
            }
        }
    }

}
