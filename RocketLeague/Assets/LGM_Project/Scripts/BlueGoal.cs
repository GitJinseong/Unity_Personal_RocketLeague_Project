using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlueGoal : MonoBehaviourPun
{
    public Collider goalLineCd;   // 골대 콜라이더
    public Collider pushCd;   // 골 성공 푸쉬 콜라이더

    private void OnTriggerEnter(Collider collision)   // 축구공이 골대 안 콜라이더에 들어오면 실행
    {
           // 콜라이더에 들어온 오브젝트가 "Ball" 태그이고, GameManager 의 isGoaled 값이 false 이면 골 성공
        if (collision.tag == "Ball" && GameManager.instance.isGoaled == false)
        {
            GameManager.instance.GoalCheck();   // 골 성공 시 골 중복을 막기 위해 게임매니저에 있는 GoalCheck 함수를 실행한다
            GameManager.instance.OrangeScoreUp();   // GameManager 의 score 를 더해주는 함수를 실행
            GameManager.instance.ResetGame();   // GameManager 에 있는 게임 라운드 리셋 함수를 실행
               // 마스터 클라이언트에게 푸쉬 콜라이더를 활성화 하도록 실행한다
            photonView.RPC("PushColliderOn", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    public void PushColliderOn()   // 푸쉬 콜라이더를 활성화 시켜주는 함수
    {
        pushCd.enabled = true;   // 푸쉬 콜라이더를 활성화
        photonView.RPC("ApplyPushCollider", RpcTarget.AllBuffered, true);   // 활성화 값을 모든 클라이언트와 동기화

        StartCoroutine(GoalPushDelay());   // 몇 초 후 활성화 된 콜라이더를 다시 비활성화 시키는 코루틴 함수
    }

    [PunRPC]
    public void PushColliderOff()   // 푸쉬 콜라이더를 비활성화 시켜주는 함수
    {
        pushCd.enabled = false;   // 푸쉬 콜라이더를 비활성화
        photonView.RPC("ApplyPushCollider", RpcTarget.AllBuffered, false);   // 푸쉬 콜라이더 비활성화를 모든 클라이언트와 동기화
    }

    [PunRPC]
    public void ApplyPushCollider(bool state)   // 푸쉬 콜라이더를 모든 클라이언트와 동기화 하는 함수
    {
        pushCd.enabled = state;   // 푸쉬 콜라이더를 모든 클라이언트와 동기화
    }

    IEnumerator GoalPushDelay()   // 몇 초 뒤 푸쉬 콜라이더를 비활성화 시켜주는 코루틴 함수
    {
        yield return new WaitForSeconds(0.5f);

           // 마스터 클라이언트에게 푸쉬 콜라이더 비활성화 기능 함수를 실행하도록 한다
        photonView.RPC("PushColliderOff", RpcTarget.MasterClient);
    }
}
