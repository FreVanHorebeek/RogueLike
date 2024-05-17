using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public Actor Player { get; set; }

    public List<Actor> Enemies { get; private set; } = new List<Actor>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Player = GetComponent<Actor>();
        if (Player == null)
        {
            Debug.LogError("Player Actor component is not assigned.");
        }
    }

    public static GameManager Get { get => instance; }

    public void AddEnemy(Actor enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Attempted to add null enemy to the GameManager.");
            return;
        }

        Enemies.Add(enemy);
    }

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);

        if (actor == null)
        {
            Debug.LogError($"Prefab for {name} could not be loaded.");
            return null;
        }

        Actor actorComponent = actor.GetComponent<Actor>();
        if (actorComponent == null)
        {
            Debug.LogError("Actor component is missing on the instantiated prefab.");
            return null;
        }

        if (name == "Player")
        {
            Player = actorComponent;
        }
        else
        {
            AddEnemy(actorComponent);
        }

        actor.name = name;
        return actor;
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player != null && Player.transform.position == location)
        {
            return Player;
        }

        foreach (var enemy in Enemies)
        {
            if (enemy != null && enemy.transform.position == location)
            {
                return enemy;
            }
        }

        return null;
    }

    public void StartEnemyTurn()
    {
        foreach (var enemy in Enemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.RunAI();
            }
        }
    }

    public class Action
    {
        static private void EndTurn(Actor actor)
        {
            if (actor.GetComponent<Player>() != null)
            {
                GameManager.Get.StartEnemyTurn();
            }
        }

        static public void Move(Actor actor, Vector2 direction)
        {
            Actor target = GameManager.Get.GetActorAtLocation(actor.transform.position + (Vector3)direction);

            if (target == null)
            {
                actor.Move(direction);
                actor.UpdateFieldOfView();
            }

            EndTurn(actor);
        }
    }
}
