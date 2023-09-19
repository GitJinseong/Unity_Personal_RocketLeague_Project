using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallAura : MonoBehaviourPun
{
    Vector3 ballPosition;   // 축구공 위치 Vecter3 위치값
    Vector3 ballAuraSize;

    private GameObject ballObj;   // 축구공 오브젝트
    public GameObject ballAura;
    public GameObject ballAuraInlineObj;   // 축구공 위치 표식중 안쪽 표식 오브젝트

    private float ballY = default;   // 축구공 Y 위치값
    private float ballYRes = default;   // 축구공 Y 위치 백분율 계산값
    private float auraSize = default;   // 축구공 위치 표식 Scale 백분율 계산값
    private bool startDelayTime = false;

    void Awake()
    {
        // 초기 변수값 설정
        ballObj = GameObject.Find("Ball(Clone)").GetComponent<GameObject>();
        //ballAura = GetComponent<GameObject>();
        //ballAuraInlineObj = GetComponent<GameObject>();

        ballY = 0f;
        ballYRes = 0f;
        auraSize = 0f;
        // end 초기 변수값 설정
    }

    void Update()
    {
        // 현재 축구공 위치 Vecter3 위치값
        ballPosition = new Vector3(ballObj.transform.position.x, ballAuraInlineObj.transform.position.y, ballObj.transform.position.z);
        transform.position = ballPosition;   // 축구공 표식을 축구공 X, Z 위치로 이동시킴

        ballY = ballObj.transform.position.y - 4f;   // Y 최소값이 4f 정도이므로 축구공의 Y 위치값에서 4f 를 빼준다
        ballYRes = (ballY / (40f / 100f));   // 축구공 Y 위치값을 백분율로 구한다

        if (ballYRes > 99f) { ballYRes = 99f; }   // 축구공 Y 위치값의 최대 한계값을 설정
        if (ballYRes < 0f) { ballYRes = 0f; }   // 축구공 Y 위치값의 최소 한계값을 설정

        ballYRes = 100f - ballYRes;   // 축구공 표식은 최대값에서 최소값으로 백분율이 반대이므로 100f 에서 축구공 Y 백분율을 빼준다
        auraSize = (6f / 100f) * ballYRes;   // 축구공 Y 위치 백분율만큼 축구공 표식 Scale 백분율을 구한다

        ballAuraSize = new Vector3(auraSize, auraSize, 1f);
        ballAuraInlineObj.transform.localScale = ballAuraSize;

        //FixAuraSize();   // 축구공 표식 스케일 PunRPC 동기화 실행
    }

    //[PunRPC]
    //public void FixAuraSize()   // 축구공 표식 스케일 PunRPC 동기화 실행
    //{
    //    if (PhotonNetwork.IsMasterClient)   // 마스터 클라이언트가 축구공 표식 스케일을 연산한다
    //    {
    //        ballAuraInlineObj.transform.localScale = new Vector3(auraSize, auraSize, 1f);   // 축구공 Y 위치 백분율만큼 축구공 표식 Scale 값을 변경해준다

    //        photonView.RPC("FixAuraSize", RpcTarget.Others, auraSize);   // 다른 로컬들에게 축구공 표식 스케일 연산한 값을 전달한다
    //    }
    //}
}
