using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private AdamMilVisibility algorithm;
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();
    public int FieldOfViewRange = 8;

    [Header("Powers")]
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int power;
    [SerializeField] private int level;
    [SerializeField] private int xp;
    [SerializeField] private int xpToNextLevel;

    public int MaxHitPoints { get => maxHitPoints; set => maxHitPoints = value; }
    public int HitPoints { get => hitPoints; set => hitPoints = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Power { get => power; set => power = value; }
    public int Level { get => level; set => level = value; }
    public int XP { get => xp; set => xp = value; }
    public int XPToNextLevel { get => xpToNextLevel; set => xpToNextLevel = value; }

    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
            HealthBar.Instance.SetLevel(level);
            HealthBar.Instance.SetXP(xp);
        }
    }

    public void Move(Vector3 direction)
    {
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            transform.position += direction;
        }
    }

    public void UpdateFieldOfView()
    {
        var pos = MapManager.Get.FloorMap.WorldToCell(transform.position);

        FieldOfView.Clear();
        algorithm.Compute(pos, FieldOfViewRange, FieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.Get.UpdateFogMap(FieldOfView);
        }
    }

    private void Die()
    {
        if (GetComponent<Player>())
        {
            UIManager.Instance.AddMessage("You died!", Color.red);
        }
        else
        {
            UIManager.Instance.AddMessage($"{name} is dead!", Color.green);
        }

        GameObject remains = GameManager.Get.CreateActor("Dead", transform.position);
        remains.name = $"Remains of {name}";

        if (!GetComponent<Player>())
        {
            GameManager.Get.RemoveEnemy(this);
        }

        Destroy(gameObject);
    }

    public void DoDamage(int hp, Actor attacker)
    {
        hitPoints -= hp;
        if (hitPoints < 0) hitPoints = 0;

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
        }

        if (hitPoints == 0)
        {
            Die();

            if (attacker != null && attacker.GetComponent<Player>())
            {
                attacker.AddXp(XP);
            }
        }
    }

    public void Heal(int hp)
    {
        int healedHP = Mathf.Min(maxHitPoints - hitPoints, hp);
        hitPoints += healedHP;

        if (GetComponent<Player>())
        {
            UIManager.Instance.UpdateHealth(hitPoints, maxHitPoints);
            UIManager.Instance.AddMessage($"You were healed for {healedHP} HP!", Color.green);
        }
    }

    public void AddXp(int xp)
    {
        this.xp += xp;
        while (this.xp >= xpToNextLevel)
        {
            LevelUp();
        }
        HealthBar.Instance.SetXP(this.xp);
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel = Mathf.FloorToInt(xpToNextLevel * 1.5f);
        maxHitPoints += 10;
        defense += 2;
        power += 2;

        HealthBar.Instance.SetLevel(level);
        UIManager.Instance.AddMessage($"Congratulations! You've reached level {level}!", Color.yellow);
    }
}
