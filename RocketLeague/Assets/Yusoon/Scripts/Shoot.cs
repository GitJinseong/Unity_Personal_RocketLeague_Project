using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shoot : MonoBehaviourPun
{
   
    public NewCar car;
    public Transform carTransform;
    Vector3 dir;
    float carSpeed;
    Vector3 position;
    Rigidbody rb;
    private void Update()
    {
        carSpeed =car.currentSpeed;
        position=transform.position;
        position.y=0;
    }

    private void OnCollisionEnter(Collision collision)
    {

        int viewId_;
        if (collision.collider.tag.Equals("Ball"))
        {
           // Vector3 dir = (collision.transform.position-transform.position).normalized;
            //rb.AddForce(dir*car.currentSpeed*0.5f, ForceMode.Impulse);

            Vector3 dir = (collision.transform.position - position).normalized;
            //Debug.Log(dir);



            PhotonView ballView = collision.gameObject.GetComponent<PhotonView>();
            if (ballView != null)
            {
                viewId_=ballView.ViewID;
                photonView.RPC("ShootForce", RpcTarget.MasterClient, viewId_, dir, carSpeed);

            }
        }

    }

    [PunRPC]
    private void ShootForce(int viewId_, Vector3 dir_, float speed_)
    {
        PhotonView targetView = PhotonView.Find(viewId_);
        if (targetView != null)
        {
            Debug.LogFormat("���� photonview ������ ID : {0}", targetView.ViewID);
        }
        Rigidbody targetRigid = targetView.GetComponent<Rigidbody>();

        if (targetRigid!=null)
        {

            targetRigid.AddForce(dir_*speed_*0.3f, ForceMode.Impulse);
         
        }
        else
        {
            Debug.Log("TargetRigid : NULL");

        }

    }
}
