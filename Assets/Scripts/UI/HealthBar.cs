using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance { get; private set; } // Definieer de Instance eigenschap

    private VisualElement root;
    private VisualElement healthBar;
    private Label healthLabel;
    private Label levelLabel;
    private Label xpLabel;

    void Start()
    {
        Instance = this; // Stel de Instance eigenschap in wanneer de klasse wordt gestart

        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        healthBar = root.Q<VisualElement>("HealthBar");
        healthLabel = root.Q<Label>("HealthText");
        levelLabel = root.Q<Label>("LevelText"); // Voeg dit toe
        xpLabel = root.Q<Label>("XPText"); // Voeg dit toe
    }

    public void SetValues(int currentHitPoints, int maxHitPoints)
    {
        float percent = (float)currentHitPoints / maxHitPoints * 100;
        healthBar.style.width = new Length(percent, LengthUnit.Percent);
        healthLabel.text = $"{currentHitPoints}/{maxHitPoints} HP";
    }

    public void SetLevel(int level) // Voeg deze functie toe
    {
        levelLabel.text = $"Level: {level}";
    }

    public void SetXP(int xp) // Voeg deze functie toe
    {
        xpLabel.text = $"XP: {xp}";
    }
}
