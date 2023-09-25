using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoosterEffect_Yoo : MonoBehaviourPun
{
    NewCar car;
    CarBooster_Yoo booster;
    public GameObject boostLevel1 { get; private set; }
    public GameObject boostLevel2 { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        car = GetComponent<NewCar>();
        booster = GetComponent<CarBooster_Yoo>();
        boostLevel1 = transform.GetChild(0).GetChild(0).Find("BoostLevel1").gameObject;
        boostLevel2 = transform.GetChild(0).GetChild(0).Find("BoostLevel2").gameObject;
        Debug.Log(boostLevel1.gameObject.name);
        Debug.Log(boostLevel2.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if(!PhotonNetwork.IsConnected)
        {
            Debug.Log("서버와 연결되지 않음");
            return;
        }

        // 부스터 사용중 아니고, 2단 부스터 사용중 아니고, 강제 부스팅 상태가 아니면 이펙트 끔
        if (booster.useBoost == false && car.useSecondBoost == false && car.outOfControl == false)
        {
            //photonView.RPC("TwoBoostOff", RpcTarget.All, boostLevel1, boostLevel2);

            //boostLevel1.SetActive(false);
            //boostLevel2.SetActive(false);
            //if (photonView.IsMine)
            //{
            //    photonView.RPC("TwoBoostOff", RpcTarget.Others, boostLevel1, boostLevel2);
            //}

            DoTwoBoostOff();
        }
        // 부스터 사용중 일때 1단 부스터 이펙트켬
        if (booster.useBoost == true)
        {
            //photonView.RPC("OneBoostOn", RpcTarget.All, boostLevel1);

            //boostLevel1.SetActive(true);
            //if (photonView.IsMine)
            //{
            //    photonView.RPC("OneBoostOn", RpcTarget.Others, boostLevel1);
            //}

            DoOneBoostOn();
        }
        // 강제 부스팅 상태 일때 1단,2단 부스터 이펙트켬
        if (car.outOfControl == true)
        {
            //photonView.RPC("TwoBoostOn", RpcTarget.All, boostLevel1, boostLevel2);

            //boostLevel1.SetActive(true);
            //boostLevel2.SetActive(true);
            //if (photonView.IsMine)
            //{
            //    photonView.RPC("TwoBoostOn", RpcTarget.Others, boostLevel1, boostLevel2);
            //}

            DoTwoBoostOn();
        }
        // 2단 부스터 사용중 일때 1단, 2단 부스터 이펙트켬
        if (car.useSecondBoost == true)
        {
            //photonView.RPC("TwoBoostOn", RpcTarget.All, boostLevel1, boostLevel2);

            //boostLevel1.SetActive(true);
            //boostLevel2.SetActive(true);
            //if (photonView.IsMine)
            //{
            //    photonView.RPC("TwoBoostOn", RpcTarget.Others, boostLevel1, boostLevel2);
            //}

            DoTwoBoostOn();
        }
    }

    [PunRPC]
    void OneBoostOn()
    {
        //boost1.SetActive(true);
        boostLevel1.SetActive(true);
    }

    [PunRPC]
    void TwoBoostOn()
    {
        //boost1.SetActive(true);
        //boost2.SetActive(true);
        boostLevel1.SetActive(true);
        boostLevel2.SetActive(true);
    }

    [PunRPC]
    void TwoBoostOff()
    {
        //boost1.SetActive(false); 
        //boost2.SetActive(false);
        boostLevel1.SetActive(false);
        boostLevel2.SetActive(false);
    }

    void DoOneBoostOn()
    {
        //boostLevel1.SetActive(true);
        if (photonView.IsMine)
        {
            photonView.RPC("OneBoostOn", RpcTarget.All);
        }
    }

    void DoTwoBoostOn()
    {
        //boostLevel1.SetActive(true);
        //boostLevel2.SetActive(true);
        if (photonView.IsMine)
        {
            photonView.RPC("TwoBoostOn", RpcTarget.All);
        }
    }

    void DoTwoBoostOff()
    {
        //photonView.RPC("TwoBoostOff", RpcTarget.All, boostLevel1, boostLevel2);

        //boostLevel1.SetActive(false);
        //boostLevel2.SetActive(false);
        if (photonView.IsMine)
        {
            photonView.RPC("TwoBoostOff", RpcTarget.All);
        }
    }
}
