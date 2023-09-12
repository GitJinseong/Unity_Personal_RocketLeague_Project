
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CustomizingManager_Choi : MonoBehaviour
{
    // ##################################################################################################
    // ***[변수 & 싱글톤 선언부]
    // ##################################################################################################
        #region 싱글톤 선언
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
        #endregion

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
        private Dictionary<string, List<string>> temp_DataDictionary; // CSV 파일 임시 저장용 변수
        private Dictionary<string, Dictionary<string, List<string>>> dataDictionary; // CSV 파일들을 관리

        [Header("Delegate")]
        private static Dictionary<string, CustomizingFunc> customizingFuncs = 
            new Dictionary<string, CustomizingFunc>(); // 키 값으로 함수에 접근하기 위해 딕셔너리 선언

        [Header("Parents")]
        private List<GameObject> parents; // 부모들을 관리하는 리스트



    // ##################################################################################################
    // ***[라이프 사이클 메서드]
    // ##################################################################################################
        private void Awake()
        {
            // csvFileList에 있는 모든 CSV 파일 로드
            ReadCSVFileAndSave();

            // parents 리스트에 GameObject 부모들을 추가
            InputParents();

            // 오브젝트 풀링을 위한 오브젝트 생성
            CreateObjectPools();

            // 오브젝트 가져오기
            // 매개변수로 (카테고리 / 찾을 파츠 오브젝트명) 줘야함
            Debug.Log($"가져온 오브젝트 파츠명: {GetCurrentObject("Wheels_FR", "DefaultWheel_FR").name}");
        }




    // ##################################################################################################
    // ***[인스턴스 호출 메서드]
    // ##################################################################################################
        // csvFileList에 있는 모든 CSV 파일을 읽어서 dataDictionary에 저장하는 함수
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
        }



    // ##################################################################################################
    // ***[오브젝트 생성 메서드]
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
        }

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
                temp_Obj.SetActive(false);

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
        }

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
        }



    // ##################################################################################################
    // ***[오브젝트 호출 메서드]
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
                Debug.Log($"GetPlayerObject(): ▶ ViewID[{pvID}]: 플레이어 오브젝트 호출 완료");
            }

            // 플레이어 오브젝트를 찾지 못한 경우
            else
            {
                // 디버그 메세지 출력
                Debug.Log($"GetPlayerObject(): ▶ ViewID[{pvID}]: 플레이어 오브젝트 호출 실패 ▶ " +
                    $"'Player' 태그를 가진 오브젝트를 찾을 수 없습니다. ▶ 스크립트: CustomizingManager_Choi");
            }

        Debug.Log($"찾은 오브젝트명: {temp_PlayerObject}");
            // 찾은 플레이어 오브젝트를 반환
            return temp_PlayerObject;
        }

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
        }

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
        }

        // 플레이어 오브젝트의 현재 파츠를 가져오는 함수
        // 재귀 함수를 호출하여 모든 계층구조를 탐색해서 현재 파츠를 가져온다.
        private GameObject GetCurrentObject(string category, string objName)
        {
            // 플레이어 오브젝트를 가져옴
            GameObject temp_PlayerObj = GetPlayerObject(0); // 매개변수에 추후 포톤뷰 ID 넣어야함

            // 현재 파츠를 가져오기 위해 플레이어의 자식인 카테고리 오브젝트를 가져옴
            GameObject temp_CategoryObj = GetChildObject(temp_PlayerObj, category);

            // 카테고리 오브젝트의 자식으로 있는 파츠 오브젝트를 가져옴
            GameObject temp_Obj = GetChildObject(temp_CategoryObj, objName);

            // 현재 파츠 오브젝트를 가져온 경우
            if (temp_PlayerObj != null && temp_CategoryObj != null && temp_Obj != null)
            {
                Debug.Log($"GetCurrentObject(): ▶ 경로 {category}/{objName} ▶ " +
                    $"현재 파츠 {objName} 호출 완료");
            }

            // 현재 파츠 오브젝트를 가져오지 못한 경우
            else
            {
                // 디버그 메세지 출력
                Debug.Log($"GetCurrentObject(): ▶ 경로 {category}/{objName} ▶ " +
                    $"현재 파츠 {objName} 호출 실패 ▶ {objName}을 가져올 수 없습니다. ▶ " +
                    $"상태: temp_PlayerObj = {temp_PlayerObj != null}, " +
                    $"temp_CategoryObj = {temp_CategoryObj != null}, temp_Obj = {temp_Obj != null} ▶ " +
                    $"스크립트: CustomizingManager_Choi");
            }

            // 파츠 오브젝트 반환
            return temp_Obj;
        }

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
        }



    // ##################################################################################################
    // ***[오브젝트 관리 메서드]
    // ##################################################################################################
        // 오브젝트 토글 함수
        private void ToggleObject(string category, GameObject currentObj, GameObject newObj)
        {
            currentObj.SetActive(false); // 현재 오브젝트 비활성화
            newObj.SetActive(true); // 새로운 오브젝트 활성화

            // 아래에 category와 바뀐 오브젝트에 대한 정보를 playerPrefab에 저장하기

        }

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
        }



    // ##################################################################################################
    // ***[변수 관리 메서드]
    // ##################################################################################################
        // dataDictionary 접근용 Key를 반환하는 함수
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
        }



    // ##################################################################################################
    // ***[미사용 메서드]
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
        }
}
