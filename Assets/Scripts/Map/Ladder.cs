using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private bool up;

    public bool Up { get => up; set => up = value; }

    private void Start()
    {
        // Voeg deze ladder toe aan de GameManager
        GameManager.Get.AddLadder(this);
    }
}
