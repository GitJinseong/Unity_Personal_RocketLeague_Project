using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static TestManager instance;   // instance 선언

    Vector3 ballResetVector = Vector3.zero;   // 축구공 리셋 Vector 위치값

    public int[] score = new int[2];   // score[0] : 1팀, score[1] : 2팀 스코어
    public bool isGoal = false;   // Goal 을 넣은 상태인지 아닌지 체크

    private GameObject ball;   // 축구공 오브젝트
    private GameObject ballReposition;   // 축구공 리셋 위치 오브젝트

    void Awake()
    {
           // 초기 변수값 설정
        if (instance == null || instance == default) { instance = this; }   // TestManager instance 중복을 검사
        else { Destroy(this.gameObject); }

        ball = GameObject.Find("Ball");
        ballReposition = GameObject.Find("BallSpawn");
           // end 초기 변수값 설정
    }

    void Start()
    {
        ballResetVector = ballReposition.transform.position;   // 축구공 리셋 위치를 Vector 에 저장한다
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GoalTeam2();
        }
    }

    public void GoalTeam1()   // 1팀이 2팀의 골대에 골을 넣었을때 실행
    {
        score[0] += 1;   // 1팀의 score 에 1 점을 더해준다
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().rotation = Quaternion.identity;
        ball.gameObject.SetActive(false);
        StartCoroutine(GameResetWaitTime());   // 골을 넣은 후 잠시 딜레이 시간을 주는 함수를 실행한다
    }

    public void GoalTeam2()   // 2팀이 1팀의 골대에 골을 넣었을때 실행
    {
        score[1] += 1;   // 2팀의 score 에 1 점을 더해준다
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().rotation = Quaternion.identity;
        ball.gameObject.SetActive(false);
        StartCoroutine(GameResetWaitTime());   // 골을 넣은 후 잠시 딜레이 시간을 주는 함수를 실행한다
    }

    IEnumerator GameResetWaitTime()   // 골을 넣은 후 잠시 딜레이 시간을 주는 함수
    {
        Debug.Log("쿨타임 시작");
        yield return new WaitForSeconds(3f);   // 3초간 딜레이 시간 이후 GameReset() 함수를 실행한다
        GameReset();
    }

    public void GameReset()   // 게임이 다시 시작될 때 실행되는 함수
    {
        ball.gameObject.SetActive(true);
        ball.transform.position = ballResetVector;   // 축구공을 리셋 위치로 이동시킨다
        Debug.Log("축구공 리셋 완료");
    }
}
