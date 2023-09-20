using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeGoal : MonoBehaviour
{
    public Collider goalLineCd;   // 골대 콜라이더
    public Collider pushCd;   // 골 성공 푸쉬 콜라이더

    private void OnTriggerEnter(Collider collision)   // 축구공이 골대 안 콜라이더에 들어오면 실행
    {
           // 콜라이더에 들어온 오브젝트가 "Ball" 태그이고, GameManager 의 isGoaled 값이 false 이면 골 성공
        if (collision.tag == "Ball")
        {
            //GameManager.instance.isGoaled = true;   // GameManager 의 isGoaled 값을 true 로 변경해 골을 넣은 상태로 바꿈
            pushCd.enabled = true;   // push 콜라이더를 활성화 시킨다
            GameManager.instance.BlueScoreUp();   // TestManager 의 score 를 더해주는 함수를 실행
            
            StartCoroutine(GoalPushDelay());   // 몇초 후 활성화 된 콜라이더를 다시 비활성화 시키는 함수
        }
    }

    IEnumerator GoalPushDelay()
    {
        yield return new WaitForSeconds(0.5f);

        pushCd.enabled = false;   // 활성화 되어 있는 콜라이더를 다시 비활성화 시킨다
    }
}
