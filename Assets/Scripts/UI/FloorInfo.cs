using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class FloorInfo : MonoBehaviour
{
    public Text floorText;
    public Text enemiesLeftText;

    public void SetFloor(int floorNumber)
    {
        floorText.text = "Floor " + floorNumber;
    }

    public void SetEnemiesLeft(int enemiesCount)
    {
        enemiesLeftText.text = enemiesCount + " enemies left";
    }
}
