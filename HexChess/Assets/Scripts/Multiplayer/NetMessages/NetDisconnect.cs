using Riptide;

public class NetDisconnect : NetMessage
{
    public NetDisconnect()
    {
        op_code = OpCode.ON_DISCONNECT;
    }
    public NetDisconnect(Message _message)
    {
        op_code = OpCode.ON_DISCONNECT;
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
        NetworkManager.C_ON_DISCONNECT_RESPONESS?.Invoke(this);
    }
}
