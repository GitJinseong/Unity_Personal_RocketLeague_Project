using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Ys : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FreezeBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        StartCoroutine(FreezeCoolDown());
    }
    private IEnumerator FreezeCoolDown() 
    {
        yield return new WaitForSeconds(3);
        rb.useGravity = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.useGravity = true;
        StopAllCoroutines();
    }
}
