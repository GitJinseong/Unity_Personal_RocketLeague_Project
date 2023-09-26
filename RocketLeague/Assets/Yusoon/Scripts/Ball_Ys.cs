using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball_Ys : MonoBehaviourPunCallbacks, IPunObservable
{
    Rigidbody rb;
    Vector3 networkPosition;
    Quaternion networkRotation;
    Vector3 velocity;
    Vector3 angularVelocity;
    public string blueteamName;
    public string orangeteamName;
    public int teamNumber;

    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
    }

    public void FreezeBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        StartCoroutine(FreezeCoolDown());
    }
    private IEnumerator FreezeCoolDown()
    {
        yield return new WaitForSeconds(3);
        rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)   // 축구공에 콜라이더가 충돌하면 실행
    {
        if (PhotonNetwork.IsMasterClient)   // 마스터 클라이언트에서만 실행한다
        {
            // 차 태그가 달려있는 오브젝트가 축구공 콜라이더와 충돌했는지 체크
            if (collision.gameObject.tag == ("Car_Blue") || collision.gameObject.tag == ("Car_Orange"))
            {
                if (GameManager.instance.timePassCheck == false)   // GameManager 에서 게임 시간이 멈춰있게 설정 되어있으면 실행
                {
                    // GameManager 의 게임 시간이 다시 흘러가게 하는 함수를 실행한다
                    GameManager.instance.GameTimePassOn();
                }
                else   // GameManager 에서 게임 시간이 흘러가게 설정 되어있으면 실행
                {
                    rb.useGravity = true;

                    StopAllCoroutines();
                }

                PhotonView targetView=collision.gameObject.GetComponent<PhotonView>();
                if(targetView != null) 
                {
                    if(targetView.Owner.ActorNumber%2==0)
                    {
                        //blueteamNameCheck
                        blueteamName=targetView.Owner.NickName;
                    }    
                    else
                    {
                        orangeteamName=targetView.Owner.NickName;
                    }
                }

               


            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �����͸� ���� �� Rigidbody�� ���¸� ����ȭ�մϴ�.
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            // �����͸� ���� �� Rigidbody�� ���¸� ������ȭ�մϴ�.
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();
        }
    }
}
