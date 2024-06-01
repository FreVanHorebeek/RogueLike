using Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    public Inventory inventory;

    private bool inventoryIsOpen = false;
    private bool droppingItem = false;
    private bool usingItem = false;

    private void Awake()
    {
        controls = new Controls();
        inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory component not found on player.");
        }
    }

    private void Start()
    {
        GameManager.Get.Player = GetComponent<Actor>();
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    private void OnEnable()
    {
        if (controls != null)
        {
            controls.Player.SetCallbacks(this);
            controls.Enable();
        }
        else
        {
            Debug.LogError("Controls not initialized.");
        }
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.SetCallbacks(null);
            controls.Disable();
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();

            if (inventoryIsOpen)
            {
                if (direction.y > 0)
                {
                    UIManager.Instance.InventoryUI.SelectPreviousItem();
                }
                else if (direction.y < 0)
                {
                    UIManager.Instance.InventoryUI.SelectNextItem();
                }
            }
            else
            {
                Move(direction);
            }
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryIsOpen)
            {
                // Sluit de inventory via de UIManager
                UIManager.Instance.InventoryUI.Hide();

                // Zet de waarden van inventoryIsOpen, droppingItem en usingItem op false
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
            }
        }
    }

    private void Move(Vector2 direction)
    {
        Vector3 roundedDirection = new Vector3(Mathf.Round(direction.x), Mathf.Round(direction.y), 0);
        Debug.Log("roundedDirection: " + roundedDirection);
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 playerPosition = transform.position;
            Consumable item = GameManager.Get.GetItemAtLocation(playerPosition);

            if (item == null)
            {
                Debug.Log("No item at player's location.");
            }
            else if (inventory.IsFull)
            {
                Debug.Log("Inventory is full.");
            }
            else
            {
                inventory.AddItem(item);
                item.gameObject.SetActive(false);
                GameManager.Get.RemoveItem(item);
                Debug.Log($"Picked up {item.name}");
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                // Toon de inventory via de UIManager
                UIManager.Instance.InventoryUI.Show(inventory.Items);

                // Zet de waarde van inventoryIsOpen op true
                inventoryIsOpen = true;

                // Zet de waarde van droppingItem op true
                droppingItem = true;
            }
            else
            {
                // Implementeer de logica voor het droppen van een item
                Consumable selectedItem = inventory.Items[UIManager.Instance.InventoryUI.Selected];
                inventory.DropItem(selectedItem);
                selectedItem.transform.position = transform.position;
                GameManager.Get.AddItem(selectedItem);
                selectedItem.gameObject.SetActive(true);

                // Werk de UI bij en reset de status
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
                UIManager.Instance.InventoryUI.Hide();
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryIsOpen)
        {
            Consumable selectedItem = inventory.Items[UIManager.Instance.InventoryUI.Selected];
            if (droppingItem)
            {
                inventory.DropItem(selectedItem);
                selectedItem.transform.position = transform.position;
                GameManager.Get.AddItem(selectedItem);
                selectedItem.gameObject.SetActive(true);
            }
            else if (usingItem)
            {
                UseItem(selectedItem);
                inventory.Items.Remove(selectedItem);
                Destroy(selectedItem.gameObject);
            }

            // Werk de UI bij en reset de status
            inventoryIsOpen = false;
            droppingItem = false;
            usingItem = false;
            UIManager.Instance.InventoryUI.Hide();
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                // Toon de inventory via de UIManager
                UIManager.Instance.InventoryUI.Show(inventory.Items);

                // Zet de waarde van inventoryIsOpen op true
                inventoryIsOpen = true;

                // Zet de waarde van usingItem op true
                usingItem = true;
            }
            else
            {
                Consumable selectedItem = inventory.Items[UIManager.Instance.InventoryUI.Selected];

                if (droppingItem)
                {
                    inventory.DropItem(selectedItem);
                    selectedItem.transform.position = transform.position;
                    GameManager.Get.AddItem(selectedItem);
                    selectedItem.gameObject.SetActive(true);
                }
                else if (usingItem)
                {
                    UseItem(selectedItem);
                    inventory.Items.Remove(selectedItem);
                    Destroy(selectedItem.gameObject);
                }

                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
                UIManager.Instance.InventoryUI.Hide();
            }
        }
    }

    private void UseItem(Consumable item)
    {
        if (item != null)
        {
            if (item is HealthPotion healthPotion)
            {
                int healAmount = healthPotion.HealAmount;
                GetComponent<Actor>().Heal(healAmount);
                Debug.Log($"Used {item.name}. Restored {healAmount} health points.");
            }
            else if (item is Fireball fireball)
            {
                List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in nearbyEnemies)
                {
                    int damage = fireball.Damage;
                    enemy.DoDamage(damage);
                }
                Debug.Log($"Used {item.name}. Dealt {fireball.Damage} damage to nearby enemies.");
            }
            else if (item is ScrollOfConfusion)
            {
                List<Actor> nearbyEnemies = GameManager.Get.GetNearbyEnemies(transform.position);
                foreach (Actor enemy in nearbyEnemies)
                {
                    enemy.GetComponent<Enemy>().Confuse();
                }
                Debug.Log($"Used {item.name}. Confused nearby enemies.");
            }
        }
    }
}
