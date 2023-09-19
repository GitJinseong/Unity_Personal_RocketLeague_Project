using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball_Ys : MonoBehaviourPunCallbacks,IPunObservable
{
    Rigidbody rb;
    Vector3 networkPosition;
    Quaternion networkRotation;
    Vector3 velocity;
    Vector3 angularVelocity;
    // Start is called before the first frame update
    private void Awake()
    {
        rb=GetComponent<Rigidbody>();

    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
    private void OnCollisionEnter(Collision collision)
    {
      
        rb.useGravity = true;        

        
        StopAllCoroutines();
    }

    private void FixedUpdate()
    {
        //if (!photonView.IsMine)
        //{
        //    rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
        //    rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        //}
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
