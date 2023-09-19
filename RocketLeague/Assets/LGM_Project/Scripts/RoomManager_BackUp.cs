using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RoomManager_BackUp : MonoBehaviourPunCallbacks, IPunObservable, IInRoomCallbacks   // 룸 정보 콜백 기능 헤더
{
    private PhotonView pv;
    private Text playerCount;   // 게임 룸에 접속한 플레이어 수 표시
    private Button[] roomButton = new Button[4];   // 게임 룸에 활성화 되어있는 버튼을 배열로 저장
    private TextMeshProUGUI[] teamCountText = new TextMeshProUGUI[2];   // 팀 지정된 플레이어 표시 텍스트 저장
    private Text infoText;   // 룸 상태 정보 표시 텍스트 저장
    private GameValue gameValue;

    private int currPlayer = default;   // 현재 게임 룸에 존재하는 플레이어 수
    private int maxPlayer = default;   // 현재 게임 룸의 최대 플레이어 수
    private int[] teamCount = new int[2];   // 현재 팀으로 배정된 플레이어 숫자
    private int maxTeamCount = default;   // 팀으로 배정할 수 있는 최대 플레이어 수
    private int randomSelect = default;   // 자동으로 팀 선택할 때 랜덤값
    private int teamCheck = default;
    private bool isSelectTeam = false;
    private bool isGameStart = false;

    void Awake()
    {
        maxTeamCount = PhotonNetwork.CurrentRoom.MaxPlayers / 2;   // 팀으로 배정할 수 있는 최대 플레이어 수는 룸의 최대 플레이어 숫자에 2 를 나누어준다
        randomSelect = 0;
        teamCheck = 0;
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        playerCount = GameObject.Find("Text_PlayerCount").GetComponent<Text>();
        roomButton[0] = GameObject.Find("Button_1").GetComponent<Button>();
        roomButton[1] = GameObject.Find("Button_2").GetComponent<Button>();
        roomButton[2] = GameObject.Find("Button_3").GetComponent<Button>();
        roomButton[3] = GameObject.Find("Button_4").GetComponent<Button>();
        roomButton[0].onClick.AddListener(JoinBlueTeam);
        roomButton[1].onClick.AddListener(JoinRandomTeam);
        roomButton[2].onClick.AddListener(JoinOrangeTeam);
        roomButton[3].onClick.AddListener(ExitRoom);
        teamCountText[0] = GameObject.Find("Text_BlueCount").GetComponent<TextMeshProUGUI>();
        teamCountText[1] = GameObject.Find("Text_OrangeCount").GetComponent<TextMeshProUGUI>();
        infoText = GameObject.Find("Text_Info").GetComponent<Text>();
        gameValue = GameObject.Find("GameValue").GetComponent<GameValue>();

        Invoke("CheckPlayerCount", 0.5f);   // 게임 룸 접속 시 0.5 초 후에 룸 정보를 갱신해준다
    }

    void Update()
    {
        if (isGameStart == true) { return; }

        if (teamCount[0] == maxTeamCount && teamCount[1] == maxTeamCount)
        {
            isGameStart = true;
            StartCoroutine(GameStart());
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)   // 새로운 플레이어가 룸에 접속할 때 마다 룸 정보를 갱신해준다
    {
        CheckPlayerCount();   // 룸 정보 갱신 함수
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)   // 플레이어가 룸에서 나갈 때 마다 룸 정보를 갱신해준다
    {
        CheckPlayerCount();   // 룸 정보 갱신 함수
    }

    public void CheckPlayerCount()   // 룸 정보 갱신 함수
    {
        currPlayer = PhotonNetwork.PlayerList.Length;   // 현재 게임 룸에 접속중인 플레이어 수
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;   // 현재 게임 룸의 최대 플레이어 수
        playerCount.text = string.Format("Player : {0} / {1}", currPlayer, maxPlayer);
    }

    public void ExitRoom()   // 게임 룸에서 나갈 때 실행되는 함수
    {
        PhotonNetwork.LeaveRoom();   // Room 씬을 떠나는 함수 실행
        SceneManager.LoadScene("SampleScene");   // 로비 씬을 로드한다
        Invoke("CheckPlayerCount", 0.5f);   // 게임 룸을 나갈 시 0.5 초 후에 룸 정보를 갱신해준다
    }

    public void JoinBlueTeam()   // 블루에 팀 가입 버튼을 누를 때 실행
    {
        if (pv.IsMine == true)
        {
            if (teamCount[0] >= maxTeamCount) { return; }   // 해당 팀의 이미 합류한 인원이 최대 인원이면 return 실행
            if (teamCheck == 1) { return; }

            pv.RPC("FormatBlueTeamCount", RpcTarget.AllBuffered, teamCount[0], teamCount[1]);
        }
    }

    public void JoinOrangeTeam()   // 오렌지 팀 가입 버튼을 누를 때 실행
    {
        if (pv.IsMine == true)
        {
            if (teamCount[1] >= maxTeamCount) { return; }   // 해당 팀의 이미 합류한 인원이 최대 인원이면 return 실행
            if (teamCheck == 2) { return; }

            pv.RPC("FormatOrangeTeamCount", RpcTarget.AllBuffered, teamCount[0], teamCount[1]);
        }
    }

    public void JoinRandomTeam()   // 자동 팀 가입 버튼을 누를 때 실행
    {
        if (pv.IsMine == true)
        {
            randomSelect = Random.Range(0, 2);   // int 0, 1 중의 랜덤값을 생성

            if (teamCount[randomSelect] < maxTeamCount)   // 자동으로 선택한 팀의 플레이어 수가 팀의 최대 수인지 판단
            {
                if (randomSelect == 0) { JoinBlueTeam(); }   // 최대 수 보다 작으면 랜덤 값만큼 팀 선택 실행
                else if (randomSelect == 1) { JoinOrangeTeam(); }
            }
            else
            {
                ChangeRandom();   // 자동으로 선택한 팀의 플레이어 수가 팀의 최대 수 일때 실행
            }
        }
    }

    public void ChangeRandom()   // 자동으로 선택한 팀의 플레이어 수가 팀의 최대 수 일때 실행
    {
        if (randomSelect == 0)
        {
            if (teamCount[1] < maxTeamCount) { JoinOrangeTeam(); }   // 자동으로 선택한 팀이 아닌 다른 팀을 선택함
        }
        else if (randomSelect == 1)
        {
            if (teamCount[0] < maxTeamCount) { JoinBlueTeam(); }   // 자동으로 선택한 팀이 아닌 다른 팀을 선택함
        }
    }

    [PunRPC]
    public void FormatBlueTeamCount(int teamCount1, int teamCount2)
    {
        teamCount[0] = teamCount1;
        teamCount[1] = teamCount2;

        teamCount[0] += 1;   // 팀에 합류한 인원을 1 증가시킨다
        if (teamCheck == 2) { teamCount[1] -= 1; }

        teamCheck = 1;
        gameValue.GetComponent<GameValue>().playerTeamCheck = 1;

        infoText.text = string.Format("{0}, {1}, {2}", teamCheck, teamCount[0], teamCount[1]);

        teamCountText[0].text = string.Format("{0} / {1}", teamCount[0], maxTeamCount);   // 증가시킨 값을 출력
        teamCountText[1].text = string.Format("{0} / {1}", teamCount[1], maxTeamCount);
    }

    [PunRPC]
    public void FormatOrangeTeamCount(int teamCount1, int teamCount2)
    {
        teamCount[0] = teamCount1;
        teamCount[1] = teamCount2;

        teamCount[1] += 1;   // 팀에 합류한 인원을 1 증가시킨다
        if (teamCheck == 1) { teamCount[0] -= 1; }

        teamCheck = 2;
        gameValue.GetComponent<GameValue>().playerTeamCheck = 2;

        infoText.text = string.Format("{0}, {1}, {2}", teamCheck, teamCount[0], teamCount[1]);

        teamCountText[0].text = string.Format("{0} / {1}", teamCount[0], maxTeamCount);   // 증가시킨 값을 출력
        teamCountText[1].text = string.Format("{0} / {1}", teamCount[1], maxTeamCount);
    }

    IEnumerator GameStart()
    {
        infoText.text = string.Format("잠시 후 게임이 시작됩니다");

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 3; i++)
        {
            infoText.text = string.Format("{0} 초 후 게임이 시작됩니다", 3 - i);

            yield return new WaitForSeconds(1f);
        }

        infoText.text = string.Format("게임 시작");
        PhotonNetwork.LoadLevel("StandardScene");   // 메인 게임 씬 로드
    }

    public void TestGame()
    {
        PhotonNetwork.LoadLevel("StandardScene");   // 메인 게임 씬 로드
    }

    // 주기적으로 자동 실행되는, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 score 값을 보내기
            stream.SendNext(teamCount[0]);
            stream.SendNext(teamCount[1]);
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨

            // 네트워크를 통해 score 값 받기
            teamCount[0] = (int)stream.ReceiveNext();
            teamCount[1] = (int)stream.ReceiveNext();
            // 동기화하여 받은 점수를 UI로 표시

            teamCountText[0].text = string.Format("{0} / {1}", teamCount[0], maxTeamCount);   // 증가시킨 값을 출력
            teamCountText[1].text = string.Format("{0} / {1}", teamCount[1], maxTeamCount);

            AfterSerializeView();
        }
    }

    public void AfterSerializeView()
    {
        infoText.text = string.Format("{0}, {1}, {2}", teamCheck, teamCount[0], teamCount[1]);
    }
}
