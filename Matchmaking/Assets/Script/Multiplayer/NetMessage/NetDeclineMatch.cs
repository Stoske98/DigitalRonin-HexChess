using Riptide;

public class NetDeclineMatch : NetMessage
{
    public ClassType class_type { get; set; }
    public NetDeclineMatch()
    {
        op_code = OpCode.ON_DECLINE_MATCH;
    }
    public NetDeclineMatch(Message _message)
    {
        op_code = OpCode.ON_DECLINE_MATCH;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt((int)class_type);
    }
    public override void Deserialize(Message _message)
    {
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_DECLINE_MATCH_REQUEST?.Invoke(this, connection);
    }
}
