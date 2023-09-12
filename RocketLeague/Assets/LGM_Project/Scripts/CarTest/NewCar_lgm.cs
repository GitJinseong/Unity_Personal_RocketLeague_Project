using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UIElements;

public class NewCar_lgm : MonoBehaviour
{
    public Transform kartNormal;
    public Transform kartModel;
    public float acceleration;
    public float steering;
    public float gravity;
    public LayerMask layerMask;
    public LayerMask fieldMask;

    bool drifting =false;
    float speed;
   public float currentSpeed;
   float rotate;
  float currentRotate;
    public Rigidbody sphere;
    private bool isWalled = false;
    private Rigidbody colliderRb;

    void Awake()
    {
        colliderRb = GameObject.Find("Collider").GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 newposition = new Vector3(sphere.transform.position.x, sphere.transform.position.y-3.5f, sphere.transform.position.z);
        transform.position=newposition;
        float speedDir = Input.GetAxis("Vertical");
        speed = speedDir * acceleration;

        if (Input.GetAxis("Horizontal") != 0)
        {

            float dir = Input.GetAxis("Horizontal");
            float amount = Mathf.Abs((Input.GetAxis("Horizontal")));
            if (speedDir!= -1)
            {
                Steer(dir, amount);

            }
            else
            {
                Steer(-dir, amount);

            }

        }
        Vector3 rayStartPoint;
        if (Input.GetAxis("Vertical")>0)
        {
            rayStartPoint = kartNormal.position + (kartNormal.up * .3f) + (kartNormal.right*2);
        }
        else
        {
            rayStartPoint = kartNormal.position + (kartNormal.up * .3f) + (-kartNormal.right*2);


        }


        Vector3 rayDirection = -kartNormal.up; // Ray를 아래로 쏘는 방향으로 설정

        RaycastHit hitOn;
        RaycastHit hitNear;

        Vector3 newGravityDirection = -kartNormal.up; // 차량의 정렬된 방향을 중력 방향으로 사용
       // Physics.gravity = newGravityDirection * gravity;
        Physics.Raycast(rayStartPoint, rayDirection, out hitOn, 4f, layerMask);
        Physics.Raycast(rayStartPoint, rayDirection, out hitNear, 4f, layerMask);

        // Visualize the raycast
        Debug.DrawRay(rayStartPoint, rayDirection * 4f, Color.green); // Ray를 시각화합니다.

        // Calculate the rotation to align kartNormal with the ground normal
        Vector3 targetNormal = hitNear.normal;
        Quaternion targetRotation = Quaternion.FromToRotation(kartNormal.up, targetNormal);

        // Smoothly adjust the kartNormal's rotation
        kartNormal.rotation = Quaternion.Slerp(kartNormal.rotation, targetRotation * kartNormal.rotation, Time.deltaTime * 8.0f);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);


        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        if (!drifting)
        {
           
            
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90+(Input.GetAxis("Horizontal") * 15), kartModel.localEulerAngles.z), .2f);

            
        }



    }
    private void FixedUpdate()
    {

        sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
       
        //sphere.AddForce(-kartNormal.transform.up * gravity, ForceMode.Acceleration);
      
        


        //RaycastHit hitOn;
        //RaycastHit hitNear;

        //Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 3f, layerMask);
        //Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 3f, layerMask);

        ////Normal Rotation
        //kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        //kartNormal.Rotate(0, transform.eulerAngles.y, 0);


        //Vector3 rayDirection = -kartNormal.up; // Ray를 아래로 쏘는 방향으로 설정

        //RaycastHit hitOn;
        //RaycastHit hitNear;

        //Physics.Raycast(kartNormal.position + (kartNormal.up * .3f), rayDirection, out hitOn, 3f, layerMask);
        //Physics.Raycast(kartNormal.position + (kartNormal.up * .3f), rayDirection, out hitNear, 3f, layerMask);

        //// Visualize the raycast
        //Debug.DrawRay(kartNormal.position + (kartNormal.up * .1f), rayDirection * 3f, Color.green); // Visualize the ray

        //// Calculate the rotation to align kartNormal with the ground normal
        //Vector3 targetNormal = hitNear.normal;
        //Quaternion targetRotation = Quaternion.FromToRotation(kartNormal.up, targetNormal);

        //// Smoothly adjust the kartNormal's rotation
        //kartNormal.rotation = Quaternion.Slerp(kartNormal.rotation, targetRotation * kartNormal.rotation, Time.deltaTime * 8.0f);














    }
    public void Steer(float direction,float amount)
    {
        rotate += (steering *direction) * amount;
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 wallNormal = collision.contacts[0].normal;
        transform.position += wallNormal * currentSpeed * Time.deltaTime; // moveSpeed는 움직임 속도입니다.
        transform.rotation = Quaternion.FromToRotation(transform.up, wallNormal) * transform.rotation;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == ("Wall"))
        {
            isWalled = true;
            colliderRb.useGravity = false;
            Debug.Log("벽에 붙었다");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == ("Wall"))
        {
            isWalled = false;
            colliderRb.useGravity = true;
            Debug.Log("벽에서 떨어졌다");
        }
    }

}
