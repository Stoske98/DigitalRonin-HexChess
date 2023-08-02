using Riptide;
using System;
using System.Diagnostics;
using UnityEditor;

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
        _message.AddBytes(game_data);
        _message.AddInt(total_fragments);
        _message.AddInt(fragment_index);
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

