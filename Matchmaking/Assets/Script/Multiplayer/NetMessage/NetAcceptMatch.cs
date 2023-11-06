using Riptide;

public class NetAcceptMatch : NetMessage
{
    public ClassType class_type { get; set; }
    public NetAcceptMatch()
    {
        op_code = OpCode.ON_ACCEPT_MATCH;
    }
    public NetAcceptMatch(Message _message)
    {
        op_code = OpCode.ON_ACCEPT_MATCH;
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
        NetworkManager.S_ON_ACCEPT_MATCH_REQUEST?.Invoke(this, connection);
    }
}
