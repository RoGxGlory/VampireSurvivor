using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float moveSpeed = 5f; // Player's movement speed
    private Vector2 movementInput; // Stores player input for movement

    public bool bIsPlaying = false;

    private Stats playerStats;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on the player object.");
        }
        playerStats = GetComponent<Stats>();
        moveSpeed = moveSpeed * (playerStats.GetSpeedModifier()) / 100;

    }

    void Update()
    {
        if (bIsPlaying)
        {
            // Get movement input (ZQSD or Arrow keys)
            movementInput.x = Input.GetAxis("Horizontal");
            movementInput.y = Input.GetAxis("Vertical");
        }
        else
        {
            movementInput = Vector2.zero; // Stop input when not playing
        }
    }

    void FixedUpdate()
    {
        if (bIsPlaying)
        {
            // Apply movement to the Rigidbody2D
            rb.linearVelocity = movementInput * moveSpeed;
        }
    }

    public void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.InGame)
        {
            EnablePlayerControl();
        }
        else
        {
            DisablePlayerControl();
        }
    }

    private void EnablePlayerControl()
    {
        bIsPlaying = true;
    }

    private void DisablePlayerControl()
    {
        rb.linearVelocity = Vector3.zero; // Stop movement
        bIsPlaying = false;
    }


}
