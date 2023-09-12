using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private bool isBreaking;
    private float currentSteeringAngle;
    [SerializeField] private float motorForce;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque=verticalInput*motorForce;
        frontRightWheelCollider.motorTorque=verticalInput*motorForce;
    }

    private void GetInput()
    {
        horizontalInput=Input.GetAxis(HORIZONTAL);
        verticalInput=Input.GetAxis(VERTICAL);

    }

    private void HandleSteering()
    {
        currentSteeringAngle=maxSteeringAngle*horizontalInput;
        frontLeftWheelCollider.steerAngle=currentSteeringAngle;
        frontRightWheelCollider.steerAngle=currentSteeringAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(backLeftWheelCollider, backLeftWheelTransform);
        UpdateSingleWheel(backRightWheelCollider, backRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        Quaternion desiredRotation = Quaternion.Euler(0, 0, -90);
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot*desiredRotation;
        wheelTransform.position = pos;
    }
}
