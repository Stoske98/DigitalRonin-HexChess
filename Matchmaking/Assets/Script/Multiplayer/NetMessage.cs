using Riptide;

public enum OpCode
{
    ON_KEEP_ALIVE = 0,
    ON_WELCOME = 1,
    ON_AUTH = 2,

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