using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class NewCar : MonoBehaviourPunCallbacks
{
    public Transform kartNormal;
    public Transform kartModel;
    public Transform cameraCenter;
    public float acceleration;
    public float steering;
    public float gravity;
    public LayerMask layerMask;
    public LayerMask fieldMask;

     public bool isGrounded = false;
    
    bool drifting = false;
    float speed;
    public float currentSpeed;
    float rotate;
    float currentRotate;
    public Rigidbody sphere;
    public bool outOfControl = false;
    private bool isNotControl = false;
    // ?ロ??? ???? ???
    #region
    private CarBooster_Yoo booster;
    private float normalAcceleration;
    private float timeAfterFirstBoost;
    private float useSecondBoostDelay = 0.75f;

    private Quaternion targetCameraRotation;
    private float rotationLerpSpeed = 5.0f; // ???? ??? ????
    public bool useSecondBoost { get; private set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // ?ロ??? ???? ???
        #region
        booster = GetComponent<CarBooster_Yoo>();
        normalAcceleration = acceleration;
        useSecondBoost = false;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStartCheck == false) { return; }

        if (photonView.IsMine == false)
        {
            return;
        }
        Vector3 newposition = new Vector3(sphere.transform.position.x, sphere.transform.position.y-3.5f, sphere.transform.position.z);
        transform.position=newposition;
        float speedDir = Input.GetAxis("Vertical");
        if (outOfControl)
        {
            if (isNotControl==false)
            {
                isNotControl = true;
                StartCoroutine(ControlTimer());
            }
            speedDir=1;
            acceleration= normalAcceleration * 3.5f;
        }

        // ?ロ??? ???? ???
        #region
        if (booster.useBoost == true)
        {
            speedDir = 1;
            if (useSecondBoost == false)
            {
                if (acceleration >= normalAcceleration * 2.25f)
                {
                    timeAfterFirstBoost += Time.deltaTime;
                    //if(timeAfterFirstBoost <= useSecondBoostDelay)
                    //{
                    //    Debug.Log("2?? ?ロ?????? ???? ?��?" + (useSecondBoostDelay - timeAfterFirstBoost));
                    //}
                }

                if (timeAfterFirstBoost >= useSecondBoostDelay)
                {
                    useSecondBoost = true;
                    //Debug.Log("2?? ?ロ??? ???");
                }
            }
        }

        if (booster.useBoost == false)
        {
            timeAfterFirstBoost = 0;
            useSecondBoost = false;
        }

        if (outOfControl == false && booster.useBoost == false)
        {
            acceleration = normalAcceleration;
        }
        #endregion

        speed = speedDir * acceleration;
        if (isGrounded == false)
        {
            steering=60f;
        }
        else
        {
            steering=15f;
        }

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


        Vector3 rayDirection = -kartNormal.up; // Ray?? ????? ??? ???????? ????

        RaycastHit hitOn;
        RaycastHit hitNear;

        Vector3 newGravityDirection = -kartNormal.up; // ?????? ????? ?????? ??? ???????? ???
                                                      // Physics.gravity = newGravityDirection * gravity;
        Physics.Raycast(rayStartPoint, rayDirection, out hitOn, 4f, layerMask);
        Physics.Raycast(rayStartPoint, rayDirection, out hitNear, 4f, layerMask);
        isGrounded=Physics.Raycast(rayStartPoint, rayDirection, out hitNear, 4f, layerMask);
       // Debug.LogFormat("isGrounded : {0}", isGrounded);
        // Visualize the raycast
        Debug.DrawRay(rayStartPoint, rayDirection * 4f, Color.green); // Ray?? ?��??????.

        // Calculate the rotation to align kartNormal with the ground normal
        Vector3 targetNormal = hitNear.normal;
        Quaternion targetRotation = Quaternion.FromToRotation(kartNormal.up, targetNormal);

        // Smoothly adjust the kartNormal's rotation
        kartNormal.rotation = Quaternion.Slerp(kartNormal.rotation, targetRotation * kartNormal.rotation, Time.deltaTime * 8.0f);



        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        cameraCenter.position=transform.position;
        if (isGrounded)
        {
            cameraCenter.rotation = Quaternion.Lerp(cameraCenter.rotation, kartModel.rotation, Time.deltaTime * rotationLerpSpeed);
           // targetCameraRotation = cameraCenter.rotation; // ??? ??????? ???? ?????? ????????.
        }


        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 6f); rotate = 0f;

        if (isGrounded)
        {


            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90+(Input.GetAxis("Horizontal") * 15), kartModel.localEulerAngles.z), .2f);


        }
        else
        {
            kartNormal.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 0, 90+(Input.GetAxis("Vertical") * 15)), .2f);

        }





    }
    private void FixedUpdate()
    {
        if (GameManager.instance.gameStartCheck == false) { return; }

        if (photonView.IsMine == false)
        {
            return;
        }
        sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        if(isGrounded)
        {
        sphere.AddForce(-kartNormal.transform.up * gravity, ForceMode.Acceleration);

        }
        else
        {

            sphere.AddForce(-transform.up*gravity, ForceMode.Acceleration);
        }


        // ?ロ??? ???? ???
        #region
        if (booster.useBoost == true && useSecondBoost == false)
        {
            acceleration *= 1.1f;
            if (acceleration >= normalAcceleration * 2.25f)
            {
                acceleration = normalAcceleration * 2.25f;
            }
        }

        if (useSecondBoost == true)
        {
            acceleration = normalAcceleration * 3.5f;
        }
        #endregion


        //RaycastHit hitOn;
        //RaycastHit hitNear;

        //Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 3f, layerMask);
        //Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 3f, layerMask);

        ////Normal Rotation
        //kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        //kartNormal.Rotate(0, transform.eulerAngles.y, 0);


        //Vector3 rayDirection = -kartNormal.up; // Ray?? ????? ??? ???????? ????

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
    private IEnumerator ControlTimer()
    {
        yield return new WaitForSeconds(5);
        outOfControl = false;
        isNotControl = false;
    }
    public void Steer(float direction, float amount)
    {
        rotate += (steering *direction) * amount;
    }
    private void OnCollisionEnter(Collision collision)
    {

      
    }
    private void OnCollisionStay(Collision collision)
    {
        Vector3 wallNormal = collision.contacts[0].normal;
        transform.position += wallNormal * currentSpeed * Time.deltaTime; // moveSpeed?? ?????? ???????.
        transform.rotation = Quaternion.FromToRotation(transform.up, wallNormal) * transform.rotation;
    }

}
