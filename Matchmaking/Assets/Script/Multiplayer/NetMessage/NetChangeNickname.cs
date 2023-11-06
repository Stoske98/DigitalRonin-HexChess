using Riptide;

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
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_CHANGE_NICKNAME_REQUEST?.Invoke(this, connection);
    }
}
