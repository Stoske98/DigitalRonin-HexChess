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
        _message.AddString(enemy_data.nickname);
        _message.AddInt(enemy_data.rank);
        _message.AddInt((int)enemy_data.class_type);
    }
    public override void Deserialize(Message _message)
    {
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_FIND_MATCH_REQUEST?.Invoke(this, connection);
    }
}
