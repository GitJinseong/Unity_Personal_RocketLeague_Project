using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public NewCar car;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (collision.transform.position-transform.position).normalized;
            rb.AddForce(dir*car.currentSpeed*0.5f, ForceMode.Impulse);
            
        }
    }
}
