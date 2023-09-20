using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTest : MonoBehaviour
{
    public Rigidbody ballRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ballRb.AddForce(Vector3.up * 20f, ForceMode.Impulse);

            Debug.Log("�� ����!");
        }
    }
}
