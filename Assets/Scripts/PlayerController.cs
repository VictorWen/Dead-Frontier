using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement Fields
    [SerializeField] private float moveSpeed = 3;
    private Vector2 inputVector = new Vector2();
    private Rigidbody2D body;

    // Shooting Fields
    private PlayerGunScript shootScript;

    // ===============
    //  UNITY METHODS
    // ===============

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        shootScript = GetComponent<PlayerGunScript>();
        shootScript.SetAimVisibility(false);
    }

    private void Update()
    {
        SaveMovementInput();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    // ================
    //  HELPER METHODS
    // ================

    private void SaveMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        inputVector = new Vector2(horizontal, vertical);
    }

    private void ApplyMovement()
    {
        if (inputVector.x != 0 || inputVector.y != 0)
        {
            Vector2 displacement = inputVector * moveSpeed * Time.fixedDeltaTime;
            body.MovePosition(body.position + displacement);
        }
    }
}