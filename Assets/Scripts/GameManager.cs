using Items;
using System;
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
    public List<Ladder> Ladders = new List<Ladder>();
    public List<Tombstone> Tombstones = new List<Tombstone>(); // List to hold tombstones

    public GameObject CreateActor(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x, position.y, 0), Quaternion.identity);
        if (actor == null)
        {
            Debug.LogError($"Failed to create actor: {name}");
        }
        actor.name = name;
        return actor;
    }

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
        UIManager.Instance.UpdateEnemiesLeftText(Enemies.Count); // Update the enemies left text
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
            Destroy(enemy.gameObject);
            UIManager.Instance.UpdateEnemiesLeftText(Enemies.Count); // Update the enemies left text
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
        if (Player != null && Player.transform.position == location)
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
        GameObject item = Instantiate(Resources.Load<GameObject>($"Prefabs/{itemName}"), new Vector3(position.x, position.y, 0), Quaternion.identity);
        if (item == null)
        {
            Debug.LogError($"Failed to create item: {itemName}");
        }
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

    public List<Actor> GetNearbyEnemies(Vector3 location)
    {
        List<Actor> nearbyEnemies = new List<Actor>();
        foreach (var enemy in Enemies)
        {
            if (Vector3.Distance(enemy.transform.position, location) < 5)
            {
                nearbyEnemies.Add(enemy);
            }
        }
        return nearbyEnemies;
    }

    public void CreateLadder(Vector2 position, bool up)
    {
        GameObject ladderObject = Instantiate(Resources.Load<GameObject>("Prefabs/Ladder"), new Vector3(position.x, position.y, 0), Quaternion.identity);
        if (ladderObject != null)
        {
            Ladder ladder = ladderObject.GetComponent<Ladder>();
            if (ladder != null)
            {
                ladder.Up = up;
                Ladders.Add(ladder);
            }
            else
            {
                Debug.LogError("Failed to create ladder: Ladder component is missing.");
                Destroy(ladderObject);
            }
        }
        else
        {
            Debug.LogError("Failed to load ladder prefab.");
        }
    }

    public void AddLadder(Ladder ladder)
    {
        Ladders.Add(ladder);
    }

    public Ladder GetLadderAtLocation(Vector3 location)
    {
        foreach (Ladder ladder in Ladders)
        {
            if (ladder.transform.position == location)
            {
                return ladder;
            }
        }
        return null;
    }

    public void MoveActorToPosition(Actor actor, Vector2 position)
    {
        if (actor != null)
        {
            actor.transform.position = new Vector3(position.x, position.y, 0);
        }
        else
        {
            Debug.LogError("Failed to move actor: Actor is null.");
        }
    }

    public void AddTombStone(Tombstone stone)
    {
        Tombstones.Add(stone);
    }

    // SaveGame data structuur om de spelerinformatie op te slaan
    [Serializable]
    public struct SaveGame
    {
        public int maxHitPoints;
        public int hitPoints;
        public int defense;
        public int power;
        public int level;
        public int xp;
        public int xpToNextLevel;
    }

    private SaveGame playerSaveData; // Variabele om de spelerinformatie op te slaan

    // Functie om de spelerinformatie op te slaan
    public void SavePlayerData()
    {
        playerSaveData.maxHitPoints = Player.MaxHitPoints;
        playerSaveData.hitPoints = Player.HitPoints;
        playerSaveData.defense = Player.Defense;
        playerSaveData.power = Player.Power;
        playerSaveData.level = Player.Level;
        playerSaveData.xp = Player.XP;
        playerSaveData.xpToNextLevel = Player.XPToNextLevel;

        // Converteer de spelerdata naar JSON
        string jsonData = JsonUtility.ToJson(playerSaveData);

        // Bewaar de JSON-data in de spelerprefs
        PlayerPrefs.SetString("PlayerSaveData", jsonData);
        PlayerPrefs.Save();
    }

    // Functie om de spelerinformatie te laden
    public void LoadPlayerData()
    {
        // Controleer of er een opgeslagen spelerbestand is
        if (PlayerPrefs.HasKey("PlayerSaveData"))
        {
            // Haal de JSON-data op uit de spelerprefs
            string jsonData = PlayerPrefs.GetString("PlayerSaveData");

            // Converteer de JSON-data naar de SaveGame structuur
            playerSaveData = JsonUtility.FromJson<SaveGame>(jsonData);

            // Update de speler met de opgeslagen gegevens
            Player.MaxHitPoints = playerSaveData.maxHitPoints;
            Player.HitPoints = playerSaveData.hitPoints;
            Player.Defense = playerSaveData.defense;
            Player.Power = playerSaveData.power;
            Player.Level = playerSaveData.level;
            Player.XP = playerSaveData.xp;
            Player.XPToNextLevel = playerSaveData.xpToNextLevel;
        }
    }

    // Functie om de savegame te verwijderen als de speler sterft
    public void RemoveSaveGame()
    {
        PlayerPrefs.DeleteKey("PlayerSaveData");
        PlayerPrefs.Save();
    }

    // Functie om de vloer te wissen
    public void ClearFloor()
    {
        foreach (var enemy in Enemies)
        {
            Destroy(enemy.gameObject);
        }
        Enemies.Clear();

        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }
        Items.Clear();

        foreach (var ladder in Ladders)
        {
            Destroy(ladder.gameObject);
        }
        Ladders.Clear();

        foreach (var stone in Tombstones)
        {
            Destroy(stone.gameObject);
        }
        Tombstones.Clear();

        // Update UI elements if necessary
        UIManager.Instance.UpdateEnemiesLeftText(0);
    }
}
