using Riptide;

public class NetGameExist : NetMessage
{
    public int match_id { get; set; }
    public string ip_address { get; set; }
    public ushort port { get; set; }

    public NetGameExist()
    {
        op_code = OpCode.ON_GAME_EXIST;
    }
    public NetGameExist(Message _message)
    {
        op_code = OpCode.ON_GAME_EXIST;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
    }
    public override void Deserialize(Message _message)
    {
        match_id = _message.GetInt();
        ip_address = _message.GetString();
        port = (ushort)_message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_GAME_EXIST_RESPONESS?.Invoke(this);
    }
}
