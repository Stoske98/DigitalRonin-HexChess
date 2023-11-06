using Riptide;

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


    ON_LOGIN = 101,
    ON_GAME_EXIST = 102,
    ON_CHANGE_NICKNAME = 103,
    ON_CREATE_TICKET = 104,
    ON_FIND_MATCH = 105,
    ON_STOP_MATCH_FINDING = 106,
    ON_ACCEPT_MATCH = 107,
    ON_DECLINE_MATCH = 108,
    ON_MATCH_CREATED = 109,
    ON_CREATE_LOBY = 110,
    ON_JOIN_LOBY = 111,
}
public class NetMessage : IMessageSerializable
{
    public OpCode op_code { set; get; }
  
    public virtual void Serialize(Message _message)
    {
    }
    public virtual void Deserialize(Message _message)
    {
    }

    public virtual void ReceivedOnClient()
    {
    }

}
