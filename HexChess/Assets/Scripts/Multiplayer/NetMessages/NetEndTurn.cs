using Riptide;

public class NetEndTurn : NetMessage
{
    public ClassType class_on_turn { get; set; }
    public NetEndTurn()
    {
        op_code = OpCode.ON_END_TURN;
    }
    public NetEndTurn(Message _message)
    {
        op_code = OpCode.ON_END_TURN;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
    }
    public override void Deserialize(Message _message)
    {
        class_on_turn = (ClassType)_message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_END_TURN_RESPONESS?.Invoke(this);
    }
}