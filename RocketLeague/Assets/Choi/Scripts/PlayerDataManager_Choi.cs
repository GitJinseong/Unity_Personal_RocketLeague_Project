using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerDataManager_Choi : MonoBehaviour
{
    #region [싱글톤 & 변수 선언부]
    // ##################################################################################################
    // ▶[싱글톤 & 변수 선언부]
    // ##################################################################################################
        private static PlayerDataManager_Choi m_instance; // 싱글톤이 할당될 static 변수
        public static PlayerDataManager_Choi instance
        {
            get
            {
                // 만약 싱글톤 오브젝트에 할당이 되지 않았다면
                if (m_instance == null)
                {
                    // 씬에서 오브젝트를 찾아 할당
                    m_instance = FindObjectOfType<PlayerDataManager_Choi>();
                }

                // 싱글톤 오브젝트를 반환
                return m_instance;
            }
        }
        [Header("Temps")]
        private Dictionary<string, int> temp_IndexDictionary; // 임시로 플레이어 인덱스를 저장하는 딕셔너리
    #endregion

    #region [라이프 사이클 메서드]
    // ##################################################################################################
    // ▶[라이프 사이클 메서드]
    // ##################################################################################################
        private void Awake()
        {
        
        }
    #endregion

    #region [플레이어 프리팹 메서드]
    // ##################################################################################################
    // ▶[플레이어 프리팹 메서드]
    // ##################################################################################################
    // PlayerCustomizing_Choi에서 커스터마이징 설정 후 적용시 해당 데이터를
    // 배열로 받아서 PlayerPref에 저장하는 함수
    public void SetPlayerPrefForIndex()
    {
        // CustomizingManage_Choi에 저장된 각 카테고리의 파츠 인덱스 전부가 저장된
        // 딕셔너리를 호출
        temp_IndexDictionary = CustomizingManager_Choi.instance.GetIndexDictionary();
            
        // 딕셔너리에 저장되어 있는 값들을 foreach로 순회한 후
        // PlayerPref에 저장한다. ex) 
        foreach (KeyValuePair<string, int> value in temp_IndexDictionary)
        {
            PlayerPrefs.SetInt(value.Key, value.Value); // CarFrames, 0 과 같은 형태로 입력후 저장
            // 디버그 메세지 출력
            Debug.Log($"SaveDataForIndex(): ▶ PlayerPrefs 저장 성공 ▶ " +
                $"키: {value.Key}, 값: {value.Value}");
        }

        // 한 번더 PlayerPrefs 저장
        PlayerPrefs.Save();
    } // SaveData()

    public int GetPlayerPrefForIndex(string key)
    {
        // PlayerPrefs에 저장된 Index를 가져옴
        // 오버로드를 해서 실패했을 때 -1을 반환
        int temp_Value = PlayerPrefs.GetInt(key, -1);
        // PlayerPrefs에 저장된 Index를 가져오는데 성공했을 경우
        if (temp_Value != -1)
        {
            // 디버그 메세지 출력
            Debug.Log($"GetPlayerPrefForIndex(): ▶ PlayerPrefs 로드 성공 ▶ " +
                $"키: {key}, 값: {temp_Value}");
        }

        // PlayerPrefs에 저장된 Index를 가져오지 못했을 경우
        else
        {
            // 디버그 메세지 출력
            Debug.Log($"GetPlayerPrefForIndex(): ▶ PlayerPrefs 로드 실패 ▶ " +
                $"키: {key} ▶ 키가 올바른지 확인해주세요 ▶ 스크립트: PlayerDataManager_Choi");
        }

        // 찾은 Index 반환
        return temp_Value;
    } // GetPlayerPrefForIndex()

    #endregion
}
