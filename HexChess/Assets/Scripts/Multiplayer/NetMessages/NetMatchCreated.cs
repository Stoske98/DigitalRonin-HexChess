using Riptide;

public class NetMatchCreated : NetMessage
{
    public int match_id { get; set; }
    public string ip_address { get; set; }
    public ushort port { get; set; }

    public NetMatchCreated()
    {
        op_code = OpCode.ON_MATCH_CREATED;
    }
    public NetMatchCreated(Message _message)
    {
        op_code = OpCode.ON_MATCH_CREATED;
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
        NetworkManager.C_ON_MATCH_CREATED_RESPONESS?.Invoke(this);
    }
}
