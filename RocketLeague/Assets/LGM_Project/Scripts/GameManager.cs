using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance
    {
        get
        {
               // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                   // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }
               // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    public static GameManager m_instance;   // 싱글톤이 할당될 static 변수

    public GameObject ballPrefab;   // 축구공을 생성할 축구공 프리팹
    public GameObject ballAuraPf;   // 축구공 표식을 생성할 축구공 표식 프리팹
    public GameObject blueCar;   // 블루팀 RC 카 생성할 프리팹
    public GameObject orangeCar;   // 오렌지팀 RC 카 생성할 프리팹
    public Transform ballSpawnTransform;   // 축구공을 생성할 구역
    public Transform[] blueCarSpawner;   // 블루팀 RC 카를 생성할 구역
    public Transform[] orangeCarSpawner;   // 오렌지팀 RC 카를 생성할 구역
    public TMP_Text blueScoreText;   // 블루팀 스코어 텍스트
    public TMP_Text orangeScoreText;   // 오렌지팀 스코어 텍스트
    public TMP_Text currentTimerText;

    public int blueScore;   // 블루팀 골 스코어
    public int orangeScore;   // 오렌지팀 골 스코어
    public bool isGoaled = false;   // 현재 골 성공 상태인지 체크

    private GameObject ballOj;   // 축구공 오브젝트
    private Rigidbody ballRb;   // 축구공 리짓바디
    private Transform blueSpawnPoint;   // 플레이어가 RC 카를 생성할 블루팀 포인트 구역
    private Transform orangeSpawnPoint;   // 플레이어가 RC 카를 생성할 오렌지팀 포인트 구역

    private int playerCount = default;   // 룸에 참여중인 플레이어 수

    void Awake()
    {
        playerCount = PhotonNetwork.PlayerList.Length;   // 포톤 서버에 접속한 플레이어 수만큼 플레이어 수로 지정해준다
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
               // 마스터 클라이언트 일시 축구공 프리팹을 불러와 축구공 오브젝트를 생성한다
            ballOj = PhotonNetwork.Instantiate(ballPrefab.name, ballSpawnTransform.position, Quaternion.identity);
               // 축구공 표식 오브젝트도 프리팹을 불러와 오브젝트를 생성한다
            PhotonNetwork.Instantiate(ballAuraPf.name, new Vector3(0f, 1.6f, 0f), Quaternion.Euler(90f, 0f, 0f));

            ballRb = ballOj.GetComponent<Rigidbody>();   // 축구공 리짓바디를 변수로 저장한다
        }

        if (playerCount % 2 == 0)
        {
            if (playerCount == 2)
            {
                blueSpawnPoint = blueCarSpawner[0];
            }
            else if (playerCount == 4)
            {
                blueSpawnPoint = blueCarSpawner[1];
            }
            else if (playerCount == 6)
            {
                blueSpawnPoint = blueCarSpawner[2];
            }
            // 블루 카 생성
            //PhotonNetwork.Instantiate(blueCar.name, blueSpawnPoint.position, blueSpawnPoint.rotation);
            CustomizingManager_Choi.instance.CreateObjectWithCustomizing(0);
        }
        else
        {
            if (playerCount == 1)
            {
                orangeSpawnPoint = orangeCarSpawner[0];
            }
            else if (playerCount == 3)
            {
                orangeSpawnPoint = orangeCarSpawner[1];
            }
            else if (playerCount == 5)
            {
                orangeSpawnPoint = orangeCarSpawner[2];
            }
            // 오렌지 카 생성
            //PhotonNetwork.Instantiate(orangeCar.name, orangeSpawnPoint.position, orangeSpawnPoint.rotation);
            CustomizingManager_Choi.instance.CreateObjectWithCustomizing(1);
        }

        playerCount = PhotonNetwork.PlayerList.Length;
    }

    void Update()
    {
        //* fix : score system *//
    }

    public void ResetGame()
    {
        ballOj.SetActive(false);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballOj.transform.position = ballSpawnTransform.position;

        StartCoroutine(ResetGameDelay());

        //* fix : goalEffect *//
    }

    IEnumerator ResetGameDelay()
    {
        yield return new WaitForSeconds(3f);

        ballOj.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //* Empty *//
    }

    [PunRPC]
    public void AddBlueScore()
    {
        blueScore += 1;
        photonView.RPC("UpdateScore", RpcTarget.All, blueScore, orangeScore);
    }

    [PunRPC]
    public void AddOrangeScore()
    {
        orangeScore += 1;
        photonView.RPC("UpdateScore", RpcTarget.All, blueScore, orangeScore);
    }

    public void OrangeScoreUp()
    {
        photonView.RPC("AddOrangeScore", RpcTarget.MasterClient);
    }

    public void BlueScoreUp()
    {
        photonView.RPC("AddBlueScore", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void UpdateScore(int _blueScore, int _orangeScore)
    {
        blueScore = _blueScore;
        orangeScore = _orangeScore;

        blueScoreText.text = string.Format("{0}", blueScore);
        orangeScoreText.text = string.Format("{0}", orangeScore);
    }

    [PunRPC]
    public void BallRespawn()
    {
        photonView.RPC("BallSpawn", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void BallSpawn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(ballPrefab.name, ballSpawnTransform.position, Quaternion.identity);
        }
    }

    // 주기적으로 자동 실행되는, 동기화 메서드
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    // 로컬 오브젝트라면 쓰기 부분이 실행됨
    //    if (stream.IsWriting)
    //    {
    //        // 네트워크를 통해 score 값을 보내기
    //        stream.SendNext(teamCount[0]);
    //        stream.SendNext(teamCount[1]);
    //    }
    //    else
    //    {
    //        // 리모트 오브젝트라면 읽기 부분이 실행됨

    //        // 네트워크를 통해 score 값 받기
    //        teamCount[0] = (int)stream.ReceiveNext();
    //        teamCount[1] = (int)stream.ReceiveNext();
    //        // 동기화하여 받은 점수를 UI로 표시

    //        teamCountText[0].text = string.Format("{0} / {1}", teamCount[0], maxTeamCount);   // 증가시킨 값을 출력
    //        teamCountText[1].text = string.Format("{0} / {1}", teamCount[1], maxTeamCount);

    //        AfterSerializeView();
    //    }
    //}
}
