using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangePushCar : MonoBehaviour
{
    private Vector3 pushVector;   // 차들이 날아갈 거리 벡터값

    private Rigidbody onCarRb;   // 골대 거리에 들어온 차들의 리짓바디

    private void OnTriggerEnter(Collider collision)
    {
           // 차 태그가 달려있는 오브젝트가 push 콜라이더 안에 있을경우 실행
        if ((collision.tag == "Car_Blue" || collision.tag == "Car_Orange"))
        {
            onCarRb = collision.gameObject.GetComponent<Rigidbody>();   // 날아갈 차들의 리짓바디를 가져온다
            pushVector = collision.transform.position - transform.position;   // 현재 차 위치에서 골인 지점 위치를 뺀다
            pushVector = pushVector.normalized;   // 계산된 위치값을 1 의 값으로 변경해준다
            pushVector += Vector3.up * 80;   // 계산된 위치값에 점프력을 증가시킨다
            pushVector += Vector3.right * 120;   // 계산된 위치값에 날아갈 거리값을 증가시킨다
            onCarRb.AddForce(pushVector * 800, ForceMode.Impulse);   // 골인 효과 범위내에 있는 차 들을 AddForce 로 힘을 줘 밀어낸다
        }
    }
}
