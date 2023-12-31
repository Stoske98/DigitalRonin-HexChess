﻿using Riptide;

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
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_KEEP_ALIVE_RESPONESS?.Invoke(this);
    }
}
