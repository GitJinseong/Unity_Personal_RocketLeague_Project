using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public Transform kartNormal;
    public int jumpCount=0;
    public NewCar car;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(car.isGrounded&&!Input.GetKeyDown(KeyCode.Space))
        {
            jumpCount=0;
        }

        if(Input.GetMouseButtonDown(1))
        {
            car.isGrounded=false;
          if(jumpCount<1)
            {
                rb.AddForce(kartNormal.up*40f, ForceMode.VelocityChange);
                jumpCount+=1;
            }

           
        }
    }
   
}
