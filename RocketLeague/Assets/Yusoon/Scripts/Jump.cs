using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public Transform kartNormal;
    Rigidbody rb;
    int jumpCount=0;
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            jumpCount+=1;
            if(jumpCount<=2)
            {
            rb.AddForce(kartNormal.up*50f, ForceMode.Impulse);

            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Floor"))
        {
            jumpCount=0;
        }
    }
}
