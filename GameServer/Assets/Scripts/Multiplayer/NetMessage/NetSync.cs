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
        _message.AddBytes(fragment_data);
        _message.AddInt(total_fragments);
        _message.AddInt(fragment_index);
        _message.AddInt(game_data_lenght);
    }
    public override void Deserialize(Message _message)
    {
        match_id = _message.GetInt();
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_SYNC_REQUEST?.Invoke(this, connection);
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
    }
    public override void Deserialize(Message _message)
    {
        match_id = _message.GetInt();
        fragment_index = _message.GetInt();
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_SYNC_LOST_FRAGMENT_REQUEST?.Invoke(this, connection);
    }
}
