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
    ON_SINGLE_TARGET_ABILITY = 6,
    ON_MULTIPLE_TARGETS_ABILITY = 7,
    ON_INSTANT_ABILITY = 8,
    ON_END_TURN = 9,
    ON_TRAP_ABILITY = 10,
    ON_UPGRADE_CLASS = 11,
    ON_SYNC_LOST_FRAGMENT = 12,
    ON_DISCONNECT = 13,
    ON_RECONNECT = 14,
    ON_END_GAME = 15,
}
