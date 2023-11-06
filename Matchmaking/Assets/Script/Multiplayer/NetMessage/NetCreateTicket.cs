using Riptide;

public class NetCreateTicket : NetMessage
{
    public ClassType class_type { get; set; }
    public NetCreateTicket()
    {
        op_code = OpCode.ON_CREATE_TICKET;
    }
    public NetCreateTicket(Message _message)
    {
        op_code = OpCode.ON_CREATE_TICKET;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt((int)class_type);
    }
    public override void Deserialize(Message _message)
    {
        class_type = (ClassType)_message.GetInt();
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_CREATE_TICKET_REQUEST?.Invoke(this, connection);
    }
}

public class NetCreateLoby : NetMessage
{
    public string loby_id { get; set; }
    public ClassType class_type { get; set; }
    public NetCreateLoby()
    {
        op_code = OpCode.ON_CREATE_LOBY;
    }
    public NetCreateLoby(Message _message)
    {
        op_code = OpCode.ON_CREATE_LOBY;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt((int)class_type);
        _message.AddString(loby_id);
    }
    public override void Deserialize(Message _message)
    {
        class_type = (ClassType)_message.GetInt();
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_CREATE_LOBY_REQUEST?.Invoke(this, connection);
    }
}

public class NetJoinLoby : NetMessage
{
    public string loby_id { get; set; }
    public ClassType class_type { get; set; }
    public NetJoinLoby()
    {
        op_code = OpCode.ON_JOIN_LOBY;
    }
    public NetJoinLoby(Message _message)
    {
        op_code = OpCode.ON_JOIN_LOBY;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt((int)class_type);
        _message.AddString(loby_id);
    }
    public override void Deserialize(Message _message)
    {
        loby_id = _message.GetString();
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_JOIN_LOBY_REQUEST?.Invoke(this, connection);
    }
}
