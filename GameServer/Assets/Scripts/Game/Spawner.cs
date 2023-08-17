using System;
using System.Collections.Generic;
using UnityEngine;

public static class Spawner
{//SWORDSMEN LIGHT & DARK
    private static Unit SpawnLightSwordsman(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit),
                    new MeleeAttack(unit),
                    new SwordsmanSpecial(unit, new AbilityData(), Direction.UP)
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new Dash(unit, new AbilityData() 
                    {
                        range = 2, amount = 1
                    })
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkSwordsman(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit),
                    new MeleeAttack(unit),
                    new SwordsmanSpecial(unit, new AbilityData(), Direction.DOWN)
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },  
                behaviours_to_add = new List<Behaviour>()
                {
                    new Blink(unit, new AbilityData()
                    {
                        range = 1, amount = 1
                    })
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //ARCHER LIGHT & DARK
    private static Unit SpawnLightArcher(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 2,
                    increase_attack_range = 2,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit),
                    new RangedAttack(unit),
                    new ArcherSpecial(unit, new AbilityData())
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {
                    new Hunt(unit, new AbilityData() { amount = 25 })
                },

            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkArcher(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 2,
                    increase_attack_range = 2,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit),
                    new RangedAttack(unit),
                    new ArcherSpecial(unit, new AbilityData())
                },

            },
             //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new TrapAbility(unit, new AbilityData()
                    {
                         range = 2, amount = 1, max_cooldown = 3, current_cooldown = 0, cc = 2
                    }),
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                 behaviour_to_switch = new Dictionary<KeyCode, Behaviour>
                {
                     {KeyCode.Q,  new TrapAbilityFinal(unit, new AbilityData()
                     {
                         range = 2, amount = 1, max_cooldown = 3, current_cooldown = 0, cc = 2
                     })}
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //TANK LIGHT & DARK
    private static Unit SpawnLightTank(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 4,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new DirectionMovement(unit, 2),
                    new MeleeAttack(unit),
                    new TankAttackStance(unit, new AbilityData()
                    {
                        range = 2, max_cooldown = 2, current_cooldown = 0, cc = 1
                    })
                },

            },
             //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {
                    new Earthshaker(unit, new AbilityData()
                    {
                        range = 1, amount = 1, max_cooldown = 2, current_cooldown = 0 , cc = 2
                    })
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkTank(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 4,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new DirectionMovement(unit, 2),
                    new MeleeAttack(unit),
                    new TankAttackStance(unit, new AbilityData()
                    {
                        range = 2, max_cooldown = 2, current_cooldown = 0, cc = 1
                    })
                },

            },
             //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },

                behaviours_to_add=new List<Behaviour>()
                {
                    new Fear(unit, new AbilityData()
                    {
                        range = 1, amount = 1, max_cooldown = 2, current_cooldown = 0
                    })
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //KNIGHT LIGHT & DARK
    private static Unit SpawnLightKnight(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new KnightMovement(unit),
                    new MeleeAttack(unit),
                    new KnightSpecial(unit, new AbilityData()
                    {
                        amount = 1
                    })
                },

            },
             //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {
                    new Joust(unit, new AbilityData()
                    {
                        range = 2, amount = 1, max_cooldown = 3, current_cooldown = 0
                    })
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);


        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkKnight(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new KnightMovement(unit),
                    new MeleeAttack(unit),
                    new KnightSpecial(unit, new AbilityData()
                    {
                        amount = 1
                    })
                },

            },
             //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {
                    new Warstrike(unit, new AbilityData()
                    {
                        range = 2, amount = 1, max_cooldown = 3, current_cooldown = 0, cc = 2
                    })
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //WIZARD LIGHT & DARK
    private static Unit SpawnLightWizard(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NoAttack(unit),
                    new TeleportMovement(unit, 2),
                    new WizardSpecial(unit, new AbilityData()
                    {
                        range = 2
                    }),
                    new Blessing(unit, new AbilityData()
                    {
                        range = 2, amount = 1, max_cooldown = 2, current_cooldown = 0
                    })
                },

            },
             //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {
                    new Skyfall(unit, new AbilityData()
                    {
                        range = 2, amount = 1, max_cooldown = 4, current_cooldown = 0
                    })
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {
                     new FireBall(unit, new AbilityData()
                    {
                       amount = 1, max_cooldown = 6, current_cooldown = 0
                    })
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkWizard(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NoAttack(unit),
                    new TeleportMovement(unit, 2),
                    new WizardSpecial(unit, new AbilityData()
                    {
                        range = 2
                    }),
                    new Necromancy(unit, new AbilityData()
                    {
                         range = 2, amount = 1, max_cooldown = 2, current_cooldown = 0
                    })
                },

            },
             //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {
                    new Curse(unit, new AbilityData()
                    {
                        range = 2, amount = 1, max_cooldown = 4, current_cooldown = 0, cc = 2
                    })
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                },

                behaviours_to_add = new List<Behaviour>()
                {

                }
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //JESTER LIGHT & DARK
    private static Unit SpawnLightJester(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);

        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit,2),
                    new MeleeAttack(unit),
                    new JesterSpecial(unit, new AbilityData() { range = 2 })
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new TheTricksOfTradeFake(unit, new AbilityData()),
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviour_to_switch = new Dictionary<KeyCode, Behaviour>()
                {
                    {
                        KeyCode.S, new JesterSpecialFinal(unit, new AbilityData(){range = 2})
                    }
                }
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkJester(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);

        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit,2),
                    new MeleeAttack(unit),
                    new JesterSpecial(unit, new AbilityData() { range = 2 })
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new TheFakeFool(unit, new AbilityData()
                    {
                        range = 1, amount = 1
                    }),
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },

                behaviour_to_switch = new Dictionary<KeyCode, Behaviour>()
                {
                    {
                        KeyCode.S, new JesterSpecialFinal(unit, new AbilityData(){range = 2})
                    },
                    {
                        KeyCode.Q, new TheFakeFoolFinal(unit, new AbilityData(){ range = 1, amount = 1})
                    }
                }
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    //QUEEN LIGHT & DARK
    private static Unit SpawnLightQueen(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 2,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new DirectionMovement(unit,3),
                    new MeleeAttack(unit),
                    new QueenSpecial(unit,new AbilityData()
                    {
                        range = 3
                    }),
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add= new List<Behaviour>()
                {
                    new QueensCommand(unit,new AbilityData()
                    {
                        range = 3
                    }),
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkQueen(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 2,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new DirectionMovement(unit,3),
                    new MeleeAttack(unit),
                    new QueenSpecial(unit,new AbilityData()
                    {
                        range = 3
                    }),
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add= new List<Behaviour>()
                {
                    new Haunt(unit,new AbilityData()
                    {
                        range = 3
                    }),
                }
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //KING LIGHT & DARK
    private static Unit SpawnLightKing(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 5
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit),
                    new NoAttack(unit),
                    new KingSpecial(unit, new AbilityData()
                    {
                        range = 1,
                    })
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkKing(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        Unit unit = new Unit(_game, _class_type, _unit_type);
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 5
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NormalMovement(unit),
                    new NoAttack(unit),
                    new KingSpecial(unit, new AbilityData()
                    {
                        range = 1,
                    })
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        unit.game_object = CreateUnitGameObjectByPath(_class_type, _unit_type);
        unit.game_object.transform.SetParent(MapContainer.Instance.units_container);

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    public static Unit SpawnUnit(Game _game, UnitType _unit_type, ClassType _class_type, Hex _hex)
    {
        switch (_unit_type)
        {
            case UnitType.Swordsman:
                if (_class_type == ClassType.Light)
                    return SpawnLightSwordsman(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkSwordsman(_game, _hex, _class_type, _unit_type);
            case UnitType.Archer:
                if (_class_type == ClassType.Light)
                    return SpawnLightArcher(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkArcher(_game, _hex, _class_type, _unit_type);
            case UnitType.Knight:
                if (_class_type == ClassType.Light)
                    return SpawnLightKnight(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkKnight(_game, _hex, _class_type, _unit_type);
            case UnitType.Tank:
                if (_class_type == ClassType.Light)
                    return SpawnLightTank(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkTank(_game, _hex, _class_type, _unit_type);
            case UnitType.Jester:
                if (_class_type == ClassType.Light)
                    return SpawnLightJester(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkJester(_game, _hex, _class_type, _unit_type);
            case UnitType.Wizard:
                if (_class_type == ClassType.Light)
                    return SpawnLightWizard(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkWizard(_game, _hex, _class_type, _unit_type);
            case UnitType.Queen:
                if (_class_type == ClassType.Light)
                    return SpawnLightQueen(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkQueen(_game, _hex, _class_type, _unit_type);
            case UnitType.King:
                if (_class_type == ClassType.Light)
                    return SpawnLightKing(_game, _hex, _class_type, _unit_type);
                else
                    return SpawnDarkKing(_game, _hex, _class_type, _unit_type);
            default:
                return null;
        }
    }

    public static Unit CreateLightJesterIllusion(Game _game, Unit _unit_parent)
    {
        Unit illusion = new Unit(_game, _unit_parent.class_type, _unit_parent.unit_type);

        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NoMovement(illusion),
                    new NoAttack(illusion),
                    new JesterFakeSpecial(illusion, new AbilityData() { range = 2})
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new TheTricsOfTrade(illusion, new AbilityData())
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviour_to_switch = new Dictionary<KeyCode, Behaviour>()
                {
                    {
                        KeyCode.S, new JesterFakeSpecialFinal(illusion, new AbilityData(){range = 2})
                    }
                }
            }
        };

        illusion.AddLevels(levels);
        illusion.LevelUp();

        illusion.game_object = CreateUnitGameObjectByPath(_unit_parent.class_type, _unit_parent.unit_type);
        illusion.game_object.transform.SetParent(MapContainer.Instance.units_container);
        illusion.game_object.transform.position = new Vector3(-999, -999, -999);
        illusion.game_object.name += "Illusion: " + _unit_parent.class_type.ToString();

        _game.object_manager.AddObject(illusion);

        return illusion;
    }
    public static Unit CreateDarkJesterIllusion(Game _game, Unit _unit_parent)
    {
        Unit illusion = new Unit(_game, _unit_parent.class_type, _unit_parent.unit_type);

        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 3,
                    increase_attack_range = 1,
                    increase_damage = 1,
                    increase_attack_speed = 0.25f,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NoMovement(illusion),
                    new NoAttack(illusion),
                    new JesterFakeSpecial(illusion, new AbilityData() { range = 2})
                },

            },
            //level 2 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new TheFool(illusion, new AbilityData()
                    {
                        range = 1, amount = 1
                    })
                },
            },
            //level 3 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                    increase_damage = 1,
                },
                behaviour_to_switch = new Dictionary<KeyCode, Behaviour>()
                {
                    {
                        KeyCode.S, new JesterFakeSpecialFinal(illusion, new AbilityData(){range = 2})
                    },
                    {
                        KeyCode.Q, new TheFoolFinal(illusion, new AbilityData(){ range = 1, amount = 1})
                    }
                }
            }
        };

        illusion.AddLevels(levels);
        illusion.LevelUp();

        illusion.game_object = CreateUnitGameObjectByPath(_unit_parent.class_type, _unit_parent.unit_type);
        illusion.game_object.transform.SetParent(MapContainer.Instance.units_container);
        illusion.game_object.transform.position = new Vector3(-999, -999, -999);
        illusion.game_object.name += "Illusion: " + _unit_parent.class_type.ToString();

        _game.object_manager.AddObject(illusion);

        return illusion;
    }
    public static Unit CreateStone(Game _game, ClassType class_type)
    {
        Unit stone = new Unit(_game, class_type, UnitType.Stone);
        stone.is_immune_to_magic = true;
        List<Level> levels = new List<Level>()
        {
            //level 1 
            new Level()
            {
                update_stats = new StatsUpdate()
                {
                    increase_max_health = 1,
                },
                behaviours_to_add = new List<Behaviour>()
                {
                    new NoMovement(stone),
                    new NoAttack(stone),
                },

            }
        };

        stone.AddLevels(levels);
        stone.LevelUp();

        stone.game_object = CreateUnitGameObjectByPath(stone.class_type, stone.unit_type);
        stone.game_object.transform.SetParent(MapContainer.Instance.units_container);
        stone.game_object.transform.position = new Vector3(-999, -999, -999);
        stone.game_object.name += "Stone: " + class_type.ToString();

        _game.object_manager.AddObject(stone);

        return stone;

    }
    public static GameObject CreateUnitGameObjectByPath(ClassType class_type, UnitType unit_type)
    {
        GameObject game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/" + unit_type.ToString() + "/" + class_type.ToString() + "/prefab"));

        game_object.name = class_type.ToString() + "_" + unit_type.ToString();

        if (class_type == ClassType.Dark)
            game_object.transform.eulerAngles = new Vector3(0, 180, 0);

        return game_object;
    }
}


