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
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_STOP_MATCH_FINDING_REQUEST?.Invoke(this, connection);
    }
}
