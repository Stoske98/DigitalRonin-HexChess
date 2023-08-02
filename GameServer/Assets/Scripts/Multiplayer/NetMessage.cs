using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
public class NetMessage : IMessageSerializable
{
    public OpCode op_code { set; get; }
    public virtual void Serialize(Message message)
    {
    }
    public virtual void Deserialize(Message message)
    {
    }
    public virtual void ReceivedOnServer(Connection connection)
    {
    }
}
public enum OpCode
{
    ON_KEEP_ALIVE = 0,
    ON_WELCOME = 1,
    ON_AUTH = 2,
    ON_SYNC = 3,
    ON_ATTACK = 4,
    ON_MOVE = 5,
    ON_TARGETABLE_ABILITY = 6,
    ON_INSTANT_ABILITY = 7,
}
