using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviourPunCallbacks,IPunObservable
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

    private static GameManager m_instance;   // 싱글톤이 할당될 static 변수

    public GameObject ballPrefab;
    public GameObject ballAuraPf;
    public GameObject ballObject;
    public GameObject ballAura;
    public GameObject blueCar;
    public GameObject orangeCar;
    public Rigidbody ballRb;
    public Transform ballSpawnTransform;
    public Transform[] blueCarSpawner;
    public Transform[] orangeCarSpawner;
    public Transform blueSpawnPoint;
    public Transform orangeSpawnPoint;

    public int blueSpawnCheck = default;
    public int orangeSpawnCheck = default;
    public int gameMaxPlayers = default;
    public int playerTeamCheck = default;
    public TMP_Text blueScoreText;
    public TMP_Text orangeScoreText;
    public TMP_Text currentTimerText;

    public int blueScore;
    public int orangeScore;

    public int playerCount = default;
    public bool isGoaled = false;
    public int[] score = new int[2];
 
    void Awake()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        score[0] = 0;
        score[1] = 0;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ballObject = PhotonNetwork.Instantiate(ballPrefab.name, ballSpawnTransform.position, Quaternion.identity);
            ballAura = PhotonNetwork.Instantiate(ballAuraPf.name, new Vector3(0f, 1.6f, 0f), Quaternion.Euler(90f, 0f, 0f));
            ballRb = ballObject.GetComponent<Rigidbody>();
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

            PhotonNetwork.Instantiate(blueCar.name, blueSpawnPoint.position, blueSpawnPoint.rotation);
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

            PhotonNetwork.Instantiate(orangeCar.name, orangeSpawnPoint.position, orangeSpawnPoint.rotation);
        }

        playerCount = PhotonNetwork.PlayerList.Length;
    }

    public void GoalTeam1(int newScore)   // 1팀이 2팀의 골대에 골을 넣었을때 실행
    {
        score[0] += newScore;   // 1팀의 score 에 1 점을 더해준다

        ResetGame();

        //ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //ball.GetComponent<Rigidbody>().rotation = Quaternion.identity;
        //ball.gameObject.SetActive(false);
        //StartCoroutine(GameResetWaitTime());   // 골을 넣은 후 잠시 딜레이 시간을 주는 함수를 실행한다
    }

    public void GoalTeam2(int newScore)   // 2팀이 1팀의 골대에 골을 넣었을때 실행
    {
        score[1] += newScore;   // 2팀의 score 에 1 점을 더해준다

        ResetGame();

        //ballObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //ballObject.GetComponent<Rigidbody>().rotation = Quaternion.identity;
        //ballObject.gameObject.SetActive(false);
        //StartCoroutine(GameResetWaitTime());   // 골을 넣은 후 잠시 딜레이 시간을 주는 함수를 실행한다
    }

    public void ResetGame()
    {
        ballObject.SetActive(false);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballObject.transform.position = ballSpawnTransform.position;

        StartCoroutine(ResetGameDelay());
        //* add : goalEffect *//
        
    }

    IEnumerator ResetGameDelay()
    {
        yield return new WaitForSeconds(3f);

        ballObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //* Empty *//
    }

    [PunRPC]
    public void AddBlueScore()
    {
        blueScore+=1;
    }

    [PunRPC]
    public void AddOrangeScore()
    {
        orangeScore+=1;
    }

    public void OrangeScoreUp()
    {
        photonView.RPC("AddOrangeScore", RpcTarget.AllBuffered);
    }

    public void BlueScoreUp()
    {
        photonView.RPC("AddBlueScore", RpcTarget.AllBuffered);

    }


  
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
