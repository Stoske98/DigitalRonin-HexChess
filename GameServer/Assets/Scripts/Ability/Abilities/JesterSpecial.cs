using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class JesterSpecial : PassiveAbility
{
    public List<Unit> illusions;
    public JesterSpecial() : base() { illusions = new List<Unit>(); }
    public JesterSpecial(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data)
    {
        Game game = NetworkManager.Instance.games[unit.match_id];
        if(unit.class_type == ClassType.Light)
            illusions = new List<Unit>() { Spawner.CreateLightJesterIllusion(game, unit), Spawner.CreateLightJesterIllusion(game, unit) };
        else
            illusions = new List<Unit>() { Spawner.CreateDarkJesterIllusion(game, unit), Spawner.CreateDarkJesterIllusion(game, unit) };
    }

    public override void Execute()
    {

    }

    public override void RegisterEvents()
    {
        unit.events.OnStartMovement_Local += OnStartMovement;
        unit.events.OnRecieveDamage_Local += OnParentUnitRecieveDamage;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovement;
        unit.events.OnRecieveDamage_Local -= OnParentUnitRecieveDamage;

        Game game = NetworkManager.Instance.games[unit.match_id];

        foreach (var illusion in illusions)
        {
            game.map.GetHex(illusion)?.RemoveObject(illusion);
            game.object_manager.RemoveObject(illusion);
        }

        game.object_manager.ProcessPendingActions();

        illusions.Clear();
    }

    private void OnParentUnitRecieveDamage(Hex hex)
    {
        foreach (var illusion in illusions)
            RemoveIllusion(illusion);
    }

    private void OnStartMovement(Hex from_hex, Hex _target_hex)
    {
        Behaviour behaviour = unit.GetBehaviour<MovementBehaviour>();
        if (behaviour != null && behaviour is NormalMovement)
        {
            if (from_hex != null)
            {
                Game game = NetworkManager.Instance.games[unit.match_id];
                List<Hex> hexes = PathFinding.PathFinder.BFS_HexesMoveRange(from_hex, ability_data.range, game.map);
                hexes.Remove(from_hex);
                hexes.Remove(_target_hex);

                List<Hex> walkable_hexes = new List<Hex>();
                foreach (Hex hex in hexes)
                    if (hex.IsWalkable())
                        walkable_hexes.Add(hex);
                if (walkable_hexes.Count != 0)
                {
                    int rand = (int)MathF.Floor(game.random_seeds_generator.GetRandomPercentSeed() * walkable_hexes.Count);
                    Hex first_walkable_hex = walkable_hexes[rand];
                    walkable_hexes.RemoveAt(rand);

                    PlaceIllusion(game, illusions[0], from_hex, first_walkable_hex);

                    if (walkable_hexes.Count != 0)
                    {
                        rand = (int)MathF.Floor(game.random_seeds_generator.GetRandomPercentSeed() * walkable_hexes.Count);
                        Hex second_walkable_hex = walkable_hexes[rand];
                        walkable_hexes.RemoveAt(rand);

                        PlaceIllusion(game, illusions[1], from_hex, second_walkable_hex);
                    }
                    else
                        RemoveIllusion(illusions[1]);
                }
                else
                {
                    foreach (var _illusion in illusions)
                        RemoveIllusion(_illusion);
                }
            }
        }
    }
    private void RemoveIllusion(Unit _illusion)
    {
        NetworkManager.Instance.games[unit.match_id].map.GetHex(_illusion)?.RemoveObject(_illusion);
        _illusion.stats.current_health = 0;
        IObject.ObjectVisibility(_illusion, Visibility.NONE);
        _illusion.game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
    }
    private void PlaceIllusion(Game _game, Unit _illusion, Hex _parent_hex, Hex _desired_hex)
    {
        _game.map.GetHex(_illusion)?.RemoveObject(_illusion);

        _parent_hex.PlaceObject(_illusion);
        _illusion.game_object.transform.position = _parent_hex.game_object.transform.position;

        _illusion.stats.current_health = unit.stats.current_health;
        IObject.ObjectVisibility(_illusion, Visibility.BOTH);

        NormalMovement normal_movement = new NormalMovement(_illusion, 2);
        normal_movement.SetPath(_parent_hex, _desired_hex);
        _illusion.AddBehaviourToWork(normal_movement);
    }

}

public class JesterFakeSpecial : PassiveAbility
{
    public JesterFakeSpecial() : base() { }
    public JesterFakeSpecial(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data)
    {
    }

    public override void Execute()
    {
    }

    public override void RegisterEvents()
    {
        unit.events.OnRecieveDamage_Local += OnIllusionRecieveDamage;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnRecieveDamage_Local -= OnIllusionRecieveDamage;
    }

    private void OnIllusionRecieveDamage(Hex _hex)
    {
        NetworkManager.Instance.games[unit.match_id].map.GetHex(unit)?.RemoveObject(unit);
        unit.stats.current_health = 0;
        IObject.ObjectVisibility(unit, Visibility.NONE);
        unit.game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
    }
}

public class JesterFakeSpecialFinal : TargetableAbility, ITargetMultipleHexes, ISubscribe
{
    public int max_hexes { get; set; }
    public bool has_condition { get; set; }
    public List<Hex> targetable_hexes { get; set; }
    public JesterFakeSpecialFinal() : base()
    {
        targetable_hexes = new List<Hex>();
    }
    public JesterFakeSpecialFinal(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data)
    {
        targetable_hexes = new List<Hex>();

        max_hexes = 0;
        has_condition = true;
    }
    public override void Execute()
    {
    }
    public void RegisterEvents()
    {
        unit.events.OnRecieveDamage_Local += OnIllusionRecieveDamage;
    }

    public void UnregisterEvents()
    {
        unit.events.OnRecieveDamage_Local -= OnIllusionRecieveDamage;
    }

    private void OnIllusionRecieveDamage(Hex _hex)
    {
        NetworkManager.Instance.games[unit.match_id].map.GetHex(unit)?.RemoveObject(unit);
        unit.stats.current_health = 0;
        IObject.ObjectVisibility(unit, Visibility.NONE);
        unit.game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        return new List<Hex>();
    }

}

public class JesterSpecialFinal : TargetableAbility, ITargetMultipleHexes, ISubscribe
{
    public int max_hexes { get; set; }
    public bool has_condition { get; set; }
    public List<Hex> targetable_hexes { get; set; }
    public List<Unit> illusions { get; set; }
    public JesterSpecialFinal() : base()
    {
        targetable_hexes = new List<Hex>();
        illusions = new List<Unit>();
    }
    public JesterSpecialFinal(Unit _unit, AbilityData _ability_data) : base(_unit, _ability_data)
    {
        targetable_hexes = new List<Hex>();
        illusions = new List<Unit>();

        Game game = NetworkManager.Instance.games[unit.match_id];
        if (unit.class_type == ClassType.Light)
            illusions = new List<Unit>() { Spawner.CreateLightJesterIllusion(game, unit), Spawner.CreateLightJesterIllusion(game, unit) };
        else
            illusions = new List<Unit>() { Spawner.CreateDarkJesterIllusion(game, unit), Spawner.CreateDarkJesterIllusion(game, unit) };

        foreach (var illusion in illusions)
            illusion.AddMovementBehaviour(new NormalMovement(illusion, 2));

        max_hexes = illusions.Count + 1;
        has_condition = true;
    }

    public void RegisterEvents()
    {
        unit.events.OnRecieveDamage_Local += OnParentUnitRecieveDamage;
    }

    public void UnregisterEvents()
    {
        unit.events.OnRecieveDamage_Local -= OnParentUnitRecieveDamage;

        Game game = NetworkManager.Instance.games[unit.match_id];

        foreach (var illusion in illusions)
        {
            game.map.GetHex(illusion)?.RemoveObject(illusion);
            game.object_manager.RemoveObject(illusion);
        }

        game.object_manager.ProcessPendingActions();

        illusions.Clear();
    }
    private void OnParentUnitRecieveDamage(Hex hex)
    {
        foreach (var illusion in illusions)
            RemoveIllusion(illusion);
    }
    public override void Execute()
    {
        Game game = NetworkManager.Instance.games[unit.match_id];
        Hex hex = game.map.GetHex(unit);

        if (hex != null)
        {
            for (int i = 0; i < targetable_hexes.Count; i++)
            {
                if (i == 0)
                    unit.Move(hex, targetable_hexes[i]);
                else
                    PlaceIllusion(game, illusions[i - 1], hex, targetable_hexes[i]);
            }
        }

        targetable_hexes.Clear();
        Exit();
    }
    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        List<Hex> _available_moves = PathFinding.PathFinder.BFS_HexesMoveRange(_unit_hex, ability_data.range, NetworkManager.Instance.games[unit.match_id].map);

        if (_available_moves.Count >= illusions.Count + 1)
            max_hexes = illusions.Count + 1;
        else
            max_hexes = _available_moves.Count;

        return _available_moves;
    }
    private void RemoveIllusion(Unit _illusion)
    {
        NetworkManager.Instance.games[unit.match_id].map.GetHex(_illusion)?.RemoveObject(_illusion);
        _illusion.stats.current_health = 0;
        IObject.ObjectVisibility(_illusion, Visibility.NONE);
        _illusion.game_object.transform.position = new UnityEngine.Vector3(-999, -999, -999);
    }
    private void PlaceIllusion(Game _game, Unit _illusion, Hex _parent_hex, Hex _desired_hex)
    {
        _game.map.GetHex(_illusion)?.RemoveObject(_illusion);

        _parent_hex.PlaceObject(_illusion);
        _illusion.game_object.transform.position = _parent_hex.game_object.transform.position;

        _illusion.stats.current_health = unit.stats.current_health;
        IObject.ObjectVisibility(_illusion, Visibility.BOTH);

        NormalMovement normal_movement = new NormalMovement(_illusion, 2);
        normal_movement.SetPath(_parent_hex, _desired_hex);
        _illusion.AddBehaviourToWork(normal_movement);
    }
}