using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class JesterPassive : PassiveAbility
{
    public List<Unit> illusions;
    public JesterPassive() : base() { illusions = new List<Unit>(); }
    public JesterPassive(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data)
    {
    }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {
        /*  if(illusions != null && illusions.Count > 0)
              illusions = new List<Unit>() { CreateIllusion(unit), CreateIllusion(unit) };*/

          unit.events.OnStartMovement_Local += OnStartMovement;
          unit.events.OnRecieveDamage_Local += OnParentUnitRecieveDamage;
          foreach (var illusion in illusions)
              illusion.events.OnRecieveDamage_Local += OnIllusionRecieveDamage;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovement;
        unit.events.OnRecieveDamage_Local -= OnParentUnitRecieveDamage;
        foreach (var illusion in illusions)
            illusion.events.OnRecieveDamage_Local -= OnIllusionRecieveDamage;
    }

    private void OnParentUnitRecieveDamage(Hex hex)
    {
        foreach (var illusion in illusions)
        {
            NetworkManager.Instance.games[unit.match_id].GetHex(illusion)?.RemoveUnit();
            illusion.game_object.SetActive(false);
        }
    }

    private void OnIllusionRecieveDamage(Hex _hex)
    {
        foreach (var illusion in illusions)
        {
            NetworkManager.Instance.games[unit.match_id].GetHex(illusion)?.RemoveUnit();
            illusion.game_object.SetActive(false);
        }
    }

    private void OnStartMovement(Hex from_hex, Hex _target_hex)
    {
        Behaviour behaviour = unit.GetBehaviour<MovementBehaviour>();
        if (behaviour != null && behaviour is NormalMovement)
        {
            if (from_hex != null)
            {
                List<Hex> hexes = PathFinding.PathFinder.BFS_HexesMoveRange(from_hex, ability_data.range, NetworkManager.Instance.games[unit.match_id].GetMapHexes());
                hexes.Remove(from_hex);
                hexes.Remove(_target_hex);

                List<Hex> walkable_hexes = new List<Hex>();
                foreach (Hex hex in hexes)
                    if (hex.IsWalkable())
                        walkable_hexes.Add(hex);
                if (walkable_hexes.Count != 0)
                {
                    int rand = (int)MathF.Floor(NetworkManager.Instance.games[unit.match_id].random_seeds_generator.GetRandomSeed() * walkable_hexes.Count);
                    Hex first_walkable_hex = walkable_hexes[rand];
                    walkable_hexes.RemoveAt(rand);

                    PlaceIllusion(illusions[0], unit, from_hex, first_walkable_hex);

                    if (walkable_hexes.Count != 0)
                    {
                        rand = (int)MathF.Floor(NetworkManager.Instance.games[unit.match_id].random_seeds_generator.GetRandomSeed() * walkable_hexes.Count);
                        Hex second_walkable_hex = walkable_hexes[rand];
                        walkable_hexes.RemoveAt(rand);

                        PlaceIllusion(illusions[1], unit, from_hex, second_walkable_hex);
                    }
                    else
                    {

                        NetworkManager.Instance.games[unit.match_id].GetHex(illusions[1])?.RemoveUnit();
                        illusions[1].game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
                    }
                }
                else
                {
                    NetworkManager.Instance.games[unit.match_id].GetHex(illusions[0])?.RemoveUnit();
                    illusions[0].game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);

                    NetworkManager.Instance.games[unit.match_id].GetHex(illusions[1])?.RemoveUnit();
                    illusions[1].game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
                }
            }


        }
    }

    private void PlaceIllusion(Unit _illusion, Unit _parent_unit, Hex _parent_hex, Hex _desired_hex)
    {
        NetworkManager.Instance.games[unit.match_id].GetHex(_illusion)?.RemoveUnit();
        NetworkManager.Instance.games[unit.match_id].PlaceObject(_illusion, _parent_hex);
        _illusion.stats.current_health = _parent_unit.stats.current_health;
        _illusion.Move(_parent_hex, _desired_hex);
    }


}

  /*  private void OnParentUnitRecieveDamage(Hex hex)
    {
        /*foreach (var illusion in illusions)
        {
            GameManager.Instance.game.GetHex(illusion)?.RemoveUnit();
            illusion.game_object.SetActive(false);
        }*/
    //

    /*private void OnIllusionRecieveDamage(Hex _hex)
    {
        foreach (var illusion in illusions)
        {
            illusion.game.GetHex(illusion)?.RemoveUnit();
            illusion.game_object.SetActive(false);
        }
    }*/

   /* private void OnStartMovement(Hex from_hex, Hex _target_hex)
    {
        Behaviour behaviour = unit.GetBehaviour<MovementBehaviour>();
        if (behaviour != null && behaviour is NormalMovement)
        {
            if(from_hex != null)
            {
                List<Hex> hexes = PathFinding.PathFinder.BFS_HexesMoveRange(from_hex, ability_data.range, GameManager.Instance.game.GetMapHexes());
                hexes.Remove(from_hex);
                hexes.Remove(_target_hex);

                List<Hex> walkable_hexes = new List<Hex>();
                foreach (Hex hex in hexes)
                    if (hex.IsWalkable())
                        walkable_hexes.Add(hex);
                if (walkable_hexes.Count != 0)
                {
                    Random random = new Random();

                    int rand = random.Next(walkable_hexes.Count);
                    Hex first_walkable_hex = walkable_hexes[rand];
                    walkable_hexes.RemoveAt(rand);

                    PlaceIllusion(illusions[0], unit, from_hex, first_walkable_hex);

                    if (walkable_hexes.Count != 0)
                    {
                        rand = random.Next(walkable_hexes.Count);
                        Hex second_walkable_hex = walkable_hexes[rand];
                        walkable_hexes.RemoveAt(rand);

                        PlaceIllusion(illusions[1], unit, from_hex, second_walkable_hex);
                    }
                    else
                    {

                        GameManager.Instance.game.GetHex(illusions[1])?.RemoveUnit();
                        illusions[1].game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
                    }
                }
                else
                {
                    GameManager.Instance.game.GetHex(illusions[0])?.RemoveUnit();
                    illusions[0].game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);

                    GameManager.Instance.game.GetHex(illusions[1])?.RemoveUnit();
                    illusions[1].game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
                }
            }
           

        }
    }

    private Unit CreateIllusion(Unit _unit_parent)
    {
        Unit illusion;
        if (_unit_parent.class_type == ClassType.LIGHT)
        {
            illusion = new Unit(_unit_parent.class_type, UnitStatsToIllusion(_unit_parent));
            illusion.behaviours.Add(new NormalMovement(illusion, 2));
            illusion.behaviours.Add(new NoAttack(illusion));
            illusion.behaviours.Add(new TheTricsOfTrade(illusion, new AbilityData() { range = 1, quantity = 1 }));
        }
        else
        {
            illusion = new Unit(_unit_parent.class_type, UnitStatsToIllusion(_unit_parent));
            illusion.behaviours.Add(new NormalMovement(illusion, 2));
            illusion.behaviours.Add(new NoAttack(illusion));
            illusion.behaviours.Add(new TheFool(illusion, new AbilityData() { range = 1, quantity = 1 }));
        }

        illusion.RegisterEvents();
        illusion.game_object.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.red;
        illusion.game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
        illusion.game_object.name += "Illusion: " + _unit_parent.class_type.ToString();
        illusion.game_object.transform.SetParent(GameManager.Instance.map_controller.units_container);
        illusion.game_object.GetComponent<UnityEngine.Collider>().enabled = false;

        return illusion;
    }

    private Stats UnitStatsToIllusion(Unit _unit)
    {
        return new Stats 
        {
            max_health = _unit.stats.max_health,
            current_health = _unit.stats.current_health,
            damage = _unit.stats.damage
        };
    }*/

