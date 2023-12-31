﻿using Riptide;

public class NetUpgradeClass : NetMessage
{
    public ClassType class_type;
    public UnitType unit_type_to_upgrade;
    public NetUpgradeClass()
    {
        op_code = OpCode.ON_UPGRADE_CLASS;
    }
    public NetUpgradeClass(Message _message)
    {
        op_code = OpCode.ON_UPGRADE_CLASS;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddInt((int)class_type);
        _message.AddInt((int)unit_type_to_upgrade);
    }
    public override void Deserialize(Message _message)
    {
        class_type = (ClassType)_message.GetInt();
        unit_type_to_upgrade = (UnitType)_message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_UPGRADE_CLASS_RESPONESS?.Invoke(this);
    }
}