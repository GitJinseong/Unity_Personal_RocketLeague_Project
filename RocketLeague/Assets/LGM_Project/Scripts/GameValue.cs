using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameValue : MonoBehaviour
{
    public int playerTeamCheck = default;   // 플레이어가 어떤 팀을 선택했는지 체크
    public int gameMaxPlayer = default;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);   // 다른 씬이 로드 되어도 이 스크립트가 초기화 되지 않도록 해준다

        playerTeamCheck = 0;
        gameMaxPlayer = 0;
    }
}
