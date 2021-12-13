using Unity.Netcode;
using Unity.Collections;

public class PlayerHud : NetworkBehaviour {
    private NetworkVariable<NetworkString> playersName = new NetworkVariable<NetworkString>();

}

public struct NetworkString: INetworkSerializable {
    private FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T: IReaderWriter {
        serializer.SerializeValue(ref info);
    }

    public override string ToString() {
        return info.ToString();
    }
}
