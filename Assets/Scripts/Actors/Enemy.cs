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
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance < 1.5f)
        {
            Action.Hit(GetComponent<Actor>(), Target);
        }
        else
        {
            Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(targetPosition);
            MoveAlongPath(gridPosition);
        }
    }
}
