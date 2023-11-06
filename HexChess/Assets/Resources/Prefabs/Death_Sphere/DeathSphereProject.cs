using RayFire;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DeathSphereProject : MonoBehaviour
{
    private Queue<Vector3> trajectory_positions { get; set; }
    private Vector3 next_desired_position { get; set; }
    private Vector3 direction { get; set; }
    public float speed { get; set; }
    public bool work = false;

    public void Prepare(Vector3 start_position, Vector3 desired_position, float _speed, float angle)
    {
        gameObject.transform.position = start_position + new Vector3(0, 1, 0);
        trajectory_positions = CalculateTrajectoryPositions(start_position + new Vector3(0, 1, 0), desired_position + new Vector3(0, 1, 0), angle, _speed);
        if(trajectory_positions.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        next_desired_position = trajectory_positions.Dequeue();
        work = true;
    }
    void Update()
    {
        if(work)
        {
            if (trajectory_positions.Count > 0)
            {
                if (Vector3.Distance(transform.position, next_desired_position) < 0.1f)
                    next_desired_position = trajectory_positions.Dequeue();

                direction = (next_desired_position - transform.position).normalized;
                transform.Translate(direction * speed * Time.deltaTime);
            }
            else
                Destroy(gameObject);
        }
    }

    private Queue<Vector3> CalculateTrajectoryPositions(Vector3 start_position, Vector3 target_position, float angle, float _speed)
    {
        Queue<Vector3> trajectory_positions = new Queue<Vector3>();
        speed = _speed;
        float missile_speed = _speed;
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
