
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CustomizingManager_Choi : MonoBehaviour
{
    #region [싱글톤 & 변수 선언부]
    // ##################################################################################################
    // ▶[싱글톤 & 변수 선언부]
    // ##################################################################################################
        // 싱글톤 선언
        private static CustomizingManager_Choi m_instance; // 싱글톤이 할당될 static 변수
        public static CustomizingManager_Choi instance
        {
            get
            {
                // 만약 싱글톤 오브젝트에 할당이 되지 않았다면
                if (m_instance == null)
                {
                    // 씬에서 오브젝트를 찾아 할당
                    m_instance  = FindObjectOfType<CustomizingManager_Choi>();
                }
            
                // 싱글톤 오브젝트를 반환
                return m_instance;
            }
        }

        private delegate void CustomizingFunc(); // 델리게이트 정의

        [Header("CSVFileReader")]
        private string defaultDirectory = "CSVFiles/"; // 기본 디렉토리 경로
        private string[] csvFileList =
        {
            "CarFrameList", "ColorList", "FlagList", "MarkList", 
            "WheelList_FL", "WheelList_FR", "WheelList_BL", "WheelList_BR"
        }; // CSV 파일명 리스트
        private string[] categoryList =
        {
            "CarFrames", "Colors", "Flags", "Marks",
            "Wheels_FL", "Wheels_FR", "Wheels_BL", "Wheels_BR"
        };
        private Dictionary<string, List<string>> temp_DataDictionary; // CSV 파일 임시 저장용 딕셔너리
        private Dictionary<string, Dictionary<string, List<string>>> dataDictionary; // CSV 파일들을 관리

        [Header("Delegate")]
        private static Dictionary<string, CustomizingFunc> customizingFuncs = 
            new Dictionary<string, CustomizingFunc>(); // 키 값으로 함수에 접근하기 위해 딕셔너리 선언

        [Header("Parents")]
        private List<GameObject> parents; // 부모들을 관리하는 리스트

        [Header("PlayerPrefab")]
        private Dictionary<string, int> 
        temp_IndexDictionary = new Dictionary<string, int>(); // 인덱스를 저장하는 딕셔너리(플레이어 프리팹 저장 목적)
                                                              // 카테고리를 키 값으로 접근한다.
    #endregion

    #region [라이프 사이클 메서드]
    // ##################################################################################################
    // ▶[라이프 사이클 메서드]
    // ##################################################################################################
        private void Awake()
        {
            // csvFileList에 있는 모든 CSV 파일 로드
            ReadCSVFileAndSave();

            // parents 리스트에 GameObject 부모들을 추가
            InputParents();

            // 오브젝트 풀링을 위한 오브젝트 생성
            CreateObjectPools();

            // 원하는 오브젝트 가져오기
            // 매개변수로 (카테고리 / 찾을 파츠 오브젝트명) 줘야함
            Debug.Log($"가져온 오브젝트 파츠명: {FindTargetObject("Wheels_FR", "DefaultWheel_FR").name}");

            // 현재 파츠 가져오기
            Debug.Log($"CarFrames의 현재 파츠 가져오기 : {GetCurrentObject("CarFrames").name}");

            // 현재 인덱스 저장
            SaveDataForPlayerPrefab();

            // PlayerPrefab에 저장되어 있는 인덱스 가져오기
            GetDataForPlayerPrefab("CarFrames");
        } // Awake()
    #endregion

    #region [인스턴스 호출 메서드]
    // ##################################################################################################
    // ▶[인스턴스 호출 메서드]
    // ##################################################################################################
    // csvFileList에 있는 모든 CSV 파일을 읽어서 dataDictionary에 저장하는 함수
    // 참조하는 스크립트 인스턴스: CSVReader_Choi
        private void ReadCSVFileAndSave()
            {
                // 데이터 딕셔너리 초기화
                dataDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                // csvFileList에 있는 모든 CSV 파일들을 dataDictionary에 저장
                for (int i = 0; i < csvFileList.Length; i++)
                {
                    temp_DataDictionary = 
                    CSVReader_Choi.instance.ReadCSVFile(defaultDirectory + csvFileList[i]);
                    dataDictionary.Add(csvFileList[i], temp_DataDictionary);
                }
            } // ReadCSVFileAndSave()

        // temp_IndexDictionary에 저장된 정보를 PlayerPrefab에 저장하는 함수
        // 참조하는 스크립트 인스턴스: PlayerDataManager_Choi
        public void SaveDataForPlayerPrefab()
        {
            // 인스턴스를 호출한 후 temp_IndexDictionary에 있는 정보를 PlayerPrefab에 저장
            PlayerDataManager_Choi.instance.SetPlayerPrefForIndex();
        } // SaveDataForPlayerPrefab()

        // PlayerPrefab에 저장된 인덱스를 호출하는 함수
        // 참조하는 스크립트 인스턴스: PlayerDataManager_Choi
        public int GetDataForPlayerPrefab(string key)
        {
            // PlayerPrefab에 저장된 인덱스를 키값으로 호출
            int temp_Index = PlayerDataManager_Choi.instance.GetPlayerPrefForIndex(key);
            
            // 호출한 Index 값 반환
            return temp_Index;
        } // GetDataForPlayerPrefab()
    #endregion

    #region [오브젝트 생성 메서드]
    // ##################################################################################################
    // ▶[오브젝트 생성 메서드]
    // ##################################################################################################
    // 오브젝트 풀링을 위한 오브젝트 생성 함수
        private void CreateObjectPools()
        {
            // 임시 변수 생성
            GameObject temp_parent = new GameObject();
            string temp_Category = "";
            string temp_DataDictionaryKey = "";

            // CSV 파일 갯수 만큼 for문 반복
            for (int i = 0; i < csvFileList.Length; i++)
            {
                // 임시 temp 변수에 값 할당
                temp_parent = parents[i];
                temp_Category = csvFileList[i];
                temp_DataDictionaryKey = GetKeyForDataDictionary(temp_Category);
                // dataDictionary[키 값]에 있는 정보를 바탕으로 오브젝트 생성
                for (int j = 0; j < dataDictionary[temp_Category][temp_DataDictionaryKey].Count; j++)
                {
                    // 오브젝트 인스턴스 생성 함수 호출
                    CreateInstantiate(temp_Category, temp_DataDictionaryKey, j, temp_parent);
                }
            }
        } // CreateObjectPools()

        // 오브젝트 인스턴스 생성 함수
        private void CreateInstantiate(string category, string key, int index, GameObject parent)
        {
            // 프리팹 인스턴스 오브젝트 생성
            GameObject temp_Prefab = GetPrefab(category, key, index);

            // 인스턴스 생성 성공시
            if (temp_Prefab != null)
            {
                // 오브젝트 생성 & 포지션 보정
                GameObject temp_Obj = Instantiate(temp_Prefab, AdjustChildPosition(parent, temp_Prefab), 
                    temp_Prefab.transform.rotation, parent.transform);
                // 오브젝트 이름 설정
                temp_Obj.name = temp_Prefab.name;
                // 오브젝트 비활성화
                temp_Obj.SetActive(true);

                Debug.Log("CreateInstantiate(): ▶ 오브젝트 인스턴스 생성에 성공하였습니다.");
            }

            // 인스턴스 생성 실패시
            else
            {
                // 디버그 메세지 출력
                Debug.Log("CreateInstantiate(): ▶ 오브젝트 인스턴스 생성에 실패하였습니다. ▶ " +
                    "Prefab을 가져올 수 없습니다. ▶ 스크립트: CustomizingManager_Choi");
                // 종료
                return;
            }
        } // CreateInstantiate()

        // Parents 리스트에 부모들을 추가하는 함수
        private void InputParents()
        {
            // 리스트 초기화
            parents = new List<GameObject>();
            // 임시 변수 생성
            GameObject temp_Obj = new GameObject();
            // 각 카테고리에 있는 부모 오브젝트들을 찾아 parents에 추가
            for (int i = 0; i < categoryList.Length; i++)
            {
                temp_Obj = GameObject.Find(categoryList[i]);
                if (temp_Obj != null) 
                { 
                    parents.Add(temp_Obj);
                }
            }
        } // InputParents()
    #endregion

    #region [오브젝트 호출 메서드]
    // ##################################################################################################
    // ▶[오브젝트 호출 메서드]
    // ##################################################################################################
    // 플레이어 오브젝트를 가져오는 함수
    // 추후 포톤서버 연결시 pv.IsMine으로 변경해야함
        private GameObject GetPlayerObject(int pvID)
        {
            // "Player" 태그로 플레이어 오브젝트를 검색
            GameObject temp_PlayerObject = GameObject.FindGameObjectWithTag("Player");
            // 플레이어 오브젝트를 찾은 경우
            if (temp_PlayerObject != null)
            {
                Debug.Log($"GetPlayerObject(): ▶ ViewID[{pvID}]: 플레이어 오브젝트 {temp_PlayerObject.name} 호출 완료 ▶ ");
            }

            // 플레이어 오브젝트를 찾지 못한 경우
            else
            {
                // 디버그 메세지 출력
                Debug.Log($"GetPlayerObject(): ▶ ViewID[{pvID}]: 플레이어 오브젝트 호출 실패 ▶ " +
                    $"'Player' 태그를 가진 오브젝트를 찾을 수 없습니다. ▶ 스크립트: CustomizingManager_Choi");
            }

            // 찾은 플레이어 오브젝트를 반환
            return temp_PlayerObject;
        } // GetPlayerObject()

        // 재귀 함수를 사용하여 원하는 하위 자식 오브젝트를 찾는 함수
        // 부모 오브젝트의 모든 계층 구조에 있는 자식을 탐색한다.
        private Transform FindChildRescursive(Transform parent, string targetName)
        {
            // parent의 모든 자식을 순회
            foreach (Transform child in parent)
            {
                // 원하는 자식 오브젝트를 찾은 경우
                if (child.name == targetName)
                {
                    // 찾은 자식 오브젝트를 반환
                    return child;
                }

                // 찾지 못한 경우
                else
                {
                    // 자식 오브젝트의 하위 자식들을 검색하기 위해 재귀 호출
                    // FindChildRescursive()의 매개변수로 child를 넣어 계층 구조를 탐색한다
                    Transform foundChild = FindChildRescursive(child, targetName);
                    // 원하는 자식 오브젝트를 찾은 경우
                    if (foundChild != null)
                    {
                        // 찾은 자식 오브젝트 반환
                        return foundChild;
                    }
                }
            }

            // targetName과 일치하는 자식 오브젝트를 찾지 못한 경우
            // 재귀를 위해 null 반환
            return null;
        } // FindChildRescursive()

        // 자식 오브젝트를 가져오는 함수
        private GameObject GetChildObject(GameObject parentObj, string childName)
        {
            Transform temp_ParentTransform = parentObj.transform;

            // 재귀 함수를 사용하여 원하는 자식 오브젝트를 찾음
            Transform temp_ChildTransform = FindChildRescursive(parentObj.transform, childName);

            // 자식 오브젝트의 트랜스폼을 찾은 경우
            if (temp_ChildTransform != null)
            {
                Debug.Log($"GetChildObject(): ▶ {parentObj.name} 자식 오브젝트 트랜스폼 " +
                    $"{childName} 호출 완료");
            }

            // 자식 오브젝트 트랜스폼을 찾지 못한 경우
            else
            {
                // 디버그 메세지 출력
                Debug.Log($"GetChildObject(): ▶ {parentObj.name} 자식 오브젝트 트랜스폼 호출 실패 ▶ " +
                    $"자식 {childName}을 찾을 수 없습니다. ▶ 스크립트: CustomizingManager_Choi");
            }

            // temp_Obj에 찾아낸 자식 오브젝트 추가
            GameObject temp_Obj = temp_ChildTransform.gameObject;

            // 찾은 자식 오브젝트 반환
            return temp_Obj;
        } // GetChildObject()

        // 부모 오브젝트의 활성화된 자식 파츠 오브젝트를 찾아서 반환하는 함수
        // 파츠 오브젝트를 찾을 경우 인덱스가 temp_IndexDictionary에 저장된다.
        private GameObject GetChildForActiveTrue(Transform parent)
        {
            int temp_Index = -1;
            string temp_Key = parent.name; // 인덱스 딕셔너리 저장용 키 값
            // parent 오브젝트의 자식을 전부 순회
            foreach(Transform child in parent)
            {
                temp_Index += 1;
                // 자식 오브젝트가 활성화 되있을 경우
                if (child.gameObject.activeSelf)
                {
                    // 현재 인덱스를 IndexDictionary에 저장
                    temp_IndexDictionary[temp_Key] = temp_Index;
                    // 찾은 자식 오브젝트를 반환
                    return child.gameObject;
                }
            }
            // 디버그 메세지 출력
            Debug.Log($"GetChildForActiveTrue(): ▶ 활성화된 {parent}의 자식 오브젝트 검색 실패 ▶ " +
                $"찾는 자식 오브젝트의 Active 상태를 확인하세요. ▶ 스크립트: CustomizingManager_Choi");

            // 찾지 못할 경우 null 반환
            return null;
        } // GetChildForActiveTrue()

        // 플레이어의 현재 파츠 오브젝트를 가져오는 함수
        // 매개변수로 카테고리를 넣으면 해당 카테고리의 활성화된 오브젝트를 반환
        private GameObject GetCurrentObject(string category)
        {
            // 부모인 플레이어 오브젝트를 찾음(추후 포톤ID넣어야함)
            GameObject temp_ParentObj = GetPlayerObject(0);
            // 카테고리에 해당하는 오브젝트 가져오기
            GameObject temp_CategoryObj = FindChildRescursive(temp_ParentObj.transform, category).gameObject;
            // 카테고리에 있는 활성화된 현재 파츠 오브젝트 가져오기
            GameObject temp_Obj = GetChildForActiveTrue(temp_CategoryObj.transform);
            // 오브젝트를 전부 가져왔을 경우
            if (temp_ParentObj != null && temp_CategoryObj != null && temp_Obj != null)
            {
                Debug.Log($"GetCurrentObject(): ▶ {temp_ParentObj.name} 자식 오브젝트 " +
                    $"{temp_CategoryObj.name}의 활성화된 현재 파츠 오브젝트 {temp_Obj.name} 호출 완료");
            }

            // 하나라도 오브젝트를 가져오지 못한 경우
            else
            {
                // 디버그 메세지 출력
                Debug.Log($"GetCurrentObject(): ▶ {temp_ParentObj.name} 자식 오브젝트 " +
                    $"{temp_CategoryObj.name}의 ▶ 활성화된 현재 파츠 오브젝트를 찾을 수 없습니다. " +
                    $"▶ 상태: temp_ParentObj = {temp_ParentObj != null}, " +
                    $"temp_CategoryObj = {temp_CategoryObj != null}, " +
                    $"temp_Obj = {temp_Obj != null} " +
                    $"스크립트: CustomizingManager_Choi");
            }

            // 찾은 현재 파츠 오브젝트 반환
            return temp_Obj;
        } // GetCurrentObject()

        // 원하는 플레이어 파츠 오브젝트를 가져오는 함수
        // 매개변수로 카테고리와 원하는 오브젝트 이름을 넣는다
        // 재귀 함수를 호출하여 모든 계층구조를 탐색해서 현재 파츠를 가져온다.
        private GameObject FindTargetObject(string category, string targetObjName)
        {
            // 플레이어 오브젝트를 가져옴
            GameObject temp_PlayerObj = GetPlayerObject(0); // 매개변수에 추후 포톤뷰 ID 넣어야함

            // 현재 파츠를 가져오기 위해 플레이어의 자식인 카테고리 오브젝트를 가져옴
            GameObject temp_CategoryObj = GetChildObject(temp_PlayerObj, category);

            // 카테고리 오브젝트의 자식으로 있는 파츠 오브젝트를 가져옴
            GameObject temp_Obj = GetChildObject(temp_CategoryObj, targetObjName);

            // 현재 파츠 오브젝트를 가져온 경우
            if (temp_PlayerObj != null && temp_CategoryObj != null && temp_Obj != null)
            {
                Debug.Log($"FindTargetObject(): ▶ 경로 {category}/{targetObjName} ▶ " +
                    $"현재 파츠 {targetObjName} 호출 완료");
            }

            // 현재 파츠 오브젝트를 가져오지 못한 경우
            else
            {
                // 디버그 메세지 출력
                Debug.Log($"FindTargetObject(): ▶ 경로 {category}/{targetObjName} ▶ " +
                    $"현재 파츠 {targetObjName} 호출 실패 ▶ {targetObjName}을 가져올 수 없습니다. ▶ " +
                    $"상태: temp_PlayerObj = {temp_PlayerObj != null}, " +
                    $"temp_CategoryObj = {temp_CategoryObj != null}, temp_Obj = {temp_Obj != null} ▶ " +
                    $"스크립트: CustomizingManager_Choi");
            }

            // 파츠 오브젝트 반환
            return temp_Obj;
        } // GetCurrentObject()

        // 원하는 카테고리의 프리팹을 반환하는 함수
        private GameObject GetPrefab(string category, string Key, int index)
        {
            // CSV 파일을 변환하는 과정에서 공백이 생기는 문제를 해결하기 위해 Trim() 함수 호출
            string temp_Name = dataDictionary[category][Key][index].Trim();
            GameObject temp_Prefab = Resources.Load<GameObject>(temp_Name);

            // 프리팹 로드 성공시
            if (temp_Prefab != null)
            {
                Debug.Log($"GetPrefab(): ▶ 경로 {category}/{temp_Name} ▶ 프리팹 로드 성공");
            }

            // 프리팹 로드 실패시
            else
            {
                Debug.Log($"GetPrefab(): ▶ 경로 {category}/{temp_Name} ▶ 프리팹 로드 실패 ▶ " +
                    $"Resources 폴더에 일치하는 Prefab이 없습니다. ▶ 스크립트: CustomizingManager_Choi");
            }

            // 프리팹 반환
            return temp_Prefab;
        } //GetPrefab()
    #endregion

    #region [오브젝트 관리 메서드]
    // ##################################################################################################
    // ▶[오브젝트 관리 메서드]
    // ##################################################################################################
    // 오브젝트 토글 함수
        private void ToggleObject(GameObject currentObj, GameObject newObj)
        {
            currentObj.SetActive(false); // 현재 오브젝트 비활성화
            newObj.SetActive(true); // 새로운 오브젝트 활성화

            // 아래에 category와 바뀐 오브젝트에 대한 정보를 playerPrefab에 저장하기

        } //ToggleObject()

        // 부모와 자식간의 포지션을 보정하는 함수
        private Vector3 AdjustChildPosition(GameObject parent, GameObject child)
        {
          // 트랜스폼 호출
            Vector3 parentPos = parent.transform.position;
                Vector3 childPos = child.transform.position;

                // 포지션 보정(부모Pos + 자식Pos)
                childPos = parentPos + childPos;

                // 보정된 포지션 반환
                return childPos;
        } // AdjustChildPosition()
    #endregion

    #region [변수 관리 메서드]
    // ##################################################################################################
    // ▶[변수 관리 메서드]
    // ##################################################################################################
        // dataDictionary 접근용 Key를 반환하는 함수
        // "PrefabName"으로 한번에 접근하면 접근할 수 없어 foreach로
        // Key를 찾아서 접근한다.
        private string GetKeyForDataDictionary(string category)
        {
            string temp_DataDictionaryKey = "";

            foreach (KeyValuePair<string, List<string>> value in dataDictionary[category])
            {
                // Key 값이 PrefabName일 경우
                if (value.Key.Contains("PrefabName"))
                {
                    // 키 값 추가
                    temp_DataDictionaryKey = value.Key;

                    // 한 번만 동작하고 종료
                    break;
                }
            }

            // 키 반환
            return temp_DataDictionaryKey;
        } // GetKeyForDataDictionary()
    
        // PlayerDataManager_Choi에서 PlayerPrefab을 저장하기 위해
        // 각 카테고리의 파츠 인덱스가 저장된 temp_IndexDictionary를 반환하는 함수
        public Dictionary<string, int> GetIndexDictionary()
        {
            // temp_IndexDictionary를 반환
            return temp_IndexDictionary;
        }
    #endregion

    #region [미사용 메서드]
    // ##################################################################################################
    // ▶[미사용 메서드]
    // ##################################################################################################
    // 델리게이트 호출 함수
        private void CallDelegateFunc(string funcName)
        {
            // 델리게이트 함수 안에 해당하는 함수명이 있는지 확인
            if (customizingFuncs.ContainsKey(funcName))
            {
                CustomizingFunc func = customizingFuncs[funcName];
                func(); // 함수 호출
            }
        } // CallDelegateFunc()
    #endregion
}
