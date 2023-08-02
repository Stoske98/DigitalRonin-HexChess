using Riptide;

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
