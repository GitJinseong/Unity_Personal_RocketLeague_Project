
using System.Collections.Generic;
using UnityEngine;


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
                m_instance = FindObjectOfType<CustomizingManager_Choi>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private delegate void CustomizingFunc(); // 델리게이트 정의

    [Header("CSVFileReader")]
    private const string DEFAULT_DIRECTORY = "CSVFiles/"; // 기본 디렉토리 경로
    private const string DEFAULT_KEY = "PrefabName"; // 기본 프리팹 접근 키 값 
    private string[] csvFileList =
    {
            "CarFrameList", "FlagList", "MarkList",
            "WheelList_FL", "WheelList_FR", "WheelList_BL", "WheelList_BR"
        }; // CSV 파일명 리스트
    private string[] categoryList =
    {
            "CarFrames", "Flags", "Marks",
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
    private string[] CarTypes = { "CustomizingCar", "OrangeCar" }; // CreateObjectWithCustomizing()에서 플레이어
                                                            // 오브젝트를 생성할 때 호출되는 배열 
                                                            // Resource 폴더에 같은 이름의 프리팹 생성해야함
    private Dictionary<string, int>
    temp_IndexDictionary = new Dictionary<string, int>(); // 인덱스를 저장하는 딕셔너리(플레이어 프리팹 저장 목적)
                                                          // 카테고리를 키 값으로 접근한다.

    [Header("PhotonView")]
    private int photonViewID = 0; // 추후 포톤뷰 연동시 변경 
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
        //Debug.Log($"가져온 오브젝트 파츠명: {FindTargetObject("Wheels_FR", "DefaultWheel_FR").name}");

        // 현재 파츠 가져오기
        //Debug.Log($"CarFrames의 현재 파츠 가져오기 : {GetCurrentObject("CarFrames").name}");

        // 현재 인덱스 저장
        //SaveDataForPlayerPrefab();

        // PlayerPrefab에 저장되어 있는 Index 가져오기
        //GetDataForPlayerPrefab("CarFrames");

        // PlayerPrefab에 저장되어 있는 파츠별 Index 가져온 후
        // 가져온 Index[]로 전부 토글하는 함수
        ToggleAllObejcts(GetAllDataForPlayerPrefabs());

        // 플레이어의 현재 모든 파츠를 temp_IndexDictionary에 저장
        SaveAllPlayerPartsToTempIndexDictionary();

        // PlayerPrefab에 저장되어 있는 Index 들을 가져와서 차량 오브젝트 생성 후 파츠 부착하기 
        CreateObjectWithCustomizing(0, 0);
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
        // 키 값은 카테고리
        for (int i = 0; i < csvFileList.Length; i++)
        {
            temp_DataDictionary =
            CSVReader_Choi.instance.ReadCSVFile(DEFAULT_DIRECTORY + csvFileList[i]);
            dataDictionary.Add(categoryList[i], temp_DataDictionary);
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

        // 가져온 인덱스가 -1일 경우
        // (인덱스를 가져오지 못했을 경우)
        if (temp_Index == -1)
        {
            // 오류 해결을 위해 강제로 0으로 변환
            temp_Index = 0;
        }

        // 호출한 Index 값 반환
        return temp_Index;
    } // GetDataForPlayerPrefab()

    // PlayerPrefab에 저장된 모든 인덱스를 호출하고 배열로 반환하는 함수
    public int[] GetAllDataForPlayerPrefabs()
    {
        // 임시로 인덱스를 저장할 배열 생성
        int[] temp_Indexs = new int[categoryList.Length];
        // 카테고리 배열 크기 만큼 순회
        // 임시로 categoryList.Length -> 7로 사용
        for (int i = 0; i < categoryList.Length; i++)
        {
            // temp_Indexs에 플레이어 프리팹에 저장된 인덱스를 추가
            temp_Indexs[i] = GetDataForPlayerPrefab(categoryList[i]);
        }

        // 인덱스 배열 반환
        return temp_Indexs;
    } // etAllDataForPlayerPrefabs()
    #endregion

    #region [오브젝트 생성 메서드]
    // ##################################################################################################
    // ▶[오브젝트 생성 메서드]
    // ##################################################################################################
    // 오브젝트 풀링을 위한 오브젝트 생성 함수
    private void CreateObjectPools()
    {
        // 카테고리 임시 변수 생성
        GameObject temp_CategoryObj;
        // 임시 변수에 플레이어 오브젝트 할당
        GameObject temp_PlayerObj = GetPlayerObject(photonViewID);
        string temp_Category = "";
        string temp_DataDictionaryKey = DEFAULT_KEY;
        // CSV 파일 갯수 만큼 for문 반복
        for (int i = 0; i < csvFileList.Length; i++)
        {
            // 임시 temp 변수에 값 할당
            temp_Category = categoryList[i]; // 카테고리 할당
                                             // 플레이어 오브젝트 내에 있는 카테고리 오브젝트를 재귀 함수로 찾아서 할당
            temp_CategoryObj = FindChildRescursive(temp_PlayerObj.transform, temp_Category).gameObject;
            // 접근 키 할당
            temp_DataDictionaryKey = GetKeyForDataDictionary(temp_Category);
            // dataDictionary[키 값]에 있는 정보를 바탕으로 오브젝트 생성
            for (int j = 0; j < dataDictionary[temp_Category][temp_DataDictionaryKey].Count; j++)
            {
                // 오브젝트 인스턴스 생성 함수 호출
                CreateInstantiate(temp_Category, temp_DataDictionaryKey, j, temp_CategoryObj);
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
    } // CreateInstantiate()

    // 모든 파츠 리스트를 순회하는 함수
    // PlayerPrefab에 저장되어 있는 Index를 바탕으로 오브젝트를 생성하고
    // 부위별 파츠를 오브젝트에 장착하는 함수
    // *teamID: [0]은 블루 / [1]은 오렌지 팀이다.
    public void CreateObjectWithCustomizing(int pvID, int teamID)
    {
        Debug.Log("호출");
        // 임시 변수 선언
        string temp_CsvList = "";
        string temp_Category = "";
        // 딕셔너리 키 값 넣기
        string temp_DictionaryKey = DEFAULT_KEY;
        int temp_Index = 0;
        // Resources.Load<GameObject>(string)으로 프리팹을 검색 후 가져옴
        GameObject temp_Prefab = Resources.Load<GameObject>(CarTypes[teamID]);
        // Pos, Rotation 값을 기본으로 플레이어 오브젝트 생성(블루/오렌지)
        GameObject temp_PlayerObj = Instantiate(temp_Prefab, Vector3.zero,
           Quaternion.identity);
        // 임시로 카테고리를 저장할 오브젝트
        GameObject temp_CategoryObj;
        // 오브젝트 이름 설정(BlueCar/OrangeCar)
        temp_PlayerObj.name = CarTypes[teamID];
        // categoryList[] 만큼 순회 
        for (int i = 0; i < categoryList.Length; i++)
        {
            // 임시 변수에 키 & 딕셔너리 키 할당
            temp_Category = categoryList[i];
            // 할당된 키로 PlayerPrefab에 저장된 Index 호출
            temp_Index = GetDataForPlayerPrefab(temp_Category);
            Debug.Log($"가져온 인덱스 {temp_Index}");
            // 오브젝트 인스턴스 생성 함수 호출, 위에서 생성된 플레이어 오브젝트를 부모로 설정
            // 아래 호출 부분에서 딕셔너리 키값을 찾을수없다는 오류발생
            Debug.Log($"데이터 딕셔너리 개수: {dataDictionary.Count}");
            foreach (var value in dataDictionary)
            {
                Debug.Log(value.Key);
            }
            // 플레이어 오브젝트 하위의 카테고리에 맞는 오브젝트 검색 후 반환된 오브젝트 저장
            temp_CategoryObj = FindChildRescursive(temp_PlayerObj.transform, temp_Category).gameObject;
            // 카테고리, 딕셔너리키, 인덱스, 카테고리 오브젝트를 사용하여 인스턴스를 생성한다.
            // 해당하는 카테고리 오브젝트의 자식으로 파츠가 생성된다.
            CreateInstantiate(temp_Category, temp_DictionaryKey, temp_Index, temp_CategoryObj);
        }
    }
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

    //// 카테고리에 해당하는 부모 오브젝트를 가져오는 함수
    //// CarFrames, Wheels와 같은 오브젝트를 가져온다.
    //private GameObject GetCategoryObject(string category)
    //{
    //    // 임시 변수 선언
    //    GameObject temp_Obj;
    //    GameObject temp_ParentObj;

    //    // temp_ParentObj에 플레이어 오브젝트 할당
    //    temp_ParentObj = GetPlayerObject

    //    // temp_ParentObj에 부모 오브젝트를 
    //    // temp_Obj에 카테고리에 해당하는 오브젝트 할당
    //    temp_Obj = FindChildRescursive(temp_Parent, category);


    //}

    // 자식 오브젝트를 가져오는 함수
    private GameObject GetChildObject(GameObject parentObj, string childName)
    {
        // 임시 변수에 부모 트랜스폼 추가
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
        foreach (Transform child in parent)
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
        // 부모인 플레이어 오브젝트를 찾음
        GameObject temp_ParentObj = GetPlayerObject(photonViewID);
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

    // 현재 플레이어의 모든 파츠를 가져와서 temp_IndexDictionary에 저장하는 함수
    public void SaveAllPlayerPartsToTempIndexDictionary()
    {
        for (int i = 0; i < categoryList.Length; i++)
        {
            // 파츠를 찾은 후 temp_IndexDictionary에 Index를 저장함
            GetCurrentObject(categoryList[i]);
        }
    }

    // 원하는 플레이어 파츠 오브젝트를 가져오는 함수
    // 매개변수로 카테고리와 원하는 오브젝트 이름을 넣는다
    // 재귀 함수를 호출하여 모든 계층구조를 탐색해서 현재 파츠를 가져온다.
    private GameObject FindTargetObject(string category, string targetObjName)
    {
        // 플레이어 오브젝트를 가져옴
        GameObject temp_PlayerObj = GetPlayerObject(photonViewID);

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

    // 최초 실행시 모든 카테고리에 있는 오브젝트 중에
    // 매개변수로 받은 인덱스에 해당하는 파츠를 전부 활성화 하는 함수
    private void ToggleAllObejcts(int[] indexs)
    {
        //// 임시 변수 선언
        //string temp_Category = "";
        //string temp_Name = "";
        //int temp_Index = 0;
        //GameObject temp_PlayerObj;
        //GameObject temp_CategoryObj;
        //GameObject temp_TargetObj;
        //GameObject temp_CurrentObj;
        //// temp_PlayerObj에 플레이어 오브젝트 할당
        //temp_PlayerObj = GetPlayerObject(photonViewID);
        for (int i = 0; i < categoryList.Length; i++)
        {
            // 카테고리와 인덱스를 받아서 파츠를 토글하는 함수 호출
            TogglePartForCategory(categoryList[i], indexs[i], false);

            //// 임시 변수에 카테고리 키 & 인덱스 할당
            //temp_Category = categoryList[i];
            //temp_Index = indexs[i];

            //// 위에 있는 카테고리와 인덱스로 PrefabName 호출
            //temp_Name = GetDataDictionaryForPrefabName(temp_Category, temp_Index);

            //// 카테고리에 해당하는 타겟 파츠 오브젝트를 가져옴
            //Debug.Log($"타겟 오브젝트명 : {temp_Name}");
            //Debug.Log($"플레이어 이름 : {temp_PlayerObj.name}");
            //temp_CategoryObj = FindChildRescursive(temp_PlayerObj.transform, temp_Category).gameObject;
            //Debug.Log($"카테고리 오브젝트명 : {temp_CategoryObj.name}");
            //// 카테고리 오브젝트 자식 하위의 타겟 오브젝트를 가져옴
            //temp_TargetObj = FindChildRescursive(temp_CategoryObj.transform, temp_Name).gameObject;

            //// 타겟 오브젝트가 null이 아닐 경우
            //if (temp_TargetObj != null)
            //{
            //    // temp_TargetObj를 활성화
            //    temp_TargetObj.SetActive(true);
            //}

            //// 카테고리에 해당하는 오브젝트 가져오기
            ////temp_CategoryObj = FindChildRescursive(temp_PlayerObj.transform, temp_Category).gameObject;

            //// 카테고리에 해당하는 활성화된 현재 오브젝트를 가져옴
            //temp_CurrentObj = GetCurrentObject(temp_Category);

            //// 현재 오브젝트의 Active를 false하고 / 타겟 오브젝트를 true함
            //// currentObj와 TargetObj를 토글
            //ToggleObject(temp_CurrentObj, temp_TargetObj);
        }
    } // ToggleAllObejcts()

    // 카테고리와 인덱스를 받아서 파츠를 토글하는 함수
    public void TogglePartForCategory(string category, int index, bool isToggle)
    {
        // 임시 변수 선언
        string temp_Name = "";
        GameObject temp_PlayerObj;
        GameObject temp_CategoryObj;
        GameObject temp_TargetObj;
        GameObject temp_CurrentObj;

        // temp_PlayerObj에 플레이어 오브젝트 할당
        temp_PlayerObj = GetPlayerObject(photonViewID);

        // 카테고리와 인덱스로 PrefabName 호출
        temp_Name = GetDataDictionaryForPrefabName(category, index);

        // 카테고리에 해당하는 타겟 파츠 오브젝트를 가져옴
        Debug.Log($"타겟 오브젝트명 : {temp_Name}");
        Debug.Log($"플레이어 이름 : {temp_PlayerObj.name}");
        temp_CategoryObj = FindChildRescursive(temp_PlayerObj.transform, category).gameObject;
        Debug.Log($"카테고리 오브젝트명 : {temp_CategoryObj.name}");

        // 카테고리 오브젝트 자식 하위의 타겟 오브젝트를 가져옴
        temp_TargetObj = FindChildRescursive(temp_CategoryObj.transform, temp_Name).gameObject;

        // 카테고리에 해당하는 현재 파츠를 가져옴
        temp_CurrentObj = GetCurrentObject(category);

        // 타겟 오브젝트가 null이 아닐 경우
        if (temp_TargetObj != null)
        {
            // 만약 isToggle이 true일 경우
            if (isToggle)
            {
                // 장비 교체를 위해 temp_TargetObj를 활성화 하고 기존 오브젝트를
                // 비활성화하는 토글 함수 호출
                ToggleObject(temp_CurrentObj, temp_TargetObj);
            }

            // 아닐 경우
            else
            {
                // 타겟 오브젝트를 활성화
                temp_TargetObj.SetActive(true);
            }
        }
    } // TogglePartForCategory()

    // 부모와 자식간의 포지션을 보정하는 함수
    private Vector3 AdjustChildPosition(GameObject parent, GameObject child)
    {
        // 트랜스폼 호출
        Vector3 parentPos = parent.transform.position;
        Vector3 childPos = child.transform.position;

        // 포지션 보정(부모Pos + 자식Pos)
        childPos = parentPos + childPos;

        Debug.Log($"{child.name}변환된Pos: {childPos}");

        // 보정된 포지션 반환
        return childPos;
    } // AdjustChildPosition()

    // 휠의 모든 포지션을 보정하는 함수
    // CarFrames의 리스트에 있는 포지션 값을 가져와서 보정한다.
    // 매개변수로 현재 CarFrames의 인덱스를 받는다.
    public void AdjustWheelsPosition(int index)
    {
        // 임시 변수 선언
        float temp_PosX = 0f;
        float temp_PosY = 0f;
        float temp_PosZ = 0f;
        string temp_Key = "";
        string temp_Category = "CarFrames";
        GameObject temp_CategoryObj;
        GameObject temp_PlayerObj;
        Vector3 temp_Pos;
        // temp_PlayerObj에 플레이어 오브젝트 할당
        temp_PlayerObj = GetPlayerObject(photonViewID);
        // 휠이 categoryList의 index 3번 부터 시작하여 i=1로 설정
        for (int i = 3; i < categoryList.Length; i++)
        {
            // temp_Key에 키 값 할당 
            temp_Key = categoryList[i] + "_Pos";

            // 플레이어 오브젝트와 카테고리를 매개변수로 
            // temp_CategoryObj에 카테고리 오브젝트 호출
            temp_CategoryObj = FindChildRescursive(temp_PlayerObj.transform, categoryList[i]).gameObject;

            // dataDictionary에서 temp_Key로 포지션 값들을 가져온 후
            // string -> float으로 변환 & 매개변수로 받은 index로 현재 CarFrames에 접근
            temp_PosX = float.Parse(dataDictionary[temp_Category][temp_Key + "X"][index]);
            temp_PosY = float.Parse(dataDictionary[temp_Category][temp_Key + "Y"][index]);
            temp_PosZ = float.Parse(dataDictionary[temp_Category][temp_Key + "Z"][index]);

            // temp_Pos에 포지션 값 할당
            temp_Pos = new Vector3(temp_PosX, temp_PosY, temp_PosZ);

            // 가져온 휠 카테고리 오브젝트의 로컬 포지션 변경
            // 로컬 포지션을 변경해야 부모 오브젝트의 포지션과는 관계 없이 포지션이 설정된다.
            temp_CategoryObj.transform.localPosition = temp_Pos;

            // 디버그 메세지
            Debug.Log($"index {index}: 가져온 포지션 값: {temp_CategoryObj.name} " +
                $"PosX: {dataDictionary[temp_Category][temp_Key + "X"][index]} " +
                $"PosY: {dataDictionary[temp_Category][temp_Key + "Y"][index]} " +
                $"PosZ: {dataDictionary[temp_Category][temp_Key + "Z"][index]} ");

            Debug.Log($"index {index}: 변환된 포지션 값: {temp_CategoryObj.name} " +
                $"Pos: {temp_CategoryObj.transform.localPosition}");
        }
    }
    #endregion

    #region [변수 관리 메서드]
    // ##################################################################################################
    // ▶[변수 관리 메서드]
    // ##################################################################################################
    // Parents 리스트에 부모들을 추가하는 함수
    private void InputParents()
    {
        // 리스트 초기화
        parents = new List<GameObject>();
        // 임시 변수 생성
        GameObject temp_Obj;
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

    // PlayerDataManager_Choi에서 PlayerPrefab을 저장하기 위해
    // 각 카테고리의 파츠 인덱스가 저장된 temp_IndexDictionary를 반환하는 함수
    public Dictionary<string, int> GetIndexDictionary()
    {
        // temp_IndexDictionary를 반환
        return temp_IndexDictionary;
    }

    // 카테고리와 인덱스로 DataDictionary에 있는 Prefab이름을 가져오는 함수
    // PlayerPrefab에 저장되어 있는 인덱스로 오브젝트 토글을 하기위해 사용한다
    private string GetDataDictionaryForPrefabName(string category, int index)
    {
        // 임시 변수 선언
        string temp_Name = "";
        // 카테고리와 인덱스로 dataDictionary에 있는 PrefabName을 호출
        Debug.Log($"이름:{category} 인덱스: {index}");
        temp_Name = dataDictionary[category]["PrefabName"][index];
        // 가져온 temp_Name 반환
        return temp_Name;
    }

    // 카테고리별 최대 인덱스를 배열로 반환하는 함수
    public int[] GetMaxIndexForCategorys()
    {
        // 임시로 7을 넣어 배열 생성
        int[] temp_MaxIndexs = new int[7];

        // categoryList만큼 순회
        for (int i = 0; i < categoryList.Length; i++)
        {
            // temp_MaxIndexs에 최대 인덱스 저장
            // Count를 사용할 경우 행이 포함되어 추가로 -1를 해준다.
            temp_MaxIndexs[i] = dataDictionary[categoryList[i]]["Index"].Count - 1;
        }

        // maxIndexs 배열 반환
        return temp_MaxIndexs;
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
    #endregion
}
