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
    }
    public override void Deserialize(Message _message)
    {
        class_type = (ClassType)_message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_ACCEPT_MATCH_RESPONESS?.Invoke(this);
    }
}
