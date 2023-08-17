using System.Collections.Generic;

public class ClassLevelController
{
    public int level { set; get; }
    public int max_level { set; get; }
    public UnitType unit_type { get; set; }
    public ClassType class_type { get; set; }
    public List<int> exp_value { get; set; }
    public List<int> cost_to_upgrade { get; set; }

    public ClassLevelController() { exp_value = new List<int>(); cost_to_upgrade = new List<int>(); }
    public ClassLevelController(UnitType _unit_type, ClassType _class_type, List<int> _exp_value, List<int> _cost_to_upgrade)
    {
        level = 1;
        max_level = 3;
        unit_type = _unit_type;
        class_type = _class_type;
        exp_value = _exp_value;
        cost_to_upgrade = _cost_to_upgrade;
    }
    public bool CanBeUpgraded(int _shards)
    {
        if (level < max_level && _shards >= cost_to_upgrade[level - 1])
            return true;
        return false;
    }
    public int GetExpValue()
    {
        return exp_value[level - 1];
    }
    public int GetCostToUpgrade()
    {
        return cost_to_upgrade[level - 1];
    }
}


