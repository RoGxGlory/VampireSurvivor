using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformResetter : MonoBehaviour
{
    private Vector3 initialPosition; // Store the initial position of the player

    void Start()
    {
        // Save the initial position of the player
        initialPosition = transform.position;
    }

    public void ResetPlayerPosition()
    {
        // Reset the player's position to the initial position
        transform.position = initialPosition;
    }
}
