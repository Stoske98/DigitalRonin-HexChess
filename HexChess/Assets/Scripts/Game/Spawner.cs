using System.Collections.Generic;
using UnityEngine;

public static class Spawner
{
    //SWORDSMEN LIGHT & DARK
    private static Unit SpawnLightSwordsman(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Swordsman/Light/prefab";
        string sprite_path = "UI/Unit/Swordsman/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    new SwordsmanSpecial(unit, new AbilityData(), "UI/Unit/Swordsman/Special/sprite",Direction.UP)
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
                    }, "UI/Unit/Swordsman/Light/dash")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkSwordsman(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Swordsman/Dark/prefab";
        string sprite_path = "UI/Unit/Swordsman/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    new SwordsmanSpecial(unit, new AbilityData(),"UI/Unit/Swordsman/Special/sprite", Direction.DOWN)
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
                    }, "UI/Unit/Swordsman/Dark/blink")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //ARCHER LIGHT & DARK
    private static Unit SpawnLightArcher(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Archer/Light/prefab";
        string sprite_path = "UI/Unit/Archer/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    new ArcherSpecial(unit, new AbilityData(), "UI/Unit/Archer/Special/sprite")
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
                    new Hunt(unit, new AbilityData() { amount = 25 }, "UI/Unit/Archer/Light/hunt")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkArcher(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Archer/Dark/prefab";
        string sprite_path = "UI/Unit/Archer/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    new ArcherSpecial(unit, new AbilityData(), "UI/Unit/Archer/Special/sprite")
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
                    }, "UI/Unit/Archer/Dark/trap"),
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
                     }, "UI/Unit/Archer/Dark/trap")}
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //TANK LIGHT & DARK
    private static Unit SpawnLightTank(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Tank/Light/prefab";
        string sprite_path = "UI/Unit/Tank/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/Tank/Special/attack_stance")
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
                    }, "UI/Unit/Tank/Light/earthshaker")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkTank(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Tank/Dark/prefab";
        string sprite_path = "UI/Unit/Tank/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/Tank/Special/attack_stance")
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
                    }, "UI/Unit/Tank/Dark/fear")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //KNIGHT LIGHT & DARK
    private static Unit SpawnLightKnight(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Knight/Light/prefab";
        string sprite_path = "UI/Unit/Knight/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    },"UI/Unit/Knight/Special/sprite")
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
                    }, "UI/Unit/Knight/Light/joust")
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


        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkKnight(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Knight/Dark/prefab";
        string sprite_path = "UI/Unit/Knight/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    },"UI/Unit/Knight/Special/sprite")
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
                    }, "UI/Unit/Knight/Dark/warstrike")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //WIZARD LIGHT & DARK
    private static Unit SpawnLightWizard(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Wizard/Light/prefab";
        string sprite_path = "UI/Unit/Wizard/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/Wizard/sprite"),
                    new Blessing(unit, new AbilityData()
                    {
                        range = 2, amount = 1, max_cooldown = 2, current_cooldown = 0
                    }, "UI/Unit/Wizard/Light/blessing")
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
                    }, "UI/Unit/Wizard/Light/skyfall")
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
                    }, "UI/Unit/Wizard/Light/fireball")
                },
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkWizard(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Wizard/Dark/prefab";
        string sprite_path = "UI/Unit/Wizard/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/Wizard/sprite"),
                    new Necromancy(unit, new AbilityData()
                    {
                         range = 2, amount = 1, max_cooldown = 2, current_cooldown = 0
                    }, "UI/Unit/Wizard/Dark/necromancy")
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
                    }, "UI/Unit/Wizard/Dark/curse")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //JESTER LIGHT & DARK
    private static Unit SpawnLightJester(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Jester/Light/prefab";
        string sprite_path = "UI/Unit/Jester/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    new JesterSpecial(unit, new AbilityData() { range = 2 }, "UI/Unit/Jester/Special/sprite")
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
                    new TheTricksOfTradeFake(unit, new AbilityData(),"UI/Unit/Jester/Light/the_tricks_of_trade"),
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
                        KeyCode.S, new JesterSpecialFinal(unit, new AbilityData(){range = 2}, "UI/Unit/Jester/Special/sprite")
                    }
                }
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkJester(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Jester/Dark/prefab";
        string sprite_path = "UI/Unit/Jester/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    new JesterSpecial(unit, new AbilityData() { range = 2 }, "UI/Unit/Jester/Special/sprite")
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
                    }, "UI/Unit/Jester/Dark/the_fool"),
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
                        KeyCode.S, new JesterSpecialFinal(unit, new AbilityData(){range = 2},  "UI/Unit/Jester/Special/sprite")
                    },
                    {
                        KeyCode.Q, new TheFakeFoolFinal(unit, new AbilityData(){ range = 1, amount = 1}, "UI/Unit/Jester/Dark/the_fool")
                    }
                }
            }
        };

        unit.AddLevels(levels);
        unit.LevelUp();

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    //QUEEN LIGHT & DARK
    private static Unit SpawnLightQueen(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Queen/Light/prefab";
        string sprite_path = "UI/Unit/Queen/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/Queen/Special/sprite"),
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
                    }, "UI/Unit/Queen/Light/queen_command"),
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkQueen(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/Queen/Dark/prefab";
        string sprite_path = "UI/Unit/Queen/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/Queen/Special/sprite"),
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
                    }, "UI/Unit/Queen/Dark/haunt"),
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }

    //KING LIGHT & DARK
    private static Unit SpawnLightKing(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/King/Light/prefab";
        string sprite_path = "UI/Unit/King/Light/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/King/Special/sprite")
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

        if (_game.map.PlaceObject(unit, _hex.coordinates.x, _hex.coordinates.y))
            _game.object_manager.AddObject(unit);

        return unit;
    }
    private static Unit SpawnDarkKing(Game _game, Hex _hex, ClassType _class_type, UnitType _unit_type)
    {
        string game_object_path = "Prefabs/King/Dark/prefab";
        string sprite_path = "UI/Unit/King/Dark/sprite";
        Unit unit = new Unit(_class_type, _unit_type, game_object_path, sprite_path);

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
                    }, "UI/Unit/King/Special/sprite")
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
        string game_object_path = "Prefabs/Jester/Light/prefab";
        string sprite_path = "UI/Unit/Jester/Light/sprite";
        Unit illusion = new Unit(_unit_parent.class_type, _unit_parent.unit_type, game_object_path, sprite_path);

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
                    new JesterFakeSpecial(illusion, new AbilityData() { range = 2}, "UI/Unit/Jester/Special/sprite")
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
                    new TheTricsOfTrade(illusion, new AbilityData(), "UI/Unit/Jester/Light/the_tricks_of_trade")
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
                        KeyCode.S, new JesterFakeSpecialFinal(illusion, new AbilityData(){range = 2}, "UI/Unit/Jester/Special/sprite")
                    }
                }
            }
        };

        illusion.AddLevels(levels);
        illusion.LevelUp();

        illusion.game_object.transform.position = new Vector3(-999, -999, -999);
        illusion.game_object.name += "Illusion: " + _unit_parent.class_type.ToString();

        _game.object_manager.AddObject(illusion);

        return illusion;
    }
    public static Unit CreateDarkJesterIllusion(Game _game, Unit _unit_parent)
    {
        string game_object_path = "Prefabs/Jester/Dark/prefab";
        string sprite_path = "UI/Unit/Jester/Dark/sprite";
        Unit illusion = new Unit( _unit_parent.class_type, _unit_parent.unit_type, game_object_path, sprite_path);

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
                    new JesterFakeSpecial(illusion, new AbilityData() { range = 2}, "UI/Unit/Jester/Special/sprite")
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
                    }, "UI/Unit/Jester/Dark/the_fool")
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
                        KeyCode.S, new JesterFakeSpecialFinal(illusion, new AbilityData(){range = 2}, "UI/Unit/Jester/Special/sprite")
                    },
                    {
                        KeyCode.Q, new TheFoolFinal(illusion, new AbilityData(){ range = 1, amount = 1}, "UI/Unit/Jester/Dark/the_fool")
                    }
                }
            }
        };

        illusion.AddLevels(levels);
        illusion.LevelUp();

        illusion.game_object.transform.position = new Vector3(-999, -999, -999);
        illusion.game_object.name += "Illusion: " + _unit_parent.class_type.ToString();

        _game.object_manager.AddObject(illusion);

        return illusion;
    }
    public static Unit CreateStone(Game _game, ClassType class_type)
    {
        string game_object_path = "Prefabs/Stone/" + class_type.ToString() + "/prefab";
        string sprite_path = "UI/Unit/Stone/" + class_type.ToString() + "/sprite";
        Unit stone = new Unit(class_type, UnitType.Stone, game_object_path, sprite_path);

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

        stone.game_object.transform.position = new Vector3(-999, -999, -999);
        stone.game_object.name += "Stone: " + class_type.ToString();

        _game.object_manager.AddObject(stone);

        return stone;
    }

    public static Trap CreateTrap(Unit _cast_unit, AbilityBehaviour _ability)
    {
        string _game_object_path = "Prefabs/Trap/prefab";
        return new Trap(_cast_unit, _ability, _game_object_path);
    }
}

