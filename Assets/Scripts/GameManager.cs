using Items;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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

    public static GameManager Get { get => instance; }

    public Actor Player;
    public List<Actor> Enemies = new List<Actor>();
    public List<Consumable> Items = new List<Consumable>();

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }

    public void StartEnemyTurn()
    {
        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<Enemy>().RunAI();
        }
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player.transform.position == location)
        {
            return Player;
        }
        else
        {
            foreach (Actor enemy in Enemies)
            {
                if (enemy.transform.position == location)
                {
                    return enemy;
                }
            }
        }
        return null;
    }

    // New Methods for Consumable Items
    public void AddItem(Consumable item)
    {
        Items.Add(item);
    }

    public void RemoveItem(Consumable item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            Destroy(item.gameObject);
        }
    }

    public GameObject CreateItem(string itemName, Vector2 position)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>($"Prefabs/{itemName}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        item.name = itemName;
        return item;
    }


    public Consumable GetItemAtLocation(Vector3 location)
    {
        foreach (Consumable item in Items)
        {
            if (item.transform.position == location)
            {
                return item;
            }
        }
        return null;
    }
}
