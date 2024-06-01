using Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public Label[] labels = new Label[8];
    private VisualElement root;
    private int selected;
    private int numItems;

    public int Selected { get => selected; }

    private void Start()
    {
        // Assuming you have set up a UIDocument in your scene and linked it to this script
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Initialize labels array
        for (int i = 0; i < 8; i++)
        {
            labels[i] = root.Q<Label>($"Item{i + 1}");
        }

        // Clear all labels and hide the GUI
        Clear();
        root.style.display = DisplayStyle.None;
    }

    public void Clear()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].text = string.Empty;
            labels[i].style.backgroundColor = new StyleColor(Color.clear); // Clear background color as well
        }
        selected = 0;
        numItems = 0;
    }

    private void UpdateSelected()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (i == selected)
            {
                labels[i].style.backgroundColor = new StyleColor(Color.green);
            }
            else
            {
                labels[i].style.backgroundColor = new StyleColor(Color.clear);
            }
        }
    }

    public void SelectNextItem()
    {
        if (numItems == 0) return;

        selected = (selected + 1) % numItems;
        UpdateSelected();
    }

    public void SelectPreviousItem()
    {
        if (numItems == 0) return;

        selected = (selected - 1 + numItems) % numItems;
        UpdateSelected();
    }

    public void Show(List<Consumable> list)
    {
        selected = 0;
        numItems = list.Count;
        Clear();

        for (int i = 0; i < list.Count && i < labels.Length; i++)
        {
            if (list[i] != null) // Check if the item is not null
            {
                labels[i].text = list[i].name;
            }
        }

        UpdateSelected();
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
}
