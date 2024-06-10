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
                UIManager.Instance.InventoryUI.Hide();
                inventoryIsOpen = false;
                droppingItem = false;
                usingItem = false;
            }
        }
    }

    private void Move(Vector2 direction)
    {
        Vector3 roundedDirection = new Vector3(Mathf.Round(direction.x), Mathf.Round(direction.y), 0);
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
                UIManager.Instance.InventoryUI.Show(inventory.Items);
                inventoryIsOpen = true;
                droppingItem = true;
            }
            else
            {
                Consumable selectedItem = inventory.Items[UIManager.Instance.InventoryUI.Selected];
                inventory.DropItem(selectedItem);
                selectedItem.transform.position = transform.position;
                GameManager.Get.AddItem(selectedItem);
                selectedItem.gameObject.SetActive(true);
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
            Vector3 playerPosition = transform.position;
            Ladder ladder = GameManager.Get.GetLadderAtLocation(playerPosition);

            if (ladder != null)
            {
                if (ladder.Up)
                {
                    MapManager.Get.MoveUp();
                }
                else
                {
                    MapManager.Get.MoveDown();
                }
            }
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                UIManager.Instance.InventoryUI.Show(inventory.Items);
                inventoryIsOpen = true;
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
        var player = GetComponent<Actor>();
        switch (item.Type)
        {
            case Consumable.ItemType.HealthPotion:
                player.Heal(5);
                break;
            case Consumable.ItemType.Fireball:
                {
                    var enemies = GameManager.Get.GetNearbyEnemies(transform.position);
                    foreach (var enemy in enemies)
                    {
                        enemy.DoDamage(8, player); // Add player as attacker
                        UIManager.Instance.AddMessage($"Your fireball damaged the {enemy.name} for 8HP", Color.magenta);
                    }
                    break;
                }
            case Consumable.ItemType.ScrollOfConfusion:
                {
                    var enemies = GameManager.Get.GetNearbyEnemies(transform.position);
                    foreach (var enemy in enemies)
                    {
                        enemy.GetComponent<Enemy>().Confuse();
                        UIManager.Instance.AddMessage($"Your scroll confused the {enemy.name}.", Color.magenta);
                    }
                    break;
                }
        }
    }


}
