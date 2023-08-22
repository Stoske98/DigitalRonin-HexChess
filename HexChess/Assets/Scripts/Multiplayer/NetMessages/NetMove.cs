using Riptide;

public class NetMove : NetMessage
{
    public string unit_id { get; set; }
    public int col { get; set; }
    public int row { get; set; }
    public int desired_col { get; set; }
    public int desired_row { get; set; }
    public NetMove()
    {
        op_code = OpCode.ON_MOVE;
    }
    public NetMove(Message _message)
    {
        op_code = OpCode.ON_MOVE;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddString(unit_id);
        _message.AddInt(col);
        _message.AddInt(row);
        _message.AddInt(desired_col);
        _message.AddInt(desired_row);
    }
    public override void Deserialize(Message _message)
    {
        unit_id = _message.GetString();
        col = _message.GetInt();
        row = _message.GetInt();
        desired_col = _message.GetInt();
        desired_row = _message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_MOVE_RESPONESS?.Invoke(this);
    }
}

