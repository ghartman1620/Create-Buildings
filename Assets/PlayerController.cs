using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the player's avatar, a sphere.

// This code from the Unity "Roll a Ball" tutorial example.

public class PlayerController : MonoBehaviour
{
    // Public variables can be modified in the Unity editor without changing this script! Neat, huh?
    // Can use them in place of magic numbers!
    public float speed;

    private Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        // This code is verbatim from the "roll a ball" example.

        // Set some local float variables equal to the value of our Horizontal and Vertical Inputs
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
        // multiplying it by 'speed' - our4 public player speed that appears in the inspector
        rb.AddForce(movement * speed);
    }
}
