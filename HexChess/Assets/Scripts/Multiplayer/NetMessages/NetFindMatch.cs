using Riptide;

public class NetFindMatch : NetMessage
{
    public PlayerData enemy_data { get; set; }
    public NetFindMatch()
    {
        op_code = OpCode.ON_FIND_MATCH;
    }
    public NetFindMatch(Message _message)
    {
        op_code = OpCode.ON_FIND_MATCH;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
    }
    public override void Deserialize(Message _message)
    {
        enemy_data = new PlayerData
        {
            nickname = _message.GetString(),
            rank = _message.GetInt(),
            class_type = (ClassType)_message.GetInt()
        };
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_FIND_MATCH_RESPONESS?.Invoke(this);
    }
}
