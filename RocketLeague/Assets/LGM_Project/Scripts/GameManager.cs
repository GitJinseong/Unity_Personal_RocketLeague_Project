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
    public Transform[] blueCarSpawner = new Transform[4];   // 블루팀 RC 카를 생성할 구역
    public Transform[] orangeCarSpawner = new Transform[4];   // 오렌지팀 RC 카를 생성할 구역
    public TMP_Text blueScoreText;   // 블루팀 스코어 텍스트
    public TMP_Text orangeScoreText;   // 오렌지팀 스코어 텍스트
    public TMP_Text currentTimerText;   // 게임 시간 텍스트
    public TMP_Text gameReadyText;
    public TMP_Text gameStartCountText;
    public Image gameReadyImage;

    public int blueScore;   // 블루팀 골 스코어
    public int orangeScore;   // 오렌지팀 골 스코어
    public bool isGoaled = false;   // 현재 골 성공 상태인지 체크
    public bool timePassCheck = true;   // 게임 시간이 흐를수 있는 상태인지 체크
    public bool gameStartCheck = false;

    private GameObject ballOj;   // 축구공 오브젝트
    private GameObject playerCloneCar;   // RC 카 Empty 부모 오브젝트
    private Transform playerCar;   // RC 카 콜라이더와 리짓바디가 존재하는 자식 오브젝트
    private Rigidbody ballRb;   // 축구공 리짓바디
    private Rigidbody carRb;   // RC 카 리짓바디
    private Transform blueSpawnPoint;   // 플레이어가 RC 카를 생성할 블루팀 포인트 구역
    private Transform orangeSpawnPoint;   // 플레이어가 RC 카를 생성할 오렌지팀 포인트 구역
    private int playerCount = default;   // 룸에 참여중인 플레이어 수
    private int totalTime = default;   // 남은 게임 총 시간
    private int minuteTime = default;   // 표시할 분 시간
    private int secondTime = default;   // 표시할 초 시간
    private int playerOwnerNumber = default;   // photon 에서 부여한 유저 번호
    private int playerTeamCheck = default;   // 플레이어 팀 확인값
    private int gameStartCount = default;
    private float checkTime = default;   // 1 초를 체크할 실수값
    private bool fullPlayerCheck = false;

    void Awake()
    {
           // 초기 변수값 설정
        playerCount = PhotonNetwork.PlayerList.Length;   // 포톤 서버에 접속한 플레이어 수만큼 플레이어 수로 지정해준다

        totalTime = 300;
        minuteTime = 0;
        secondTime = 0;
        checkTime = 0f;
        playerTeamCheck = 0;
        playerOwnerNumber = 0;
        gameStartCount = 4;
           // end 초기 변수값 설정
    }

    void Start()
    {
        // 내 포톤뷰 ActorNumber을 찾아서 저장하는 변수
        int myPhotonActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트 일시 축구공 프리팹을 불러와 축구공 오브젝트를 생성한다
            ballOj = PhotonNetwork.Instantiate(ballPrefab.name, ballSpawnTransform.position, Quaternion.identity);
            // 축구공 표식 오브젝트도 프리팹을 불러와 오브젝트를 생성한다
            PhotonNetwork.Instantiate(ballAuraPf.name, new Vector3(0f, 1.6f, 0f), Quaternion.Euler(90f, 0f, 0f));

            ballRb = ballOj.GetComponent<Rigidbody>();   // 축구공 리짓바디를 변수로 저장한다
        }

        if (photonView.IsMine)
        {
            playerOwnerNumber = photonView.Owner.ActorNumber;   // 플레이어마다 photon 번호를 찾아 저장한다
        }

        if (playerCount % 2 == 0)   // 플레이어 카운트 값을 2로 나누었을때 나머지가 0 이 되는 순서의 플레이어인지 체크
        {
            if (playerCount == 2)   // 플레이어 카운트 값이 2 인 플레이어의 스폰 포인트 위치 정해주기
            {
                blueSpawnPoint = blueCarSpawner[0];
            }
            else if (playerCount == 4)   // 플레이어 카운트 값이 4 인 플레이어의 스폰 포인트 위치 정해주기
            {
                blueSpawnPoint = blueCarSpawner[1];
            }
            else if (playerCount == 6)   // 플레이어 카운트 값이 6 인 플레이어의 스폰 포인트 위치 정해주기
            {
                blueSpawnPoint = blueCarSpawner[2];
            }

            playerTeamCheck = 2;   // 플레이어 팀 구분을 블루팀으로 저장해준다
            //   // 해당 플레이어의 스폰 위치에 블루팀의 자동차를 생성하고 게임 오브젝트로 저장한다
            //playerCloneCar = PhotonNetwork.Instantiate(blueCar.name, blueSpawnPoint.position, blueSpawnPoint.rotation);
            //   // 플레이어 RC 카 안에 Transform 값을 저장한다
            //playerCar = playerCloneCar.transform.Find("Collider").GetComponent<Transform>();
            //   // 플레이어 RC 카 안에 Rigidbody 값을 저장한다
            //carRb = playerCloneCar.transform.Find("Collider").GetComponent<Rigidbody>();





            // 블루 카 생성 & 포톤 ActorNumber를 매개변수로 보냄
            // 생성시 SetRespawnObjectValues()를 호출하게 함
            CustomizingManager_Choi.instance.CreateObjectWithCustomizing(0, myPhotonActorNumber, 
                blueSpawnPoint.position, Quaternion.Euler(0f, -180f, 0f));
        }
        else   // 플레이어 카운트 값을 2로 나누었을때 나머지가 0 이 안되는 순서의 플레이어인지 체크
        {
            if (playerCount == 1)   // 플레이어 카운트 값이 1 인 플레이어의 스폰 포인트 위치 정해주기
            {
                orangeSpawnPoint = orangeCarSpawner[0];
            }
            else if (playerCount == 3)   // 플레이어 카운트 값이 3 인 플레이어의 스폰 포인트 위치 정해주기
            {
                orangeSpawnPoint = orangeCarSpawner[1];
            }
            else if (playerCount == 5)   // 플레이어 카운트 값이 5 인 플레이어의 스폰 포인트 위치 정해주기
            {
                orangeSpawnPoint = orangeCarSpawner[2];
            }

            playerTeamCheck = 1;   // 플레이어 팀 구분을 오렌지팀으로 저장해준다
            //   // 해당 플레이어의 스폰 위치에 오렌지팀의 자동차를 생성하고 게임 오브젝트로 저장한다
            //playerCloneCar = PhotonNetwork.Instantiate(orangeCar.name, orangeSpawnPoint.position, orangeSpawnPoint.rotation);
            //   // 플레이어 RC 카 안에 Transform 값을 저장한다
            //playerCar = playerCloneCar.transform.Find("Collider").GetComponent<Transform>();
            //   // 플레이어 RC 카 안에 Rigidbody 값을 저장한다
            //carRb = playerCloneCar.transform.Find("Collider").GetComponent<Rigidbody>();






            // 오렌지 카 생성 & 포톤 ActorNumber를 매개변수로 보냄
            // 생성시 SetRespawnObjectValues()를 호출하게 함
            CustomizingManager_Choi.instance.CreateObjectWithCustomizing(1, myPhotonActorNumber, 
                orangeSpawnPoint.position, Quaternion.identity);
        }

        playerCount = PhotonNetwork.PlayerList.Length;   // 포톤 서버에 접속한 플레이어 수를 다시 체크해준다
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)   // 마스터 클라이언트만 우선 게임시간을 계산한다
        {
            if (gameStartCheck == false)
            {
                photonView.RPC("MasterGameStartCheck", RpcTarget.MasterClient);
            }

            if (timePassCheck == true)   // 게임 시간이 흐를 수 있는 조건이면 실행
            {
                photonView.RPC("TimePassMaster", RpcTarget.MasterClient);   // 마스터 클라이언트만 게임 시간을 계산하는 함수 실행
            }
        }
    }

    // 리스폰 관련 오브젝트 정보를 저장하는 함수
    public void SetRespawnObjectValues(GameObject playerObj)
    {
        // 플레이어 RC 카 안에 Rigidbody 값을 저장한다
        playerCar = playerObj.transform.Find("Collider").GetComponent<Transform>();
        // 플레이어 RC 카 안에 Rigidbody 값을 저장한다
        carRb = playerObj.transform.Find("Collider").GetComponent<Rigidbody>();
    }

    [PunRPC]
    public void MasterGameStartCheck()
    {
        // 현재 디버그로 1명 이상일 경우 시작되게 설정
        if (PhotonNetwork.PlayerList.Length >= 1)
        {
            gameReadyImage.enabled = false;
            gameReadyText.enabled = false;
            gameStartCountText.gameObject.SetActive(true);
            photonView.RPC("ApplyEnabledSet", RpcTarget.AllBuffered, false, true);

            if (fullPlayerCheck == false)
            {
                fullPlayerCheck = true;

                StartCoroutine(StartCountDelay());
            }
        }
        else
        {
            playerCount = PhotonNetwork.PlayerList.Length;
            // 현재 디버그로 1명 이상일 경우 시작되게 설정
            photonView.RPC("ApplyGameStartCheck", RpcTarget.AllBuffered, playerCount, 1);
        }
    }

    [PunRPC]
    public void ApplyEnabledSet(bool state1, bool state2)
    {
        gameReadyImage.enabled = state1;
        gameReadyText.enabled = state1;
        gameStartCountText.gameObject.SetActive(state2);
    }

    [PunRPC]
    public void MasterStartCount()
    {
        gameStartCount -= 1;

        if (gameStartCount > 0)
        {
            photonView.RPC("ApplyStartCount", RpcTarget.AllBuffered, gameStartCount);
        }
        else
        {
            gameStartCheck = true;
            photonView.RPC("ApplyStartCountEnd", RpcTarget.AllBuffered, true);

            StartCoroutine(GameStartEnd());
        }
    }

    [PunRPC]
    public void ApplyStartCountEnd(bool state)
    {
        gameStartCheck = state;
        gameStartCountText.text = string.Format("Start!");
    }

    IEnumerator GameStartEnd()
    {
        yield return new WaitForSeconds(1f);

        gameStartCountText.gameObject.SetActive(false);
        photonView.RPC("ApplyGameStartEnd", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    public void ApplyGameStartEnd(bool state)
    {
        gameStartCountText.gameObject.SetActive(state);
    }

    [PunRPC]
    public void ApplyStartCount(int count)
    {
        gameStartCount = count;
        gameStartCountText.text = string.Format("{0}", gameStartCount);
    }

    IEnumerator StartCountDelay()
    {
        while (gameStartCount > 0)
        {
            yield return new WaitForSeconds(1.0f);

            photonView.RPC("MasterStartCount", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    public void ApplyGameStartCheck(int count, int maxCount)
    {
        playerCount = count;
        gameReadyText.text = string.Format("참가 중인 플레이어 : {0} / {1}", playerCount, maxCount);
    }

    [PunRPC]
    public void TimePassMaster()   // 게임 시간을 계산하는 함수
    {
        checkTime += Time.deltaTime;   // 매 프레임마다 델타타임을 증가시킨다
        if (checkTime >= 1f)   // 델타타임을 더한 checkTime 값이 1 보다 크거나 같으면 실행
        {
            checkTime = 0f;   // checkTime 을 초기화 시킨다
            totalTime -= 1;   // 총 게임시간에서 1 초를 감소시킨다
            minuteTime = totalTime / 60;   // 남은 게임 시간 중 분 타임을 나타낸다
            secondTime = totalTime % 60;   // 남은 게임 시간 중 초 타임을 나타낸다

               // 계산된 게임 시간을 모든 클라이언트에게 동기화 한다
            photonView.RPC("ApplyTimePass", RpcTarget.AllBuffered, minuteTime, secondTime);
        }
    }

    [PunRPC]
    public void ApplyTimePass(int min, int sec)   // 모든 클라이언트의 게임 시간을 동기화 하는 함수
    {
        minuteTime = min;
        secondTime = sec;

        currentTimerText.text = string.Format("{0} : {1:00}", minuteTime, secondTime);
    }

    public void GameTimePassOn()   // 다시 게임 시간이 흘러가게 하는 함수
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("GameTimePassOnMaster", RpcTarget.MasterClient);
        }
    }
    
    [PunRPC]
    public void GameTimePassOnMaster()   // 마스터 클라이언트만 true 값으로 변경해준다
    {
        timePassCheck = true;
        photonView.RPC("ApplyGameTimePassOn", RpcTarget.AllBuffered, true);
    }

    [PunRPC]
    public void ApplyGameTimePassOn(bool state)   // bool 값을 참조하여 모든 클라이언트의 값을 변경해준다
    {
        timePassCheck = state;
    }

    public void ResetGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            photonView.RPC("ResetGameMaster", RpcTarget.MasterClient);

            //* fix : goalEffect *//
        }
    }

    [PunRPC]
    public void ResetGameMaster()
    {
        ballOj.SetActive(false);
        timePassCheck = false;
        photonView.RPC("ApplyResetGame", RpcTarget.AllBuffered, false);
        photonView.RPC("ApplyTimePassOff", RpcTarget.AllBuffered, false);

        StartCoroutine(ResetGameDelay());
    }

    [PunRPC]
    public void ApplyResetGame(bool state)
    {
        ballOj.SetActive(state);
    }

    [PunRPC]
    public void ApplyTimePassOff(bool state)
    {
        timePassCheck = state;
    }

    IEnumerator ResetGameDelay()
    {
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.IsMasterClient)
        {
            ballOj.transform.position = ballSpawnTransform.transform.position;
            photonView.RPC("RespawnBall", RpcTarget.MasterClient);
            photonView.RPC("CarRespawn", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void CarRespawn()
    {
        if (playerTeamCheck == 1)
        {
            playerCar.position = orangeSpawnPoint.position;
            playerCar.rotation = orangeSpawnPoint.rotation;
            carRb.velocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
        }
        else
        {
            playerCar.position = blueSpawnPoint.position;
            playerCar.rotation = blueSpawnPoint.rotation;
            carRb.velocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
        }
    }

    [PunRPC]
    public void RespawnBall()   // 축구공 오브젝트 위치를 초기화 하는 함수
    {
        ballOj.SetActive(true);   // 축구공 오브젝트를 활성화
        isGoaled = false;   // 현재 골인 중인 상태를 해제
        photonView.RPC("ApplyResetGame", RpcTarget.AllBuffered, true);   // 모든 클라이언트의 축구공 상태를 활성화 동기화
        photonView.RPC("UpdateGoalCheck", RpcTarget.AllBuffered, false);   // 모든 클라이언트의 현재 골인 중인 상태를 해제 동기화
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //* Empty *//
    }

    public void BlueScoreUp()   // 블루팀이 골 성공 시 실행
    {
        if (PhotonNetwork.IsMasterClient)   // 마스터 클라이언트인지 체크
        {
            photonView.RPC("AddBlueScore", RpcTarget.MasterClient);   // 마스터 클라이언트에서 스코어 값이 증가하도록 해줌
        }
    }

    public void OrangeScoreUp()   // 오렌지팀이 골 성공 시 실행
    {
        if (PhotonNetwork.IsMasterClient)   // 마스터 클라이언트인지 체크
        {
            photonView.RPC("AddOrangeScore", RpcTarget.MasterClient);   // 마스터 클라이언트에서 스코어 값이 증가하도록 해줌
        }
    }

    [PunRPC]
    public void AddBlueScore()   // 블루팀 스코어를 1 증가시키는 함수
    {
        blueScore += 1;   // 블루팀 스코어 1 증가
                          // 스코어 값을 모든 클라이언트와 동기화 시켜줌
        photonView.RPC("UpdateScore", RpcTarget.AllBuffered, blueScore, orangeScore);
    }

    [PunRPC]
    public void AddOrangeScore()   // 오렌지팀 스코어를 1 증가시키는 함수
    {
        orangeScore += 1;   // 오렌지팀 스코어 1 증가
                            // 스코어 값을 모든 클라이언트와 동기화 시켜줌
        photonView.RPC("UpdateScore", RpcTarget.AllBuffered, blueScore, orangeScore);
    }

    [PunRPC]
    public void UpdateScore(int _blueScore, int _orangeScore)   // 스코어값을 모든 클라이언트가 동기화하는 함수
    {
        blueScore = _blueScore;   // 블루팀 스코어값을 동기화
        orangeScore = _orangeScore;   // 오렌지팀 스코어값을 동기화

        blueScoreText.text = string.Format("{0}", blueScore);   // 블루팀 스코어 텍스트 값을 변경해준다
        orangeScoreText.text = string.Format("{0}", orangeScore);   // 오렌지팀 스코어 텍스트 값을 변경해준다
    }

    public void GoalCheck()   // 골인 상태 체크값을 변경해줄때 실행하는 함수
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ChangeCheck", RpcTarget.MasterClient);   // 마스터 클라이언트가 isGoaled bool 변수값을 변경하도록 함
        }
    }

    [PunRPC]
    public void ChangeCheck()   // 마스터 클라이언트가 isGoaled 값을 변경하도록 함
    {
        isGoaled = true;   // isGoaled 값을 true 값으로 변경
                           // 모든 클라이언트가 isGoaled 값을 동기화 하도록 실행
        photonView.RPC("UpdateGoalCheck", RpcTarget.AllBuffered, true);
    }

    [PunRPC]
    public void UpdateGoalCheck(bool state)   // 모든 클라이언트가 isGoaled 값을 동기화 하는 함수
    {
        isGoaled = state;   // isGoaled 값을 모든 클라이언트가 동기화 함
        Debug.Log(state);
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
