using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicTimeWarping;
using Microsoft.Kinect;
using Kinect.Toolbox;

namespace KungFuNao.Tools
{
    public class SkeletonDistance : IDistance<Skeleton>
    {
        /// <summary>
        /// Returns the distance between two skeletons.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Distance(Skeleton a, Skeleton b)
        {
            // Determine the centroid for both skeletons.
            var centroidA = this.SkeletonCentroid(a);
            var centroidB = this.SkeletonCentroid(b);

            // Determine the distance from the centroid and the joints.
            var distancesA = this.SkeletonDistances(centroidA, a);
            var distancesB = this.SkeletonDistances(centroidB, b);
            var totalDistance = 0.0;

            for (var n = 0; n < distancesA.Count; n++)
            {
                totalDistance += Math.Abs(distancesA[n] - distancesB[n]);
            }

            return totalDistance;
        }

        /// <summary>
        /// Return the centroid of a skeleton.
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private Vector3 SkeletonCentroid(Skeleton skeleton)
        {
            var shoulderCenter = Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.ShoulderCenter].Position);
            var spine = Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.Spine].Position);

            return (shoulderCenter - spine) * 0.5f;
        }

        /// <summary>
        /// Return a list of distances between the centroid of a skeleton and a list of joints.
        /// </summary>
        /// <param name="centroid"></param>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private List<float> SkeletonDistances(Vector3 centroid, Skeleton skeleton)
        {
            var distance = new List<float>();
            distance.Add(Vector3.Dot(centroid, Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.ShoulderCenter].Position)));
            distance.Add(Vector3.Dot(centroid, Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.ShoulderLeft].Position)));
            distance.Add(Vector3.Dot(centroid, Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.ShoulderRight].Position)));
            distance.Add(Vector3.Dot(centroid, Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.ElbowLeft].Position)));
            distance.Add(Vector3.Dot(centroid, Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.ElbowRight].Position)));
            distance.Add(Vector3.Dot(centroid, Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.HandLeft].Position)));
            distance.Add(Vector3.Dot(centroid, Kinect.Toolbox.Tools.ToVector3(skeleton.Joints[JointType.HandRight].Position)));

            return distance;
        }
    }
}
