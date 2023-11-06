using Riptide;

public class NetSync : NetMessage
{
    public int match_id { get; set; }
    public int fragment_index { get; set; }
    public int total_fragments { get; set; }
    public int game_data_lenght { get; set; }
    public byte[] fragment_data { get; set; }
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
        fragment_data = _message.GetBytes();
        total_fragments = _message.GetInt();
        fragment_index = _message.GetInt();
        game_data_lenght = _message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_SYNC_RESPONESS?.Invoke(this);
    }
}

public class NetSyncLostFragment : NetMessage
{
    public int match_id { get; set; }
    public int fragment_index { get; set; }
    public NetSyncLostFragment()
    {
        op_code = OpCode.ON_SYNC_LOST_FRAGMENT;
    }
    public NetSyncLostFragment(Message _message)
    {
        op_code = OpCode.ON_SYNC_LOST_FRAGMENT;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt(match_id);
        _message.AddInt(fragment_index);
    }
    public override void Deserialize(Message _message)
    {
    }
    public override void ReceivedOnClient()
    {
    }
}