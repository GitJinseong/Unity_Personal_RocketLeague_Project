using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OrangeGoal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.instance.BlueScoreUp();
                PhotonNetwork.Destroy(other.gameObject);
                GameManager.instance.BallRespawn();
            }
        }
    }
}
