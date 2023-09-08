using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UIElements;

public class NewCar_Yoo : MonoBehaviour
{
    public Transform kartNormal;
    public Transform kartModel;
    public float acceleration;
    public float steering =240f;
    public float gravity = 10f;
    public LayerMask layerMask;

    bool drifting =false;
    float speed;
    float currentSpeed;
    float rotate;
    float currentRotate;
    public Rigidbody sphere;

    // 부스터 관련 추가 사항
    private CarBooster_Yoo booster;
    private float basicAcceleration;
    private float timeAfterFirstBoost;
    private float secondBoostDelay;
    private bool useSecondBoost;
    // 부스터 관련 추가 사항

    // Start is called before the first frame update
    void Start()
    {
        // 부스터 관련 추가 사항
        basicAcceleration = acceleration;
        useSecondBoost = false;
        secondBoostDelay = 0.75f;
        // 부스터 관련 추가 사항

        booster = GetComponent<CarBooster_Yoo>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newposition = new Vector3(sphere.transform.position.x, sphere.transform.position.y-3.5f, sphere.transform.position.z);
        transform.position=newposition;
        float speedDir= Input.GetAxis("Vertical");

        // 부스터 관련 추가 사항
        if(booster.useBoost)
        {
            speedDir = 1;
        }
        // 부스터 관련 추가 사항

        speed = speedDir * acceleration;
        //Debug.Log(speed);
        if(Input.GetAxis("Horizontal") != 0)
        {
            
            float dir = Input.GetAxis("Horizontal");
            float amount = Mathf.Abs((Input.GetAxis("Horizontal")));
            if(speedDir!= -1)
            {
            Steer(dir, amount);

            }
            else
            {
                Steer(-dir, amount);

            }

        }
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        if (!drifting)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90+(Input.GetAxis("Horizontal") * 15), kartModel.localEulerAngles.z), .2f);
        }

        // 부스터 관련 추가 사항
        if (booster.useBoost == false)
        {
            acceleration = basicAcceleration;
            useSecondBoost = false;
            timeAfterFirstBoost = 0;
        }

        if (!useSecondBoost && acceleration >= basicAcceleration * 2.25f)
        {
            timeAfterFirstBoost += Time.deltaTime;
            Debug.Log("2단 부스트 까지 " + (secondBoostDelay - timeAfterFirstBoost) + "초 남음");
            if(timeAfterFirstBoost > secondBoostDelay)
            {
                useSecondBoost = true;
                timeAfterFirstBoost = 0;
                Debug.Log("2단 부스트 사용");
            }
        }
        // 부스터 관련 추가 사항
    }
    private void FixedUpdate()
    {
        sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);

        // 부스터 관련 추가 사항
        if (booster.useBoost == true)
        {
            if(!useSecondBoost)
            {
                acceleration *= 1.1f;
                if (acceleration >= basicAcceleration * 2.25f)
                {
                    acceleration = basicAcceleration * 2.25f;
                }
            }

            if(useSecondBoost)
            {
                acceleration = basicAcceleration * 3.5f;
            }
        }
        // 부스터 관련 추가 사항
    }
    public void Steer(float direction,float amount)
    {
        rotate += (steering *direction) * amount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb=collision.gameObject.GetComponent<Rigidbody>();
        if(rb != null)
        {
            Vector3 dir = (collision.transform.position-transform.position).normalized;
            rb.AddForce(dir*currentSpeed, ForceMode.Impulse);
        }
    }
}
