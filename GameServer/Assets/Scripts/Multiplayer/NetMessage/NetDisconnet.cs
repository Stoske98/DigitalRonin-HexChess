using Riptide;

public class NetDisconnet : NetMessage
{
    public NetDisconnet()
    {
        op_code = OpCode.ON_DISCONNECT;
    }
    public NetDisconnet(Message _message)
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
    public override void ReceivedOnServer(Connection connection)
    {
    }
}
