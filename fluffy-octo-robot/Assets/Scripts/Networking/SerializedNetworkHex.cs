using Unity.Netcode;
using System;

public struct SerializedNetworkHex : INetworkSerializable, IEquatable<SerializedNetworkHex>
{
    public int X;
    public int Z;
    public int Height;

    public SerializedNetworkHex(int x, int z, int height)
    {
        X = x;
        Z = z;
        Height = height;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref X);
        serializer.SerializeValue(ref Z);
        serializer.SerializeValue(ref Height);
    }

    public bool Equals(SerializedNetworkHex other)
    {
        return X == other.X && Height == other.Height && Z == other.Z;
    }
}