using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTest : MonoBehaviour
{
    Vector3 pushVector = Vector3.zero;   // 계산 후 RC 카들을 밀어낼 Vector3 위치 값

    private Rigidbody carRb;   // RC 카의 리짓바디

    void Awake()
    {
           // 초기 변수값 설정
        carRb = GetComponent<Rigidbody>();
           // end 초기 변수값 설정
    }

    public void GoalEffect(Vector3 _EffectPosition)   // 골인 효과 범위내에 있을때 작동되는 함수
    {
        pushVector = transform.position - _EffectPosition;   // 현재 RC 카 위치에서 골인 지점 위치를 뺀다
        pushVector = pushVector.normalized;   // 계산된 위치값을 1 의 값으로 변경해준다
        pushVector += Vector3.up * 5;   // 계산된 위치값에 5 만큼 점프력을 증가시킨다
        pushVector += Vector3.forward * 5;   // 계산된 위치값에 5 만큼 직진값을 증가시킨다.
        carRb.AddForce(pushVector * 30, ForceMode.Impulse);   // 골인 효과 범위내에 있는 RC 카들을 AddForce 로 힘을 줘 밀어낸다
    }
}
