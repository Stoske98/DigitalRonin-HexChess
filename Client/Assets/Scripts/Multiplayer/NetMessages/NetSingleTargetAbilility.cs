using Riptide;
using System.Collections.Generic;
using UnityEngine;

public class NetSingleTargetAbilility : NetMessage
{
    public string unit_id { get; set; }
    public int col { get; set; }
    public int row { get; set; }
    public int desired_col { get; set; }
    public int desired_row { get; set; }
    public KeyCode key_code { get; set; }
    public NetSingleTargetAbilility()
    {
        op_code = OpCode.ON_SINGLE_TARGET_ABILITY;
    }
    public NetSingleTargetAbilility(Message _message)
    {
        op_code = OpCode.ON_SINGLE_TARGET_ABILITY;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddString(unit_id);
        _message.AddInt(col);
        _message.AddInt(row);
        _message.AddInt(desired_col);
        _message.AddInt(desired_row);
        _message.AddInt((int)key_code);
    }
    public override void Deserialize(Message _message)
    {
        unit_id = _message.GetString();
        col = _message.GetInt();
        row = _message.GetInt();
        desired_col = _message.GetInt();
        desired_row = _message.GetInt();
        key_code = (KeyCode)_message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_SINGLE_TARGET_ABILITY_RESPONESS?.Invoke(this);
    }
}

public class NetMultipeTargetsAbilility : NetMessage
{
    public string unit_id { get; set; }
    public int col { get; set; }
    public int row { get; set; }
    private int hexes_count { get; set; }
    public List<Vector2Int> hexes_coordiantes { get; set; }
    public KeyCode key_code { get; set; }
    public NetMultipeTargetsAbilility()
    {
        op_code = OpCode.ON_MULTIPLE_TARGETS_ABILITY;
    }
    public NetMultipeTargetsAbilility(Message _message)
    {
        op_code = OpCode.ON_MULTIPLE_TARGETS_ABILITY;
        Deserialize(_message);
    }
    public override void Serialize(Message _message)
    {
        _message.AddString(unit_id);
        _message.AddInt(col);
        _message.AddInt(row);
        _message.AddInt(hexes_coordiantes.Count);

        foreach (var coordinate in hexes_coordiantes)
        {
            _message.AddInt(coordinate.x);
            _message.AddInt(coordinate.y);
        }
        _message.AddInt((int)key_code);
    }
    public override void Deserialize(Message _message)
    {
        unit_id = _message.GetString();
        col = _message.GetInt();
        row = _message.GetInt();
        hexes_count = _message.GetInt();

        hexes_coordiantes = new List<Vector2Int>();
        for (int i = 0; i < hexes_count; i++)
            hexes_coordiantes.Add(new Vector2Int(_message.GetInt(), _message.GetInt()));

        key_code = (KeyCode)_message.GetInt();
    }
    public override void ReceivedOnClient()
    {
        NetworkManager.C_ON_MULTIPLE_TARGETS_ABILITY_RESPONESS?.Invoke(this);
    }
}
