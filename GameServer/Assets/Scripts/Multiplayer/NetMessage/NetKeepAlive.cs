using Riptide;
using UnityEngine;
using UnityEngine.Rendering;

public class NetKeepAlive : NetMessage
{
    public NetKeepAlive()
    {
        op_code = OpCode.ON_KEEP_ALIVE;
    }
    public NetKeepAlive(Message _message)
    {
        op_code = OpCode.ON_KEEP_ALIVE;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
    }
    public override void Deserialize(Message _message)
    {
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_KEEP_ALIVE_REQUEST?.Invoke(this, connection);
    }
}

public class NetEndGame : NetMessage
{
    public ClassType winner { get; set; }
    public string ip_address { get; set; }
    public int port { get; set; }
    public NetEndGame()
    {
        op_code = OpCode.ON_END_GAME;
    }
    public NetEndGame(Message _message)
    {
        op_code = OpCode.ON_END_GAME;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt((int)winner);
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
