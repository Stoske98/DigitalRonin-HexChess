using Riptide;

public class NetReconnect : NetMessage
{
    public NetReconnect()
    {
        op_code = OpCode.ON_RECONNECT;
    }
    public NetReconnect(Message _message)
    {
        op_code = OpCode.ON_RECONNECT;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
    }
    public override void Deserialize(Message _message)
    {
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_RECONNECT_RESPONESS?.Invoke(this);
    }
}
