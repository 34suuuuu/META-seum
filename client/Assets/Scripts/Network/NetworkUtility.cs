
using System;

namespace StarterAssets
{
    public class NetworkUtility
    {
        public static System.Numerics.Quaternion ChangeQuaternionPackage(UnityEngine.Quaternion quaternion)
        {
            return new System.Numerics.Quaternion(
                quaternion.x,
                quaternion.y,
                quaternion.z,
                quaternion.w
                );
        }

        public static UnityEngine.Quaternion ChangeQuaternionPackage(System.Numerics.Quaternion quaternion)
        {
            return new UnityEngine.Quaternion(
                quaternion.X,
                quaternion.Y,
                quaternion.Z,
                quaternion.W
            );
        }

        public static System.Numerics.Vector3 ChangeVector3Package(UnityEngine.Vector3 vector3)
        {
            return new System.Numerics.Vector3(
                vector3.x,
                vector3.y,
                vector3.z
            );
        }

        public static UnityEngine.Vector3 ChangeVector3Package(System.Numerics.Vector3 vector3)
        {
            return new UnityEngine.Vector3(
                vector3.X,
                vector3.Y,
                vector3.Z
            );
        }
    }
}