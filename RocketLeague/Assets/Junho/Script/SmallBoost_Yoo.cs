using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBoost_Yoo : MonoBehaviourPun
{
    private CarBooster_Yoo carBooster;
    private Collider boostCollider;
    private float regenTime;
    private float timeAfterUse;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = Vector3.zero;
        boostCollider = GetComponent<Collider>();
        regenTime = 10f;
        if (PhotonNetwork.IsMasterClient)
        {
            DoScaleOff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 부스터의 스케일이 0이라면
        if (gameObject.transform.localScale == Vector3.zero)
        {
            // 사용후의 시간을 증가시킴
            timeAfterUse += Time.deltaTime;
            //Debug.LogFormat("작부 재생성까지 남은시간:" + (regenTime - timeAfterUse));
            // 사용후의 시간이 재생성 시간이상이면
            if(timeAfterUse >= regenTime)
            {
                // 부스터의 스케일을 2,2,2로 초기화 후 사용후의 시간 0으로 초기화
                DoScaleOn();

                //gameObject.transform.localScale = Vector3.one * 2;

                //boostCollider.enabled = true;
                //Debug.Log("작부 생성완료");
                timeAfterUse = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("여기들어옴?");
        // 자동차와 부딪힐 경우 부딪힌 자동차의 스크립트를 불러와서 부스터 게이지 증가함수를 발동시킴
        if (collision.CompareTag("Car_Orange")||collision.CompareTag("Car_Blue"))
        {
            //Debug.Log("여기는 들어옴?");
            if (collision.gameObject.transform.parent.gameObject.GetComponentInParent<CarBooster_Yoo>() != null)
            {
                carBooster = collision.gameObject.transform.parent.gameObject.GetComponentInParent<CarBooster_Yoo>();

                carBooster.AddBoost(12);

                if (PhotonNetwork.IsMasterClient)
                {
                    DoScaleOff();
                }

                //gameObject.transform.localScale = Vector3.zero;

                //boostCollider.enabled = false;
            }
        }
    }

    [PunRPC]
    void ScaleOff()
    {
        gameObject.transform.localScale = Vector3.zero;

        boostCollider.enabled = false;
    }

    [PunRPC]
    void ScaleOn()
    {
        gameObject.transform.localScale = Vector3.one * 2;

        boostCollider.enabled = true;
    }

    void DoScaleOn()
    {
        photonView.RPC("ScaleOn", RpcTarget.AllBuffered);
    }

    void DoScaleOff()
    {
        photonView.RPC("ScaleOff", RpcTarget.AllBuffered);
    }
}
