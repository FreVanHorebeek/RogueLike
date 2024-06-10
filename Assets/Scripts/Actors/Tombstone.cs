using UnityEngine;

public class Tombstone : MonoBehaviour
{
    private void Start()
    {
        GameManager.Get.AddTombStone(this); // Add this line to the Start function
    }
}

