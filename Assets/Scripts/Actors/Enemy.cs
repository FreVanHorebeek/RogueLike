using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))] // Voeg automatisch een Actor-component toe
public class Enemy : MonoBehaviour
{
    private void Start()
    {
        // Voeg deze enemy toe aan de lijst met enemies in de GameManager
        GameManager.Get.AddEnemy(GetComponent<Actor>());
    }
}



