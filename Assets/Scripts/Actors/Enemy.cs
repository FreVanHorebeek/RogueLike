using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class Enemy : MonoBehaviour
{
    public Actor Target { get; set; }
    public bool IsFighting { get; private set; } = false;
    private AStar Algorithm;

    private void Start()
    {
        Algorithm = GetComponent<AStar>();
        if (Algorithm == null)
        {
            Debug.LogError($"{gameObject.name}: AStar component is missing.");
        }

        var actor = GetComponent<Actor>();
        if (actor == null)
        {
            Debug.LogError($"{gameObject.name}: Actor component is missing.");
            return;
        }

        GameManager.Get.AddEnemy(actor);
    }

    void Update()
    {
        RunAI();
    }

    public void MoveAlongPath(Vector3Int targetPosition)
    {
        if (Algorithm == null)
        {
            Debug.LogError($"{gameObject.name}: Algorithm is not initialized.");
            return;
        }

        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(transform.position);
        Debug.Log($"{gameObject.name}: Current grid position is {gridPosition}, target position is {targetPosition}.");

        Vector2 direction = Algorithm.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        if (direction == null)
        {
            Debug.LogError($"{gameObject.name}: Algorithm.Compute returned null.");
            return;
        }

        Actor actor = GetComponent<Actor>();
        if (actor == null)
        {
            Debug.LogError($"{gameObject.name}: Actor component is missing.");
            return;
        }

        GameManager.Action.Move(actor, direction);
    }

    public void RunAI()
    {
        if (Target == null)
        {
            Target = GameManager.Get.Player;
            if (Target == null)
            {
                Debug.LogError($"{gameObject.name}: Target is null, GameManager.Get.Player is not assigned.");
                return;
            }
        }

        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(Target.transform.position);
        Debug.Log($"{gameObject.name}: Target grid position is {gridPosition}.");

        Actor actor = GetComponent<Actor>();
        if (actor == null)
        {
            Debug.LogError($"{gameObject.name}: Actor component is missing.");
            return;
        }

        if (IsFighting || actor.FieldOfView.Contains(gridPosition))
        {
            IsFighting = true;
            MoveAlongPath(gridPosition);
        }
    }
}
