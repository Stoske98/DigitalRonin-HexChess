using Riptide;

public class NetMatchCreated : NetMessage
{
    public int match_id { get; set; }
    public string ip_address { get; set; }
    public int port { get; set; }

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
        _message.AddInt(match_id);
        _message.AddString(ip_address);
        _message.AddInt(port);
    }
    public override void Deserialize(Message _message)
    {
    }
    public override void ReceivedOnServer(Connection connection)
    {
    }
}
