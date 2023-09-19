using Photon.Pun;
using UnityEngine;

public class RoomManagerSpawn : MonoBehaviourPun
{
    public GameObject roomManagerPf;

    private GameObject roomManager;
    private Canvas roomCanvas;

    void Awake()
    {
        roomManager = PhotonNetwork.Instantiate(roomManagerPf.name, new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    void Start()
    {
        roomCanvas = roomManager.transform.Find("JoinedRoom_UI_Canvas").GetComponent<Canvas>();

        roomCanvas.gameObject.SetActive(true);
    }
}
