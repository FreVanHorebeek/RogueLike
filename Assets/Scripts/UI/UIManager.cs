using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Documents")]
    public GameObject HealthBar;
    public GameObject Messages;
    public GameObject inventory;

    public InventoryUI InventoryUI { get => inventory.GetComponent<InventoryUI>(); }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void UpdateHealth(int current, int max)
    {
        HealthBar.GetComponent<HealthBar>().SetValues(current, max);
    }

    public void SetPlayerLevel(int level) // Voeg deze functie toe
    {
        HealthBar.GetComponent<HealthBar>().SetLevel(level);
    }

    public void SetPlayerXP(int xp) // Voeg deze functie toe
    {
        HealthBar.GetComponent<HealthBar>().SetXP(xp);
    }

    public void AddMessage(string message, Color color)
    {
        Messages.GetComponent<Messages>().AddMessage(message, color);
    }

    public FloorInfo floorInfo;

    public void InitializeFloorInfo(FloorInfo info)
    {
        floorInfo = info;
    }

    public void UpdateFloorText(int floorNumber)
    {
        if (floorInfo != null)
        {
            floorInfo.SetFloor(floorNumber);
        }
    }

    public void UpdateEnemiesLeftText(int enemiesCount)
    {
        if (floorInfo != null)
        {
            floorInfo.SetEnemiesLeft(enemiesCount);
        }
    }
}
