using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl_Choi : MonoBehaviour
{
    // 게임 시작시 마우스 커서를 숨김
    void Start()
    {
        // 마우스 커서를 비활성화 하는 함수 호출
        ToggleCursorVisible(false);
    }

    void Update()
    {
        // ESC 키를 눌렀을 경우 마우스 커서를 활성화
        // 하는 함수를 호출
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 마우스 커서를 토글하는 함수 호출
            ToggleCursorVisible(true);
        }

        // 마우스 왼쪽 버튼을 눌렀을 경우 마우스 커서를 비활성화
        // 하는 함수를 호출
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 커서를 토글하는 함수 호출
            ToggleCursorVisible(false);
        }
    }

    // 마우스 커서를 받은 매개변수로 토글하는 함수
    private void ToggleCursorVisible(bool isActive)
    {
        // 현재 마우스 커서 활성화 상태를
        // isActive로 변경
        Cursor.visible = isActive;
    }
}
