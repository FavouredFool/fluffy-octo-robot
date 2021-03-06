using Unity.Netcode;
using System;
using static HexCell;

public struct SerializedNetworkHex : INetworkSerializable, IEquatable<SerializedNetworkHex>
{
    public int X;
    public int Z;
    public int Height;
    public bool PlayerActive;
    public int RoundsTillCorrupted;
    public Biome Biome;
    public bool ShowCollectable;

    public SerializedNetworkHex(int x, int z, int height, bool playerActive, int roundsTillCorrupted, Biome biome, bool showCollectable)
    {
        X = x;
        Z = z;
        Height = height;
        PlayerActive = playerActive;
        RoundsTillCorrupted = roundsTillCorrupted;
        Biome = biome;
        ShowCollectable = showCollectable;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref X);
        serializer.SerializeValue(ref Z);
        serializer.SerializeValue(ref Height);
        serializer.SerializeValue(ref PlayerActive);
        serializer.SerializeValue(ref RoundsTillCorrupted);
        serializer.SerializeValue(ref Biome);
        serializer.SerializeValue(ref ShowCollectable);
    }

    public bool Equals(SerializedNetworkHex other)
    {
        return X == other.X && Height == other.Height && Z == other.Z;
    }
}