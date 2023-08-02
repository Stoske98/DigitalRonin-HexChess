using Riptide;

public class NetWelcome : NetMessage
{
    public NetWelcome()
    {
        op_code = OpCode.ON_WELCOME;
    }
    public NetWelcome(Message _message)
    {
        op_code = OpCode.ON_WELCOME;
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

