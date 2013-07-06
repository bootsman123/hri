using Kinect.Toolbox.Record;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace KungFuNao.Tools
{
    public class SkeletonRecordingConverter
    {
        public static List<Skeleton> FromStream(Stream stream)
        {
            List<Skeleton> skeletonList = new List<Skeleton>();

            // Start reading the stream.
            BinaryReader reader = new BinaryReader(stream);

            // Check if the stream contains skeletons.
            KinectRecordOptions options = (KinectRecordOptions)reader.ReadInt32();

            if ((options & KinectRecordOptions.Skeletons) == 0)
            {
                throw new System.ArgumentException("The stream does not contain skeleton frames.");
            }

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                KinectRecordOptions header = (KinectRecordOptions)reader.ReadInt32();

                if (header != KinectRecordOptions.Skeletons)
                {
                    continue;
                }

                // Unneeded information for the converter cause.
                long timeStamp = reader.ReadInt64();
                SkeletonTrackingMode trackingMode = (SkeletonTrackingMode)reader.ReadInt32();
                Tuple<float, float, float, float> floorClipPlane = new Tuple<float, float, float, float>(
                    reader.ReadSingle(), reader.ReadSingle(),
                    reader.ReadSingle(), reader.ReadSingle()
                );
                int frameNumber = reader.ReadInt32();

                BinaryFormatter formatter = new BinaryFormatter();
                Skeleton[] skeletons = (Skeleton[])formatter.Deserialize(reader.BaseStream);

                // Append all skeletons to the list.
                skeletonList.AddRange(skeletons);
            }

            return skeletonList;
        }
    }
}
