using Riptide;

public class NetAttack : NetMessage
{
    public string attacker_id { get; set; }
    public int attacker_col { get; set; }
    public int attacker_row { get; set; }
    public string target_id { get; set; }
    public int target_col { get; set; }
    public int target_row { get; set; }
    public NetAttack()
    {
        op_code = OpCode.ON_ATTACK;
    }
    public NetAttack(Message _message)
    {
        op_code = OpCode.ON_ATTACK;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddString(attacker_id);
        _message.AddInt(attacker_col);
        _message.AddInt(attacker_row);

        _message.AddString(target_id);
        _message.AddInt(target_col);
        _message.AddInt(target_row);
    }
    public override void Deserialize(Message _message)
    {
        attacker_id = _message.GetString();
        attacker_col = _message.GetInt();
        attacker_row = _message.GetInt();

        target_id = _message.GetString();
        target_col = _message.GetInt();
        target_row = _message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_ATTACK_RESPONESS?.Invoke(this);
    }
}
