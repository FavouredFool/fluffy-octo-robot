using Unity.Netcode;
using System;

public struct SerializedNetworkHex : INetworkSerializable, IEquatable<SerializedNetworkHex>
{
    public int X;
    public int Z;
    public int Height;
    public bool PlayerActive;

    public SerializedNetworkHex(int x, int z, int height, bool playerActive)
    {
        X = x;
        Z = z;
        Height = height;
        PlayerActive = playerActive;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref X);
        serializer.SerializeValue(ref Z);
        serializer.SerializeValue(ref Height);
        serializer.SerializeValue(ref PlayerActive);
    }

    public bool Equals(SerializedNetworkHex other)
    {
        return X == other.X && Height == other.Height && Z == other.Z && PlayerActive == other.PlayerActive;
    }
}