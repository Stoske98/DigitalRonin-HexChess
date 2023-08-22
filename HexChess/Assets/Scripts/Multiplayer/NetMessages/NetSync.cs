using Riptide;

public class NetSync : NetMessage
{
    public int match_id { get; set; }
    public int fragment_index { get; set; }
    public int total_fragments { get; set; }
    public byte[] game_data { get; set; }
    public NetSync()
    {
        op_code = OpCode.ON_SYNC;
    }
    public NetSync(Message _message)
    {
        op_code = OpCode.ON_SYNC;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt(match_id);
    }
    public override void Deserialize(Message _message)
    {
        game_data = _message.GetBytes();
        total_fragments = _message.GetInt();
        fragment_index = _message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_SYNC_RESPONESS?.Invoke(this);
    }
}