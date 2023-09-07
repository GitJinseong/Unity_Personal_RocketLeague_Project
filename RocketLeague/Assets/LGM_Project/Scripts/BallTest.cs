using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTest : MonoBehaviour
{
    private Rigidbody ballRb;
    private float pushForce = default;

    void Awake()
    {
        ballRb = GetComponent<Rigidbody>();

        pushForce = 5000f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ballRb.AddForce(transform.right * pushForce);
        }
    }
}
