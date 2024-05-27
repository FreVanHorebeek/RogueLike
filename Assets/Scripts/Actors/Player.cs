using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        GameManager.Get.Player = GetComponent<Actor>();
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move();
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        // Add your exit logic here if needed
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector3 roundedDirection = new Vector3(Mathf.Round(direction.x), Mathf.Round(direction.y), 0);
        Debug.Log("roundedDirection: " + roundedDirection);
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }
}
