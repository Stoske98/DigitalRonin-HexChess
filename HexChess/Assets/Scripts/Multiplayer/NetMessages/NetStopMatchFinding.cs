using Riptide;

public class NetStopMatchFinding : NetMessage
{
    public NetStopMatchFinding()
    {
        op_code = OpCode.ON_STOP_MATCH_FINDING;
    }
    public NetStopMatchFinding(Message _message)
    {
        op_code = OpCode.ON_STOP_MATCH_FINDING;
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
        NetworkManager.C_ON_STOP_MATCH_FINDING_RESPONESS?.Invoke(this);
    }
}
