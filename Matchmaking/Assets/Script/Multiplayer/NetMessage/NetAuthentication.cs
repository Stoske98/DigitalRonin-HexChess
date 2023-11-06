using Riptide;

public class NetAuthentication : NetMessage
{
    public string device_id { get; set; }
    public PlayerData player_data { get; set; }
    public NetAuthentication()
    {
        op_code = OpCode.ON_AUTH;
    }
    public NetAuthentication(Message _message)
    {
        op_code = OpCode.ON_AUTH;
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
        device_id = _message.GetString();
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_AUTH_REQUEST?.Invoke(this, connection);
    }
}
