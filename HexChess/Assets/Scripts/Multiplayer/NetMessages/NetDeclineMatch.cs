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
    }
    public override void Deserialize(Message _message)
    {
        class_type = (ClassType)_message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_DECLINE_MATCH_RESPONESS?.Invoke(this);
    }
}
