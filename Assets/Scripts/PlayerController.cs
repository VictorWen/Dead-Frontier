using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Physics Fields
    public float moveSpeed = 3;
    private Vector2 inputVector = new Vector2();
    private Rigidbody2D body;

    // ===============
    //  UNITY METHODS
    // ===============

    private void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        SaveInput();
    }

    private void FixedUpdate() {
        ApplyMovement();
    }

    // ================
    //  HELPER METHODS
    // ================

    private void SaveInput() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        inputVector = new Vector2(horizontal, vertical);
    }

    private void ApplyMovement() {
        if (inputVector.x != 0 || inputVector.y != 0) {
            Vector2 displacement = inputVector * moveSpeed * Time.fixedDeltaTime;
            body.MovePosition(body.position + displacement);
        }
    }
}