using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueGoal : MonoBehaviour
{
    //private Vector3 ballPosition = Vector3.zero;   // 콜라이더 안에 들어온 축구공 위치값 Vecter
    //private Rigidbody goalLineRb;   // 골대 리짓바디
    //private GameObject goalBoomObject;   // GoalBoomPusher 오브젝트 참조

    public Collider goalLineCd;   // 골대 콜라이더
    public Collider pushCd;   // 골 성공시 밀어내는 범위 콜라이더

    private GameObject onCarOj;
    private Transform onCarChild;
    private GameObject goalPost;
    private Rigidbody onCarRb;
    private Vector3 pushVector;

    void Awake()
    {
        goalPost = GameObject.Find("BlueGoal");
    }

    private void OnTriggerEnter(Collider collision)   // 축구공이 골대 안 콜라이더에 들어오면 실행
    {
           // 콜라이더에 들어온 오브젝트가 "Ball" 태그이고, GameManager 의 isGoaled 값이 false 이면 골 성공
        if (collision.tag == "Ball" && GameManager.instance.isGoaled == false)
        {
            GameManager.instance.isGoaled = true;   // GameManager 의 isGoaled 값을 true 로 변경해 골을 넣은 상태로 바꿈
            pushCd.enabled = true;
            GameManager.instance.GoalTeam2(1);   // TestManager 의 score 를 더해주는 함수를 실행

            Debug.Log("골 성공");

            StartCoroutine(GoalPushDelay());

            //   // 콜라이더에 들어온 축구공 위치값 Vector3 저장
            //ballPosition = new Vector3(collision.transform.position.x, collision.transform.position.y,
            //    collision.transform.position.z);

            //goalBoomObject.GetComponent<GoalBoomPush>().BoomOn(ballPosition);   // GoalBoomPusher 오브젝트로 축구공 위치값과 함께 참조
        }

        if ((collision.tag == "Car_Blue" || collision.tag == "Car_Orange") && GameManager.instance.isGoaled == true)
        {
            onCarOj = collision.gameObject;
            onCarChild = transform.Find("Collider");
            Debug.Log(onCarChild.name);
            onCarRb = onCarChild.GetComponent<Rigidbody>();
            Debug.Log(onCarRb.name);
            pushVector = collision.transform.position - goalPost.transform.position;   // 현재 차 위치에서 골인 지점 위치를 뺀다
            pushVector = pushVector.normalized;   // 계산된 위치값을 1 의 값으로 변경해준다
            pushVector += Vector3.up * 5;   // 계산된 위치값에 5 만큼 점프력을 증가시킨다
            pushVector += Vector3.forward * 5;   // 계산된 위치값에 5 만큼 직진값을 증가시킨다
            onCarRb.AddForce(pushVector * 20, ForceMode.Impulse);   // 골인 효과 범위내에 있는 차 들을 AddForce 로 힘을 줘 밀어낸다

            Debug.Log("푸쉬 성공");
        }
    }

    IEnumerator GoalPushDelay()
    {
        yield return new WaitForSeconds(0.5f);

        pushCd.enabled = false;
    }
}
