using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal2 : MonoBehaviour
{
    private Vector3 ballPosition = Vector3.zero;   // 콜라이더 안에 들어온 축구공 위치값 Vecter

    private Rigidbody goalLineRb;   // 골대 리짓바디
    private Collider goalLineCd;   // 골대 콜라이더
    private GameObject goalBoomObject;   // GoalBoomPusher 오브젝트 참조

    void Awake()
    {
           // 초기 변수값 설정
        goalBoomObject = GameObject.Find("GoalBoomPusher");

        goalLineRb = GetComponent<Rigidbody>();
        goalLineCd = GetComponent<Collider>();
           // end 초기 변수값 설정
    }

    private void OnTriggerEnter(Collider collision)   // 축구공이 골대 안 콜라이더에 들어오면 실행
    {
           // 콜라이더에 들어온 오브젝트가 "Ball" 태그이고, TestManager isGoal 값이 false 면 골 성공
        if (collision.tag == "Ball" && TestManager.instance.isGoal == false)
        {
            TestManager.instance.isGoal = true;   // TestManager isGoal 값을 true 로 변경해 골을 넣은 상태로 바꿈
            TestManager.instance.GoalTeam1();   // TestManager 의 score 를 더해주는 함수를 실행

               // 콜라이더에 들어온 축구공 위치값 Vector3 저장
            ballPosition = new Vector3(collision.transform.position.x, collision.transform.position.y,
                collision.transform.position.z);

            goalBoomObject.GetComponent<GoalBoomPush>().BoomOn(ballPosition);   // GoalBoomPusher 오브젝트로 축구공 위치값과 함께 참조
        }
    }
}
