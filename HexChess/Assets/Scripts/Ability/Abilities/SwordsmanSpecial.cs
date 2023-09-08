using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public enum Stance
{
    DEFENCE,
    ATTACK,
}
public class SwordsmanStance : InstantleAbility, ISubscribe, IUpgradable
{
    private GameObject shield { get; set; }
    [JsonRequired] private int swordsman_damage { get; set; }
    [JsonRequired] private Stance current_stance { get; set; }
    [JsonRequired] private Direction face_direction { get; set; }
    public SwordsmanStance() : base() { }
    public SwordsmanStance(Unit _unit, AbilityData _ability_data, string _sprite_path, Direction _direction) : base(_unit, _ability_data, _sprite_path)
    {
        face_direction = _direction;
        current_stance = Stance.ATTACK;

    }
    public override void Execute()
    {
        ChangeStance();
        Exit();
    }

    private void ChangeStance()
    {
        current_stance = current_stance == Stance.DEFENCE ? Stance.ATTACK : Stance.DEFENCE;
        switch (current_stance)
        {
            case Stance.ATTACK:

                unit.stats.max_health -= 1;
                if (unit.stats.current_health != 1)
                    unit.stats.current_health -= 1;
                unit.stats.damage = swordsman_damage;

                shield.GetComponentInChildren<Animator>().Play("TurnOff");
                unit.animator.SetBool("Shield", false);
                unit.AddAttackBehaviour(new MeleeAttack(unit));

                break;
            case Stance.DEFENCE:

                unit.stats.max_health += 1;
                unit.stats.current_health += 1;
                swordsman_damage = unit.stats.damage;
                unit.stats.damage = 0;

                shield.GetComponentInChildren<Animator>().Play("TurnOn");
                unit.animator.SetBool("Shield", true);
                unit.AddAttackBehaviour(new NoAttack(unit));

                break;
            default:
                break;
        }

        GameManager.Instance.game.game_events.OnChangeUnitData_Global?.Invoke(unit);
    }

    public void RegisterEvents()
    {
        if(shield == null)
        {
            if(unit.class_type == ClassType.Light)
                shield = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Swordsman/Light/Ability/Shield"));
            else
                shield = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Swordsman/Dark/Ability/Shield"));

            if(current_stance == Stance.DEFENCE)
            {
                unit.animator.SetBool("Shield", true);
                shield.GetComponentInChildren<Animator>().Play("TurnOn");
            }

            shield.transform.position = unit.game_object.transform.position;
            shield.transform.SetParent(unit.game_object.transform);
        }

        unit.events.OnStartMovement_Local += OnStartMovementCalculateNewUnitDirection;
        unit.events.OnGetAbilityMoves_Local += GetAbilityMoves;
        unit.events.OnRecieveDamage_Local += OnReceiveDamage;
        GameManager.Instance.game.game_events.OnEndMovement_Global += OnEndOfMovementGlobalUnit;
    }

    public void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovementCalculateNewUnitDirection;
        unit.events.OnGetAbilityMoves_Local -= GetAbilityMoves;
        unit.events.OnRecieveDamage_Local -= OnReceiveDamage;
        GameManager.Instance.game.game_events.OnEndMovement_Global -= OnEndOfMovementGlobalUnit;
    }

    private void OnReceiveDamage(Hex hex)
    {
        if (current_stance == Stance.DEFENCE)
            ChangeStance();
    }

    private void GetAbilityMoves(Hex hex, List<Hex> list)
    {
        if (current_stance == Stance.DEFENCE)
            list.Clear();
    }

    private void OnStartMovementCalculateNewUnitDirection(Hex start_hex, Hex desired_hex)
    {
        Map map = GameManager.Instance.game.map;

        if (map.diagonals_neighbors_vectors.Contains(new Vector2Int(desired_hex.coordinates.x - start_hex.coordinates.x, desired_hex.coordinates.y - start_hex.coordinates.y)))
            return;

        face_direction = map.CoordinatesToDirection(map.
            TransformCoordinatesToUnitCoordinates(new Vector2Int(desired_hex.coordinates.x - start_hex.coordinates.x, desired_hex.coordinates.y - start_hex.coordinates.y)));

        if (current_stance == Stance.DEFENCE)
            ChangeStance();
    }

    private void OnEndOfMovementGlobalUnit(Hex hex)
    {
        Unit global_unit = hex.GetUnit();

        if (!Stun.IsStuned(unit) && !Disarm.IsDissarmed(unit) && global_unit != null && global_unit.class_type != unit.class_type && !global_unit.IsDead())
        {
            Game game = GameManager.Instance.game;
            Hex unit_hex = game.map.GetHex(unit);
            if (unit_hex != null)
            {
                Vector2Int face_direction_coordinates = game.map.DirectionToCoordinates(face_direction);
                Vector2Int face_hex_coordinates = face_direction_coordinates + unit_hex.coordinates;

                List<Vector2Int> unit_neighbors_coordinates = new List<Vector2Int>();
                foreach (var coordinate in game.map.neighbors_vectors)
                    unit_neighbors_coordinates.Add(unit_hex.coordinates + coordinate);

                List<Vector2Int> face_hex_neigbors = new List<Vector2Int>();
                foreach (var coordinate in game.map.neighbors_vectors)
                    face_hex_neigbors.Add(face_hex_coordinates + coordinate);

                if (face_hex_coordinates == hex.coordinates || (unit_neighbors_coordinates.Contains(hex.coordinates) && face_hex_neigbors.Contains(hex.coordinates)))
                {
                    if (current_stance == Stance.DEFENCE)
                    {
                        if (global_unit.stats.current_health - swordsman_damage <= 0)
                        {
                            ChangeStance();
                            unit.Attack(global_unit);
                        }
                        else
                        {
                            unit.stats.damage = swordsman_damage;
                            unit.AddAttackBehaviour(new MeleeAttack(unit));
                            Attack attack = new Attack(unit);
                            attack.SetAttack(global_unit);
                            unit.AddBehaviourToWork(attack);
                            attack.OnEndBehaviour += OnEndOfAttack;
                        }

                    }
                    else
                        unit.Attack(global_unit);
                }
            }
        }
    }

    private void OnEndOfAttack(Behaviour behaviour)
    {
        behaviour.OnEndBehaviour -= OnEndOfAttack;

        unit.stats.damage = 0;
        unit.AddAttackBehaviour(new NoAttack(unit));
    }

    public override void SetAbility()
    {
    }

    public override List<Hex> GetAbilityMoves(Hex _unit_hex)
    {
        return new List<Hex>() { _unit_hex };
    }

    public void Upgrade()
    {
        if (current_stance == Stance.DEFENCE)
        {
            swordsman_damage += unit.stats.damage;
            unit.stats.damage = 0;
        }
    }
}
public class SwordsmanSpecial : PassiveAbility
{
    [JsonRequired] private Direction face_direction { get; set; }
    public SwordsmanSpecial() : base() { }
    public SwordsmanSpecial(Unit _unit, AbilityData _ability_data, string _sprite_path, Direction _direction) : base(_unit, _ability_data, _sprite_path) { face_direction = _direction; }
    public override void Execute()
    {
    }

    public override void RegisterEvents()
    {
        unit.events.OnStartMovement_Local += OnStartMovementCalculateNewUnitDirection;
        GameManager.Instance.game.game_events.OnEndMovement_Global += OnEndOfMovementGlobalUnit;
    }

    public override void UnregisterEvents()
    {
        unit.events.OnStartMovement_Local -= OnStartMovementCalculateNewUnitDirection;
        GameManager.Instance.game.game_events.OnEndMovement_Global -= OnEndOfMovementGlobalUnit;
    }

    private void OnStartMovementCalculateNewUnitDirection(Hex start_hex, Hex desired_hex)
    {
        Map map = GameManager.Instance.game.map;

        if (map.diagonals_neighbors_vectors.Contains(new Vector2Int(desired_hex.coordinates.x - start_hex.coordinates.x, desired_hex.coordinates.y - start_hex.coordinates.y)))
            return;

        face_direction = map.CoordinatesToDirection(map.
            TransformCoordinatesToUnitCoordinates(new Vector2Int(desired_hex.coordinates.x - start_hex.coordinates.x, desired_hex.coordinates.y - start_hex.coordinates.y)));
    }

    private void OnEndOfMovementGlobalUnit(Hex hex)
    {
        Unit global_unit = hex.GetUnit();

        if (!Stun.IsStuned(unit) && !Disarm.IsDissarmed(unit) && global_unit != null && global_unit.class_type != unit.class_type && !global_unit.IsDead())
        {
            Game game = GameManager.Instance.game;
            Hex unit_hex = game.map.GetHex(unit);
            if (unit_hex != null)
            {
                Vector2Int face_direction_coordinates = game.map.DirectionToCoordinates(face_direction);
                Vector2Int face_hex_coordinates = face_direction_coordinates + unit_hex.coordinates;

                List<Vector2Int> unit_neighbors_coordinates = new List<Vector2Int>();
                foreach (var coordinate in game.map.neighbors_vectors)
                    unit_neighbors_coordinates.Add(unit_hex.coordinates + coordinate);

                List<Vector2Int> face_hex_neigbors = new List<Vector2Int>();
                foreach (var coordinate in game.map.neighbors_vectors)
                    face_hex_neigbors.Add(face_hex_coordinates + coordinate);

                if (face_hex_coordinates == hex.coordinates || (unit_neighbors_coordinates.Contains(hex.coordinates) && face_hex_neigbors.Contains(hex.coordinates)))
                    unit.Attack(global_unit);
            }
        }
    }

}
