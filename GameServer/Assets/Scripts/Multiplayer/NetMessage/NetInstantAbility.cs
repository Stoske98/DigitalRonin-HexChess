using Riptide;
using UnityEngine;

public class NetInstantAbility : NetMessage
{
    public string unit_id { get; set; }
    public int col { get; set; }
    public int row { get; set; }
    public KeyCode key_code { get; set; }
    public NetInstantAbility()
    {
        op_code = OpCode.ON_INSTANT_ABILITY;
    }
    public NetInstantAbility(Message _message)
    {
        op_code = OpCode.ON_INSTANT_ABILITY;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddString(unit_id);
        _message.AddInt(col);
        _message.AddInt(row);
        _message.AddInt((int)key_code);
    }
    public override void Deserialize(Message _message)
    {
        unit_id = _message.GetString();
        col = _message.GetInt();
        row = _message.GetInt();
        key_code = (KeyCode)_message.GetInt();
    }
    public override void ReceivedOnServer(Connection connection)
    {
        NetworkManager.S_ON_INSTANT_ABILITY_REQUEST?.Invoke(this, connection);
    }
}
