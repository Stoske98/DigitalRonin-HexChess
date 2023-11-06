using System.Collections.Generic;

public class ShardController
{
    public int light_shards { set; get; }
    public int dark_shards { set; get; }
    public List<ClassLevelController> class_level_controller { get; set; }
    public ShardController()
    {
    }
    public void InitControllers()
    {
        light_shards = 0;
        dark_shards = 0;

        class_level_controller = new List<ClassLevelController>()
        {
            new ClassLevelController(UnitType.Swordsman, ClassType.Light, new List<int> { 1, 1, 2}, new List<int> { 5, 9 }),
            new ClassLevelController(UnitType.Swordsman, ClassType.Dark, new List < int > { 1, 1, 2}, new List < int > { 5, 9}),

            new ClassLevelController(UnitType.Archer,ClassType.Light, new List<int> { 2, 2, 3 }, new List < int > { 6, 12}),
            new ClassLevelController(UnitType.Archer,ClassType.Dark, new List < int > { 2, 2, 3 }, new List < int > { 6, 12}),

            new ClassLevelController(UnitType.Knight,ClassType.Light, new List < int > { 2, 2, 3 }, new List < int > { 6, 12 }),
            new ClassLevelController(UnitType.Knight,ClassType.Dark, new List < int > { 2, 2, 3}, new List < int > { 6, 12 }),

            new ClassLevelController(UnitType.Tank,ClassType.Light, new List < int > { 3, 3, 4}, new List < int > { 7, 14 }),
            new ClassLevelController(UnitType.Tank,ClassType.Dark, new List < int > { 3, 3, 4}, new List < int > { 7, 14 }),

            new ClassLevelController(UnitType.Jester, ClassType.Light, new List < int > { 5, 5, 7 }, new List < int > { 9, 15 }),
            new ClassLevelController(UnitType.Jester, ClassType.Dark, new List < int > { 5, 5, 7 }, new List < int > { 9, 15 }),

            new ClassLevelController(UnitType.Wizard, ClassType.Light, new List < int > { 5, 5, 7 }, new List < int > { 10, 16 }),
            new ClassLevelController(UnitType.Wizard, ClassType.Dark, new List < int > { 5, 5, 7 }, new List < int > { 8, 16 }),

            new ClassLevelController(UnitType.Queen,ClassType.Light, new List < int > { 7, 9, 11 }, new List < int > { 10, 20 }),
            new ClassLevelController(UnitType.Queen,ClassType.Dark, new List < int > { 7, 9, 11 }, new List < int > { 10, 20 }),

            new ClassLevelController(UnitType.King,ClassType.Light, null, new List < int > { 9, 11 }),
            new ClassLevelController(UnitType.King,ClassType.Dark, null, new List < int > { 9, 11 }),
        };
    }
    public void IncreaseShardsOnUnitDeath(ClassType _class_type, UnitType _unit_type)
    {
        foreach (var level_controller in class_level_controller)
        {
            if (level_controller.class_type == _class_type && level_controller.unit_type == _unit_type)
            {
                if (_class_type == ClassType.Light)
                    dark_shards += level_controller.GetExpValue();
                else
                    light_shards += level_controller.GetExpValue();
            }
        }
    }
    public bool CanClassBeUpgraded(ClassType _class_type, UnitType _unit_type)
    {
        foreach (var level_controller in class_level_controller)
        {
            if (level_controller.class_type == _class_type && level_controller.unit_type == _unit_type)
            {
                if (_class_type == ClassType.Light)
                    return level_controller.CanBeUpgraded(light_shards);
                else 
                    return level_controller.CanBeUpgraded(dark_shards);
            }
        }

        return false;
    }
    public void UpgradeClass(ClassType _class_type, UnitType _unit_type, List<Unit> units)
    {
        foreach (var level_controller in class_level_controller)
        {
            if (level_controller.class_type == _class_type && level_controller.unit_type == _unit_type )
            {
                if (_class_type == ClassType.Light)
                {
                    if(level_controller.CanBeUpgraded(light_shards))
                    {
                        light_shards -= level_controller.GetCostToUpgrade();
                        level_controller.level++;

                        foreach (var unit in units)
                            if(unit.class_type == _class_type && unit.unit_type == _unit_type)
                                    unit.LevelUp();

                        UnityEngine.Debug.Log(_class_type.ToString() + "_" + _unit_type.ToString() + " Class level: " + level_controller.level + " Shards: " + light_shards);
                    }
                }
                else
                {
                    if (level_controller.CanBeUpgraded(dark_shards))
                    {
                        dark_shards -= level_controller.GetCostToUpgrade();
                        level_controller.level++;

                        foreach (var unit in units)
                            if (unit.class_type == _class_type && unit.unit_type == _unit_type)
                                unit.LevelUp();

                        UnityEngine.Debug.Log(_class_type.ToString() + "_" + _unit_type.ToString() + " Class level: " + level_controller.level + " Shards: " + dark_shards);
                    }
                }
                break;
            }
        }
    }
}


