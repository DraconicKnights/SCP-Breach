using UnityEngine;

namespace SCP_Breach.Utils.Data;

public class SerializableVector3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public SerializableVector3() { }

    public SerializableVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static implicit operator Vector3(SerializableVector3 sv) =>
        new Vector3(sv.X, sv.Y, sv.Z);

    public static implicit operator SerializableVector3(Vector3 v) =>
        new SerializableVector3(v.x, v.y, v.z);
}