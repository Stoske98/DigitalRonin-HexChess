using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Missile : IActiveObject
{
    public string id { get; set; }
    public string game_object_path { get; set; }
    public Visibility visibility { get; set; }
    public ClassType class_type { get; set; }
    [JsonConverter(typeof(CustomConverters.GameObjectConverter))] public GameObject game_object { get; set; }
    private Damage damage { get; set; }
    private Unit target { get; set; }
    private float speed { get; set; }
    private Queue<Vector3> trajectory_positions { get; set; }
    private Vector3 next_desired_position { get; set; }
    private Vector3 direction { get; set; }
    private bool work { get; set; }
    public Missile() { }
    public Missile(Unit _enemy, Damage _damage, float _angle, float _speed)
    {
        id = NetworkManager.Instance.games[_damage.unit.match_id].random_seeds_generator.GetRandomIdsSeed();
        class_type = _damage.unit.class_type;
        game_object = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        game_object.GetComponent<Renderer>().material.color = Color.red;
        game_object.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        game_object.transform.position = _damage.unit.game_object.transform.position + new Vector3(0, 1.5f, 0);
        next_desired_position = game_object.transform.position;
        trajectory_positions = CalculateTrajectoryPositions(game_object.transform.position, _enemy.game_object.transform.position + new Vector3(0, 1.5f, 0), _angle);

        target = _enemy;
        damage = _damage;
        speed = _speed;
        work = true;
    }
    public void Update()
    {
        if (trajectory_positions.Count > 0)
        {
            if (Vector3.Distance(game_object.transform.position, next_desired_position) < 0.1f)
                next_desired_position = trajectory_positions.Dequeue();

            direction = (next_desired_position - game_object.transform.position).normalized;
            game_object.transform.Translate(direction * speed * Time.deltaTime);
        }
        else
        {
            work = false;
            target.ReceiveDamage(damage);
            NetworkManager.Instance.games[damage.unit.match_id].object_manager.RemoveObject(this);
            Object.Destroy(game_object);
        }
    }
    public bool IsWork()
    {
        return work;
    }

    private Queue<Vector3> CalculateTrajectoryPositions(Vector3 start_position, Vector3 target_position, float angle)
    {
        Queue<Vector3> trajectory_positions = new Queue<Vector3>();
        float missile_speed = 4;
        float factor = 5;
        float distance = Vector3.Distance(start_position, target_position);
        float height = (distance * distance) / (2 * missile_speed * missile_speed) * Mathf.Tan(angle * Mathf.Deg2Rad);

        int lineSegmentCount = Mathf.CeilToInt(distance * factor);

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float progress = i / (float)lineSegmentCount;
            Vector3 position = Vector3.Lerp(start_position, target_position, progress);

            position.y += Mathf.Sin(progress * Mathf.PI) * height;

            trajectory_positions.Enqueue(position);
        }
        return trajectory_positions;
    }
}
