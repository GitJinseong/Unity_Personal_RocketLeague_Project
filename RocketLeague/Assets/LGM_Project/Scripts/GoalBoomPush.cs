using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBoomPush : MonoBehaviour
{
    private Collider[] boomCd = new Collider[8];   // 골 효과로 밀어낼 RC 카 배열
    private GameObject car;   // 골 효과로 밀어낼 RC 카 오브젝트

    public void BoomOn(Vector3 _ballPosition)   // 골 효과로 주위 RC 카 밀어내는 함수
    {
        transform.Translate(_ballPosition);   // 이 오브젝트 위치를 골이 들어간 곳으로 이동시킨다

        boomCd = Physics.OverlapSphere(transform.position, 100f);   // 이 오브젝트 위치를 중심으로 100f 반지름 안에 있는 오브젝트들을 찾음
        for (int i = 0; i < boomCd.Length; i++)   // 찾은 오브젝트 수 만큼 for 문을 작동시킴
        {
            if (boomCd[i].tag == ("Car"))   // 오브젝트 태그가 "Car" 인 경우에만 아래 기능을 작동
            {
                car = boomCd[i].gameObject;   // 태그로 찾은 오브젝트를 car 오브젝트로 저장
                car.GetComponent<CarTest>().GoalEffect(_ballPosition);   // car 오브젝트의 "GoalEffect" 함수를 골인한 위치와 함께 참조
            }
        }
    }
}
