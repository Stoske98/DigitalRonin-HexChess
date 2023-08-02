using System;
using System.Collections.Generic;
using UnityEngine;

public static class Spawner
{
    public static void SpawnUnit(Game _game, UnitType _unit_type, ClassType _class_type, Hex _hex)
    {
        Unit _unit = null;
        Stats stats = null;
        List<AbilityBehaviour> _abilities = null;
        switch (_unit_type)
        {
            case UnitType.SWORDSMAN:
                stats = new Stats()
                {
                    max_health = 1,
                    current_health = 1,
                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f,
                };

                if (_class_type == ClassType.LIGHT)
                    _abilities = new List<AbilityBehaviour>() { };
                else
                    _abilities = new List<AbilityBehaviour>() { };

                _unit = new Unit(_class_type, _unit_type, stats);

                _unit.behaviours.Add(new NormalMovement(_unit));
                _unit.behaviours.Add(new MeleeAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.white;
                break;
            case UnitType.KNIGHT:

                stats = new Stats()
                {
                    max_health = 3,
                    current_health = 3,
                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };

                _unit = new Unit(_class_type, _unit_type, stats);


                KnightPassive knight_passive = new KnightPassive(_unit, new AbilityData() { amount = 1 });
                if (_class_type == ClassType.LIGHT)
                {
                    Joust joust = new Joust(_unit, new AbilityData() { range = 2, amount = 1, max_cooldown = 3, current_cooldown = 3 });
                    _abilities = new List<AbilityBehaviour>() { knight_passive, joust };
                }
                else
                {
                    Warstrike warstrike = new Warstrike(_unit, new AbilityData() { range = 2, amount = 1, max_cooldown = 3, current_cooldown = 3 });
                    _abilities = new List<AbilityBehaviour>() { knight_passive, warstrike };
                }

                _unit.behaviours.Add(new KnightMovement(_unit));
                _unit.behaviours.Add(new MeleeAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.green;
                break;
            case UnitType.TANK:

                stats = new Stats()
                {
                    max_health = 4,
                    current_health = 4,
                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };

                _unit = new Unit(_class_type, _unit_type, stats);


                if (_class_type == ClassType.LIGHT)
                {
                    Earthshaker earthshaker = new Earthshaker(_unit, new AbilityData() { range = 1, amount = 1, max_cooldown = 2, current_cooldown = 2 });
                    _abilities = new List<AbilityBehaviour>() { earthshaker };
                }
                else
                {
                    Fear fear = new Fear(_unit, new AbilityData() { range = 1, amount = 1, max_cooldown = 2, current_cooldown = 2 });
                    _abilities = new List<AbilityBehaviour>() { fear };
                }

                _unit.behaviours.Add(new DirectionMovement(_unit, 2));
                _unit.behaviours.Add(new MeleeAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.black;
                break;
            case UnitType.ARCHER:

                stats = new Stats()
                {
                    max_health = 2,
                    current_health = 2,
                    damage = 1,
                    attack_range = 2,
                    attack_speed = 0.25f
                };

                _unit = new Unit(_class_type, _unit_type, stats);

                if (_class_type == ClassType.LIGHT)
                {
                    Powershoot powershoot = new Powershoot(_unit, new AbilityData() { amount = 1, max_cooldown = 2, current_cooldown = 2 });
                    _abilities = new List<AbilityBehaviour>() { powershoot };
                }
                else
                {
                    TrapAbility trap = new TrapAbility(_unit, new AbilityData() { range = 2, amount = 1, max_cooldown = 3, current_cooldown = 3 });
                    _abilities = new List<AbilityBehaviour>() { trap };
                }

                _unit.behaviours.Add(new NormalMovement(_unit));
                _unit.behaviours.Add(new RangedAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.cyan;
                break;
            case UnitType.WIZARD:

                stats = new Stats()
                {
                    max_health = 3,
                    current_health = 3,
                    damage = 0,
                    attack_range = 0,
                    attack_speed = 0.0f
                };

                _unit = new Unit(_class_type, _unit_type, stats);

                WizzardPassive wizard_passive = new WizzardPassive(_unit, new AbilityData() { range = 2 });
                if (_class_type == ClassType.LIGHT)
                {
                    Blessing bless = new Blessing(_unit, new AbilityData() { range = 2, amount = 1, max_cooldown = 2, current_cooldown = 2 });
                    Skyfall skyfall = new Skyfall(_unit, new AbilityData() { range = 2, amount = 1, max_cooldown = 4, current_cooldown = 4 });
                    FireBall fireball = new FireBall(_unit, new AbilityData() { amount = 1, max_cooldown = 6, current_cooldown = 6 });

                    _abilities = new List<AbilityBehaviour>() { wizard_passive, bless, skyfall, fireball };
                }
                else
                {
                    Necromancy necromancy = new Necromancy(_unit, new AbilityData() { range = 2, amount = 1, max_cooldown = 2, current_cooldown = 2 });
                    Curse curse = new Curse(_unit, new AbilityData() { range = 2, amount = 1, max_cooldown = 4, current_cooldown = 4 });
                    // Vampirism vampirism = new Vampirism(new AbilityData() { quantity = 1 });

                    _abilities = new List<AbilityBehaviour>() { wizard_passive, necromancy, curse };
                }

                _unit.behaviours.Add(new TeleportMovement(_unit, wizard_passive.ability_data.range));
                _unit.behaviours.Add(new NoAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case UnitType.JESTER:

                stats = new Stats()
                {
                    max_health = 3,
                    current_health = 3,
                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };

                _unit = new Unit(_class_type, _unit_type, stats);

                JesterPassive jester_passive = new JesterPassive(_unit, new AbilityData() { range = 2 });
                jester_passive.illusions = new List<Unit>() { CreateIllusion(_unit, _game.match_id), CreateIllusion(_unit, _game.match_id) };

                foreach (var illusion in jester_passive.illusions)
                {
                    _game.objects.Add(illusion);
                    _game.units.Add(illusion);
                }

                if (_class_type == ClassType.LIGHT)
                {
                    _abilities = new List<AbilityBehaviour>() { jester_passive };
                }
                else
                {
                    _abilities = new List<AbilityBehaviour>() { jester_passive };
                }

                _unit.behaviours.Add(new NormalMovement(_unit, 2));
                _unit.behaviours.Add(new MeleeAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.red;
                break;
            case UnitType.QUEEN:

                stats = new Stats()
                {
                    max_health = 3,
                    current_health = 3,
                    damage = 2,
                    attack_range = 1,
                    attack_speed = 0.25f
                };

                _unit = new Unit(_class_type, _unit_type, stats);

                QueenSpecial queen_special = new QueenSpecial(_unit, new AbilityData() { range = 3, });
                if (_class_type == ClassType.LIGHT)
                {
                    _abilities = new List<AbilityBehaviour>() { queen_special };
                }
                else
                {
                    _abilities = new List<AbilityBehaviour>() { queen_special };
                }

                _unit.behaviours.Add(new DirectionMovement(_unit, 3));
                _unit.behaviours.Add(new MeleeAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case UnitType.KING:
                stats = new Stats()
                {
                    max_health = 5,
                    current_health = 5,
                    damage = 0,
                    attack_range = 0,
                    attack_speed = 0.0f
                };

                _unit = new Unit(_class_type, _unit_type, stats);

                KingPassive king_passive = new KingPassive(_unit, new AbilityData());
                if (_class_type == ClassType.LIGHT)
                    _abilities = new List<AbilityBehaviour>() { king_passive };
                else
                    _abilities = new List<AbilityBehaviour>() { king_passive };

                _unit.behaviours.Add(new NormalMovement(_unit));
                _unit.behaviours.Add(new NoAttack(_unit));

                foreach (var ability in _abilities)
                    _unit.behaviours.Add(ability);

                _unit.game_object.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            default:
                break;
        }

        if (_game != null && _unit != null && _hex != null)
        {
            _unit.match_id = _game.match_id;
            _unit.RegisterEvents();
            if(_game.PlaceObject(_unit, _hex))
            {
                _game.units.Add(_unit);
                _game.objects.Add(_unit);
            }
            _unit.game_object.transform.SetParent(MapContainer.Instance.units_container);
        }
    }

    public static void CreateUnitGameObjects(Unit _unit)
    {
        {
            string name = _unit.game_object.name;
            Vector3 position = _unit.game_object.transform.position;
            Quaternion rotation = _unit.game_object.transform.rotation;

            GameObject.Destroy(_unit.game_object);
            #region CreateSphereGameObject
            _unit.game_object = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _unit.game_object.GetComponent<Collider>().enabled = false;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (_unit.class_type == ClassType.LIGHT)
                go.GetComponent<Renderer>().material.color = Color.white;
            else
                go.GetComponent<Renderer>().material.color = Color.black;
            go.transform.SetParent(_unit.game_object.transform);

            go.GetComponent<Collider>().enabled = false;
            go.transform.localScale = new Vector3(1.25f, 1, 1.25f);

            _unit.game_object.name = name;
            _unit.game_object.transform.position = position;
            _unit.game_object.transform.rotation = rotation;
            _unit.game_object.transform.SetParent(MapContainer.Instance.units_container);
            #endregion

            switch (_unit.unit_type)
            {
                case UnitType.SWORDSMAN:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.white;
                    break;
                case UnitType.KNIGHT:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.green;
                    break;
                case UnitType.TANK:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.black;
                    break;
                case UnitType.ARCHER:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.cyan;
                    break;
                case UnitType.WIZARD:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                case UnitType.JESTER:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.red;
                    break;
                case UnitType.QUEEN:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.magenta;
                    break;
                case UnitType.KING:
                    _unit.game_object.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                default:
                    break;
            }

        }
    }

    private static Unit CreateIllusion(Unit _unit_parent, int match_id)
    {
        Unit illusion;
        if (_unit_parent.class_type == ClassType.LIGHT)
        {
            illusion = new Unit(_unit_parent.class_type, _unit_parent.unit_type, UnitStatsToIllusion(_unit_parent));
            illusion.match_id = match_id;
            illusion.behaviours.Add(new NormalMovement(illusion, 2));
            illusion.behaviours.Add(new NoAttack(illusion));
            illusion.behaviours.Add(new TheTricsOfTrade(illusion, new AbilityData() { range = 1, amount = 1 }));
        }
        else
        {
            illusion = new Unit(_unit_parent.class_type, _unit_parent.unit_type, UnitStatsToIllusion(_unit_parent));
            illusion.match_id = match_id;
            illusion.behaviours.Add(new NormalMovement(illusion, 2));
            illusion.behaviours.Add(new NoAttack(illusion));
            illusion.behaviours.Add(new TheFool(illusion, new AbilityData() { range = 1, amount = 1 }));
        }

        illusion.RegisterEvents();
        illusion.game_object.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.red;
        illusion.game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
        illusion.game_object.name += "Illusion: " + _unit_parent.class_type.ToString();
        illusion.game_object.transform.SetParent(MapContainer.Instance.units_container);
        illusion.game_object.GetComponent<UnityEngine.Collider>().enabled = false;

        return illusion;
    }

    private static Stats UnitStatsToIllusion(Unit _unit)
    {
        return new Stats
        {
            max_health = _unit.stats.max_health,
            current_health = _unit.stats.current_health,
            damage = _unit.stats.damage
        };
    }
}


