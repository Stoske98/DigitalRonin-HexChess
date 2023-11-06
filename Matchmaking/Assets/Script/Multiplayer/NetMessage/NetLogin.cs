using Riptide;

public class NetLogin : NetMessage
{
    public PlayerData player_data { get; set; }
    public NetLogin()
    {
        op_code = OpCode.ON_LOGIN;
    }
    public NetLogin(Message _message)
    {
        op_code = OpCode.ON_LOGIN;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddString(player_data.nickname);
        _message.AddInt(player_data.rank);
        _message.AddInt((int)player_data.class_type);
    }
    public override void Deserialize(Message _message)
    {
    }
    public override void ReceivedOnServer(Connection connection)
    {
    }
}
