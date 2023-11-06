using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{
    public Camera cm;
    public Transform fields_container;
    public Transform units_container;
    public Material field_material;
    public Material outer_field_material;

    Ray ray;
    Vector3 world_mouse_position;
    RaycastHit hit;
    Vector2Int rounded_vector;
    List<Vector2Int> neighbor_vectors;
    Hex closest_hex = null;
    Hex neighbor = null;
    private Map map;

    private Hex hit_hex;
    private Hex current_hex;
    public Vector2 mouse_screen_position;

    // Update is called once per frame
    void Update()
    {
        if(map != null)
            hit_hex = OnHoverGetHex(cm);
        if (hit_hex != null && current_hex != hit_hex)
            current_hex = hit_hex;
    }

    public void SetMap(Map _map)
    {
        if(NetworkManager.Instance.player.data.class_type == ClassType.Dark)
        {
            cm.transform.position = new Vector3(cm.transform.position.x, cm.transform.position.y, cm.transform.position.z * -1);
            cm.transform.eulerAngles = new Vector3(cm.transform.localEulerAngles.x, 180, cm.transform.localEulerAngles.z);
        }
        map = _map;
    }

    public Hex GetCurrentHex()
    {
        return current_hex;
    }
    private Hex OnHoverGetHex(Camera camera)
    {
        ray = camera.ScreenPointToRay(mouse_screen_position);
        world_mouse_position = Vector3.zero;

        if (Physics.Raycast(ray, out hit))
            world_mouse_position = hit.point;
        else return null;

        rounded_vector = GetRoundedVector(world_mouse_position.x, world_mouse_position.z);

        neighbor_vectors = new List<Vector2Int>()
        {
            rounded_vector,
            rounded_vector + map.neighbors_vectors[0],
            rounded_vector + map.neighbors_vectors[1],
            rounded_vector + map.neighbors_vectors[2],
            rounded_vector + map.neighbors_vectors[3],
            rounded_vector + map.neighbors_vectors[4],
            rounded_vector + map.neighbors_vectors[5],
        };

        closest_hex = null;
        neighbor = null;

        foreach (Vector2Int nv2 in neighbor_vectors)
        {
            foreach (Hex hex in map.hexes)
            {
                if (hex.coordinates.x == nv2.x && hex.coordinates.y == nv2.y)
                {
                    neighbor = hex;
                    if (closest_hex == null)
                        closest_hex = neighbor;
                    else
                    if (Vector2.Distance(new Vector2(neighbor.game_object.transform.position.x, neighbor.game_object.transform.position.z), new Vector2(world_mouse_position.x, world_mouse_position.z)) <
                                    Vector2.Distance(new Vector2(closest_hex.game_object.transform.position.x, closest_hex.game_object.transform.position.z), new Vector2(world_mouse_position.x, world_mouse_position.z)))
                        closest_hex = neighbor;
                    break;
                }
            }
        }
        return closest_hex;
    }

    private Vector2Int GetRoundedVector(float x, float y)
    {
        float _x = (2.0f / 3 * x) / (map.GetHexHeight() * map.GetHexOffset());
        float _y = (-1.0f / 3 * x + Mathf.Sqrt(3) / 3 * y) / (map.GetHexHeight() * map.GetHexOffset());

        return Round(_x, _y);
    }

    private Vector2Int Round(float x, float y)
    {
        float s = -x - y;

        int _x = Mathf.RoundToInt(x);
        int _y = Mathf.RoundToInt(y);
        int _s = Mathf.RoundToInt(s);

        float _xDiff = Mathf.Abs(_x - x);
        float _yDiff = Mathf.Abs(_y - y);
        float _sDiff = Mathf.Abs(_s - s);

        if (_xDiff > _yDiff && _xDiff > _sDiff)
            _x = -_y - _s;
        else if (_yDiff > _sDiff)
            _y = -_x - _s;
        else
            _s = -_x - _y;
        return new Vector2Int(_x, _y);
    }

    public void MarkAbilityMoves(AbilityBehaviour _ability, Hex _unit_hex)
    {
        if (_ability is TargetableAbility targetable)
            foreach (var hex in targetable.GetAbilityMoves(_unit_hex))
                hex.SetColor(Color.yellow);
        else if(_ability is InstantleAbility instant)
            foreach (var hex in instant.GetAbilityMoves(_unit_hex))
                hex.SetColor(Color.yellow);
    }

    public void MarkAvailableMoves(MovementBehaviour _movement, Hex _unit_hex)
    {
        foreach (Hex hex in _movement.GetAvailableMoves(_unit_hex))
            hex.SetColor(Color.cyan);
    }
    public void MarkAttackMoves(AttackBehaviour _attack, Hex _unit_hex)
    {
        foreach (Hex hex in _attack.GetAttackMoves(_unit_hex))
            hex.SetColor(Color.red);
    }

    public void ResetFields()
    {
        foreach (Hex hex in map.hexes)
            hex.ResetColor();
    }

    public void MarkMovementAndAttackFields(Game game, Unit selected_unit, Hex selected_hex)
    {
        if (!Stun.IsStuned(selected_unit) && selected_unit.class_type == game.class_on_turn)
        {
            MovementBehaviour movement_behaviour = selected_unit.GetBehaviour<MovementBehaviour>();
            AttackBehaviour attack_behaviour = selected_unit.GetBehaviour<AttackBehaviour>();

            if (movement_behaviour != null)
                MarkAvailableMoves(movement_behaviour, selected_hex);
            if (!Disarm.IsDissarmed(selected_unit) && attack_behaviour != null)
                MarkAttackMoves(attack_behaviour, selected_hex);
        }
    }

    public void MarkSelectedHex(Hex selected_hex)
    {
        selected_hex.SetColor(Color.green);
    }
}
