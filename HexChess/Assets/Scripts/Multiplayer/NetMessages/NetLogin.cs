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
        NetworkManager.C_ON_LOGIN_RESPONESS?.Invoke(this);
    }
}

public class NetEndGame : NetMessage
{
    public ClassType winner { get; set; }
    public string ip_address { get; set; }
    public int port { get; set; }
    public NetEndGame()
    {
        op_code = OpCode.ON_END_GAME;
    }
    public NetEndGame(Message _message)
    {
        op_code = OpCode.ON_END_GAME;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
    }
    public override void Deserialize(Message _message)
    {
        winner = (ClassType)_message.GetInt();
        ip_address = _message.GetString();
        port = _message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_END_GAME_RESPONESS?.Invoke(this);
    }
}
