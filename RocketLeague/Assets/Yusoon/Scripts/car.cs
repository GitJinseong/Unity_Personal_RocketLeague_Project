using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class car : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
                
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidBody=collision.gameObject.GetComponent<Rigidbody>();
        if (rigidBody != null) 
        {
            Vector3 dir = (collision.transform.position-transform.position).normalized;
            rigidBody.AddForce(dir*50, ForceMode.Impulse);
        }
    }
}


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public GameObject leftWheelObj;
    public GameObject rightWheelObj;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
