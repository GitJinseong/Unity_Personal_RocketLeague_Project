using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastCar : MonoBehaviour
{
    public Transform leftFrontTire;
    public Transform rightFrontTire;
    public Transform leftRearTire;
    public Transform rightRearTire;

    Rigidbody rigidBody;
    public float rayLength = 1.0f;
    public float suspensionHeight = 1f; // 원하는 서스펜션 높이
    public float suspensionForce = 10.0f; // 서스펜션 힘
 
    public float suspensionSpring = 5000.0f;
    public float suspensionDamper = 50.0f;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //  // 왼쪽 앞 타이어에서 Ray 발사
        //  RaycastHit hitInfoLeftFront;
        //  Vector3 rayDirectionLeftFront = -leftFrontTire.up;
        //  if (Physics.Raycast(leftFrontTire.position, rayDirectionLeftFront, out hitInfoLeftFront, rayLength))
        //  {
        //      // Ray가 어떤 물체에 부딪혔을 때의 동작을 여기에 추가하세요.
        //      rigidBody.AddForce(leftFrontTire.up*5);
        //      // 디버그 레이를 그립니다.
        //      Debug.DrawRay(leftFrontTire.position, rayDirectionLeftFront * rayLength, Color.red);
        //  }
        //  else
        //  {
        //      // Ray가 부딪히지 않았을 때의 동작을 여기에 추가하세요.

        //      // 디버그 레이를 그립니다.
        //      Debug.DrawRay(leftFrontTire.position, rayDirectionLeftFront * rayLength, Color.green);
        //  }

        //  Vector3 rayDirectionRightFront = -rightFrontTire.up;
        //// 오른쪽 앞 타이어에서 Ray 발사
        //RaycastHit hitInfoRightFront;
        //  if (Physics.Raycast(rightFrontTire.position, rayDirectionRightFront, out hitInfoRightFront, rayLength))
        //  {
        //      rigidBody.AddForce(rightFrontTire.up * 5);
        //      // Ray가 어떤 물체에 부딪혔을 때의 동작을 여기에 추가하세요.
        //      Debug.DrawRay(rightFrontTire.position, rayDirectionRightFront * rayLength, Color.red);
        //  }
        //  else
        //  {
        //      // Ray가 부딪히지 않았을 때의 동작을 여기에 추가하세요.

        //      // 디버그 레이를 그립니다.
        //      Debug.DrawRay(rightFrontTire.position, rayDirectionRightFront * rayLength, Color.green);
        //  }

        //  // 왼쪽 뒷 타이어에서 Ray 발사
        //  RaycastHit hitInfoLeftRear;

        //  Vector3 rayDirectionLeftRear = -leftRearTire.up;

        //  if (Physics.Raycast(leftRearTire.position, rayDirectionLeftRear, out hitInfoLeftRear, rayLength))
        //  {
        //      rigidBody.AddForce(leftRearTire.up*5);
        //      // Ray가 어떤 물체에 부딪혔을 때의 동작을 여기에 추가하세요.
        //      Debug.DrawRay(leftRearTire.position, rayDirectionLeftRear * rayLength, Color.red);

        //  }
        //  else
        //  {
        //      // Ray가 부딪히지 않았을 때의 동작을 여기에 추가하세요.

        //      // 디버그 레이를 그립니다.
        //      Debug.DrawRay(leftRearTire.position, rayDirectionLeftRear * rayLength, Color.green);

        //  }


        //  // 오른쪽 뒷 타이어에서 Ray 발사
        //  RaycastHit hitInfoRightRear;
        //  Vector3 rayDirectionRightRear = -rightRearTire.up;

        //  if (Physics.Raycast(rightRearTire.position, rayDirectionRightRear, out hitInfoRightRear, rayLength))
        //  {
        //      rigidBody.AddForce(rightRearTire.up * 5);
        //      // Ray가 어떤 물체에 부딪혔을 때의 동작을 여기에 추가하세요.
        //      Debug.DrawRay(rightRearTire.position, rayDirectionRightRear * rayLength, Color.red);
        //  }
        //  else
        //  {
        //      // Ray가 부딪히지 않았을 때의 동작을 여기에 추가하세요.

        //      // 디버그 레이를 그립니다.
        //      Debug.DrawRay(rightRearTire.position, rayDirectionRightRear * rayLength, Color.green);


        //  }
        UpdateSuspension(leftFrontTire);
        UpdateSuspension(rightFrontTire);
        UpdateSuspension(leftRearTire);
        UpdateSuspension(rightRearTire);
        if(Input.GetKey(KeyCode.W))
        {
            rigidBody.AddForce(transform.forward*50, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidBody.AddForce(-transform.forward*50, ForceMode.Acceleration);
        }
      
    }

    void UpdateSuspension(Transform tire)
    {
        RaycastHit hitInfo;
        Vector3 rayDirection = -tire.up;
        Vector3 suspensionForceDirection = tire.up;

        if (Physics.Raycast(tire.position, rayDirection, out hitInfo, rayLength))
        {
            // 땅에 닿았을 때, 타이어를 서스펜션 높이로 끌어올린다.
            float suspensionCompression = rayLength - hitInfo.distance;
            Vector3 suspensionForceVector = suspensionForceDirection * suspensionCompression * suspensionForce;
            rigidBody.AddForceAtPosition(suspensionForceVector, tire.position);
        }
    }
}
