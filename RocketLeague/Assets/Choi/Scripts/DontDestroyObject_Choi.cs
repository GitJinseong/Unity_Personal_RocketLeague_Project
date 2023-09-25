using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject_Choi : MonoBehaviour
{
    // DontDestroyObject_Choi 스크립트 컴포넌트를 상속받는
    // 오브젝트를 파괴하지 않도록 설정
    private void Awake()
    {
        // 해당 스크립트 컴포넌트를 상속받는 오브젝트를
        // 파괴하지 않도록 설정하는 함수 호출
        DontDestroyOnLoad(gameObject);
    }
}
