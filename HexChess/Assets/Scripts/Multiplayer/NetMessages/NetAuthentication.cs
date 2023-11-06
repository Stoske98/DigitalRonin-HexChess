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
        _message.AddString(device_id);
    }
    public override void Deserialize(Message _message)
    {
        player_data = new PlayerData()
        {
            nickname = _message.GetString(),
            rank = _message.GetInt(),
            class_type = (ClassType)_message.GetInt()
        };
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_AUTH_RESPONESS?.Invoke(this);
    }
}

public class NetChangeNickname : NetMessage
{
    public string nickname { get; set; }
    public NetChangeNickname()
    {
        op_code = OpCode.ON_CHANGE_NICKNAME;
    }
    public NetChangeNickname(Message _message)
    {
        op_code = OpCode.ON_CHANGE_NICKNAME;
        Deserialize(_message);
    }

    public override void Serialize(Message _message)
    {
        _message.AddString(nickname);
    }
    public override void Deserialize(Message _message)
    {
        nickname = _message.GetString();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_CHANGE_NICKNAME_RESPONESS?.Invoke(this);
    }
}
