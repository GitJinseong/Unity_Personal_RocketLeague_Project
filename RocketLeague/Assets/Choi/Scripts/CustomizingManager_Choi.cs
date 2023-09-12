
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CustomizingManager_Choi : MonoBehaviour
{
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
    private Dictionary<string, Dictionary<string, List<string>>> dataDictionary; // CSV 파일 정보들을 관리

    [Header("Delegate")]
    private static Dictionary<string, CustomizingFunc> customizingFuncs = 
        new Dictionary<string, CustomizingFunc>(); // 키 값으로 함수에 접근하기 위해 딕셔너리 선언

    [Header("Parents")]
    //public GameObject carFrames; // 카 프레임 부모
    //public GameObject colors; // 컬러 부모
    //public GameObject flags; // 플래그 부모
    //public GameObject marks; // 마크 부모
    //public GameObject[] wheels; // 휠 부모(인스펙터에 휠 부모 4개를 추가해야 한다 csvFileList 순서대로 휠 추가)
    private List<GameObject> parents; // 부모들을 관리하는 리스트
    
    private void Awake()
    {
        Debug.Log("실행");
        // csvFileList에 있는 모든 CSV 파일 로드
        ReadCSVFileAndSave();

        // parents 리스트에 GameObject 부모들을 추가
        InputParents();

        // 오브젝트 풀링을 위한 오브젝트 생성
        CreateObjectPools();
    }

    // csvFileList에 있는 모든 CSV 파일을 읽어서 dataDictionary에 저장하는 함수
    private void ReadCSVFileAndSave()
    {
        // 데이터 딕셔너리 초기화
        dataDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
        // csvFileList에 있는 모든 CSV 파일들을 dataDictionary에 저장
        for (int i = 0; i < csvFileList.Length; i++)
        {
            temp_DataDictionary = CSVReader_Choi.instance.ReadCSVFile(defaultDirectory + csvFileList[i]);
            dataDictionary.Add(csvFileList[i], temp_DataDictionary);
            Debug.Log($"키 값 : {csvFileList[i]}");
            Debug.Log($"결과 : {dataDictionary[csvFileList[i]]["Info"]}");
            Debug.Log($"개수 : {dataDictionary.Count}");

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
                Debug.Log($"부모 추가 : {temp_Obj.name}");
            }
        }
    }

    // 오브젝트 풀링을 위한 오브젝트 생성 함수
    private void CreateObjectPools()
    {
        // 임시 변수 생성
        GameObject temp_parent = new GameObject();
        string temp_Key = "";

        // CSV 파일 갯수 만큼 for문 반복
        for (int i = 0; i < csvFileList.Length; i++)
        {
            temp_parent = parents[i];
            temp_Key = csvFileList[i];
            // dataDictionary[키 값]에 있는 정보를 바탕으로 오브젝트 생성
            for (int j = 0; j < dataDictionary[temp_Key].Count; j++)
            {
                // 차량 파츠 인스턴스 생성 함수 호출
                CreateInstantiate(j, temp_Key, temp_parent);
            }
        }
    }

    // 오브젝트 토글 함수
    private void ToggleObject(GameObject previousObj, GameObject currentObj)
    {
        previousObj.SetActive(false); // 이전 오브젝트 비활성화
        currentObj.SetActive(true); // 현재 오브젝트 활성화
    }


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

    // 부모를 설정하는 함수
    private void SetParent(GameObject parent, GameObject child)
    {
        Transform parentTransform = parent.transform;
        Transform childTransform = child.transform;

        // 부모 지정
        childTransform.SetParent(parentTransform);
    }

    // 차량 파츠 오브젝트 풀링용 인스턴스 생성 함수
    private void CreateInstantiate(int index, string category, GameObject parent)
    {
        // 프리팹 인스턴스 오브젝트 생성
        GameObject prefab = GetPrefab(index, category);
        GameObject obj = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, parent.transform);

        // 오브젝트 비활성화
        obj.SetActive(false);
    }

    // 원하는 카테고리의 프리팹을 반환하는 함수
    private GameObject GetPrefab(int index, string category)
    {
        string temp_Name = "";
        GameObject temp_Prefab = new GameObject();

        // dataDictionary에 접근하기 위해 foreach문을 사용
        foreach (KeyValuePair<string, List<string>> value in dataDictionary[category])
        {
            // Key 값이 PrefabName일 경우
            if (value.Key == "PrefabName")
            {
                temp_Name = dataDictionary[category][value.Key][index]; // dataDictionary 안에 있는 값을 호출
                temp_Prefab = Resources.Load<GameObject>(temp_Name); // 위에서 불러온 값으로 일치하는 이름의 prefab을 호출
                Debug.Log($"키값:{value.Key}");
                Debug.Log($"값: {dataDictionary[category][value.Key][index]}");

                // 한 번만 동작하고 종료
                break;
            }

        }

        // 프리팹 반환
        return temp_Prefab;
    }

}
