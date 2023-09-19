using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Shoot : MonoBehaviourPun
{
    public NewCar car;
    public Transform carTransform;
    Vector3 dir;
    float carSpeed;
    Vector3 position;

    Vector3[] renderPos=new Vector3[2];
    private void Start()
    {
        
    }
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
            Vector3 dir = (collision.transform.position - position).normalized;
            Debug.Log(dir);
           
           

            PhotonView ballView = collision.gameObject.GetComponent<PhotonView>();
            if (ballView != null)
            {
                viewId_=ballView.ViewID;
                photonView.RPC("ShootForce", RpcTarget.MasterClient, viewId_, dir, carSpeed);

            }
        }
        //if (collision.collider.tag.Equals("Ball"))
        //{

        //    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        Vector3 dir = (collision.transform.position-transform.position).normalized;
        //        rb.AddForce(dir*car.currentSpeed*0.5f, ForceMode.Impulse);

        //    }
        //}
    }

    [PunRPC]
    private void ShootForce(int viewId_,Vector3 dir_,float speed_)
    {
        PhotonView targetView = PhotonView.Find(viewId_);
        if (targetView != null)
        {
            Debug.LogFormat("°øÀÇ photonview °¡Á®¿È ID : {0}", targetView.ViewID);
        }
        Rigidbody targetRigid = targetView.GetComponent<Rigidbody>();
      
        if (targetRigid!=null)
        {
          
            targetRigid.AddForce(dir_*speed_*0.3f, ForceMode.Impulse);
            Debug.Log("°øÀ» Ã¡´Ù.");
        }
        else
        {
            Debug.Log("TargetRigid : NULL");

        }

    }
}
