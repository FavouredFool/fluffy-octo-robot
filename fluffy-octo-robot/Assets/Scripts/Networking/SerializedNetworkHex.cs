using Unity.Netcode;
using System;

public struct SerializedNetworkHex : INetworkSerializable, IEquatable<SerializedNetworkHex>
{
    public int X;
    public int Y;
    public int Z;

    public SerializedNetworkHex(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref X);
        serializer.SerializeValue(ref Y);
        serializer.SerializeValue(ref Z);
    }

    public bool Equals(SerializedNetworkHex other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }
}