using Newtonsoft.Json;

public interface ITargetableSingleHex
{
    [JsonIgnore] public Hex targetable_hex { get; set; }
    public void SetAbility(Hex _targetable_hex)
    {
        targetable_hex = _targetable_hex;
    }
}


