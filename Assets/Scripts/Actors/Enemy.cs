using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class Enemy : MonoBehaviour
{
    public Actor Target;
    public bool IsFighting = false;
    private AStar algorithm;

    private void Start()
    {
        GameManager.Get.AddEnemy(GetComponent<Actor>());
        algorithm = GetComponent<AStar>();
    }

    public void MoveAlongPath(Vector3Int targetPosition)
    {
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(transform.position);
        Vector2 direction = algorithm.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        Action.MoveOrHit(GetComponent<Actor>(), direction);
    }

    public void RunAI()
    {
        if (Target == null)
        {
            Target = GameManager.Get.Player;
        }

        Vector3 targetPosition = Target.transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget < 1.5f)
        {
            // If the distance is less than 1.5 units, perform the attack
            Action.Hit(GetComponent<Actor>(), Target);
            IsFighting = true; // Assuming the enemy is fighting when attacking
        }
        else
        {
            // Continue moving towards the target
            var gridPosition = MapManager.Get.FloorMap.WorldToCell(targetPosition);
            MoveAlongPath(gridPosition);
        }
    }
}
