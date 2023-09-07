using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static TestManager instance;   // instance 선언

    public int[] score = new int[2];   // score[0] : 1팀, score[1] : 2팀 스코어
    public bool isGoal = false;   // Goal 을 넣은 상태인지 아닌지 체크

    void Awake()
    {
        if (instance == null || instance == default) { instance = this; }   // TestManager instance 중복을 검사
        else { Destroy(this.gameObject); }
    }

    public void GoalTeam1()   // 1팀이 2팀의 골대에 골을 넣었을때 실행
    {
        score[0] += 1;   // 1팀의 score 에 1 점을 더해준다
    }

    public void GoalTeam2()   // 2팀이 1팀의 골대에 골을 넣었을때 실행
    {
        score[1] += 1;   // 2팀의 score 에 1 점을 더해준다
    }

}
