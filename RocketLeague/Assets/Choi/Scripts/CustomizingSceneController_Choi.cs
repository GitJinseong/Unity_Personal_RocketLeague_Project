using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CustomizingSceneController_Choi : MonoBehaviour
{
    [Header("Preview")]
    public Slider zoomSlider; // 줌 슬라이드
    public Slider rotationSlider; // 로테이션 슬라이드
    public Camera renderCamera; // 프리뷰 랜더 카메라
    public GameObject previewObject; // 프리뷰 오브젝트
    private float cameraSize = 10.0f; // 기본 카메라 사이즈 10.0f
    private const float DEFAULT_CAMERA_ZOOM = 20.0f; // 기본 카메라 배율
    private const float DEFAULT_ROTATION = 180.0f; // 기본 Rotation 값
    private const float DEFAULT_ROTATION_SCALE = 360f; // 기본 Rotation 배율

    [Header("PartsList")]
    private string[] categorys =
    {
        "CarFrames", "Flags", "Marks",
        "Wheels_FL", "Wheels_FR", "Wheels_BL", "Wheels_BR"
    }; // 카테고리 배열
    private int[] partsListIndexs = new int[7]; // 카테고리의 길이 만큼 int 배열 선언
    private int[] partsListMaxIndexs = new int[7]; // 카테고리 별 최대 인덱스를 저장하는 배열

    [Header("UI")]
    public TMP_Text[] categoryTxts;
    public Button[] categoryBtnNexts;
    public Button[] categoryBtnPrevs;

    void Start()
    {
        // PlayerPrefab에 저장되어 있는 현재 인덱스를
        // 가져오는 함수 호출
        GetIndexsForPlayerPrefab();

        // 카테고리 별 최대 인덱스를 가져오는 함수 호출
        GetCategoryMaxIndexs();

        // 커스터마이징 씬의 카테고리 텍스트를 전부 업데이트하는 함수 호출
        UpdateTxtForCategorys();

        // 차량이 정면을 바라보지 않는 현상이 발생하여 해결하기 위해
        // Start()에서 차량 오브젝트의 회전 함수 호출
        UpdateObjectRotateFromSlider(0.0f);

        // 타이어의 포지션 위치를 재조정 하는 함수 호출
        // partsListIndexs[0]을 매개변수로 넣어 CarFrames의 현재 인덱스를 보냄
        CustomizingManager_Choi.instance.AdjustWheelsPosition(partsListIndexs[0]);

        // 이벤트 리스너 등록(onValueChanged)
        // 슬라이더 값이 변경될 때 마다 함수가 호출되게 하는 목적
        zoomSlider.onValueChanged.AddListener(UpdateCameraSizeFromSlider);
        rotationSlider.onValueChanged.AddListener(UpdateObjectRotateFromSlider);
        // OnClick이벤트가 호출될 때 마다 함수가 호출되게 하는 목적
        // categoryBtns에 있는 모든 버튼을 리스너에 등록한다.
        // categoryTxts 배열 길이만큼 순회한다
        for (int i = 0; i < categoryTxts.Length; i++)
        {
            // temp_Index 변수에 i 값을 담고 리스너에 등록할 함수의 매개변수로
            // 등록하는 이유는 i 값을 직접 넣을 경우 클로저(for문이 종료될떄)됐을 때
            // 기준으로 i값이 리스너에 모두 동일하게 들어가기 때문이다.
            // 그래서 따로 index를 선언하여 서로 다른 값을 가지게 했다.
            // temp_Index 변수를 사용하여 한 번만 선언하고 재사용 하려고 했으나
            // 매번 index를 선언하지 않을 경우 서로 같은 값을 가지는 현상이 발생한다.
            // 그래서 아래와 같이 수정했다.
            int index = i;
            // onClick 이벤트 리스너에 함수를 등록
            // 람다식을 사용하지 않을경우 void 함수를 등록할 수 없어 람다를 사용했다.
            categoryBtnNexts[i].onClick.AddListener(() => BtnNext(index));
            categoryBtnPrevs[i].onClick.AddListener(() => BtnPrevious(index));
        }
    }

    // ##################################################################################################
    // ▶[버튼 메서드]
    // ##################################################################################################
    // Save 기능을 호출하는 함수
    public void BtnSave()
    {
        // 현재 파츠를 temp_IndexDictionary에 
        // 현재 플레이어의 모든 파츠를 가져와서 CustomizingManager에 있는
        // temp_IndexDictionary에 저장하는 함수 호출
        CustomizingManager_Choi.instance.SaveAllPlayerPartsToTempIndexDictionary();
        // temp_IndexDictionary를 PlayerPref에 저장하는 함수 호출
        PlayerDataManager_Choi.instance.SetPlayerPrefForIndex();
    }
    
    // Random 하게 커스터마이징을 설정하는 함수
    public void BtnRandom()
    {
        // 임시 변수 선언
        int temp_Value = 0;
        //partsListIndexs 배열 길이 만큼 순회
        for (int i = 0; i < partsListIndexs.Length; i++)
        {
            // temp_Value에 1 이상 최대값 미만의 랜덤 값 설정(1부터 (최대값-1)까지)
            temp_Value = Random.RandomRange(0, (partsListMaxIndexs[i]));
            // partListIndexs에 값 넣기
            partsListIndexs[i] = temp_Value;

            // 현재 인덱스에 맞게 카테고리의 파츠를 업데이트하는 함수 호출
            UpdatePartForCategory(categorys[i], partsListIndexs[i]);
        }

        // 커스터마이징 씬의 카테고리 텍스트를 전부 업데이트하는 함수 호출
        UpdateTxtForCategorys();
    }

    // 로비 씬으로 돌아가는 Back 함수
    public void BtnBack()
    {
        // 로비 씬(SampleScene)으로 이동
        SceneManager.LoadScene("SampleScene");
    }

    // 커스터마이징 씬의 카테고리 텍스트를 전부 업데이트하는 함수
    public void UpdateTxtForCategorys()
    {
        // 카테고리만큼 순회
        for (int i = 0; i < categorys.Length; i++)
        {
            // 텍스트를 다음과 같이 변경
            // ex) Car_Frames(1/5)
            // 실제로 인덱스는 0부터 사용되지만 보이는 텍스트는
            // 1부터 보이게 하기 위해 partsListIndexs[i]에 추가로 1를 더한다.
            categoryTxts[i].text = categorys[i] + "(" + (partsListIndexs[i] + 1) +
                "/" + partsListMaxIndexs[i] + ")";
        }
    }

    // 카테고리의 인덱스를 다음으로 넘기는 함수
    public void BtnNext(int index)
    {
        Debug.Log($"next index: {index}");


        // 현재 인덱스가 최대 인덱스보다 같거나 클 경우
        // 인덱스와 최대 인덱스 비교를 위해 인덱스를 +1 한다
        // 왜냐하면 최대 인덱스는 1부터 시작하기 때문
        if ((partsListIndexs[index] + 1) >= partsListMaxIndexs[index])
        {
            // 최대 인덱스로 변경, 단 최대 인덱스는 랜덤 값과 텍스트 표시를 위해
            // 1부터 시작되므로 실제 인덱스에 적용할 때는 추가로 -1을 해주어야 한다.
            partsListIndexs[index] = partsListMaxIndexs[index] - 1;
        }

        // 아닐 경우
        else
        {
            // 현재 인덱스에서 1 증감
            partsListIndexs[index] += 1;
        }

        // 현재 인덱스에 맞게 카테고리의 파츠를 업데이트하는 함수 호출
        UpdatePartForCategory(categorys[index], partsListIndexs[index]);

        // 커스터마이징 씬의 카테고리 텍스트를 전부 업데이트하는 함수 호출
        UpdateTxtForCategorys();
    }

    // 카테고리의 인덱스를 뒤로 넘기는 함수
    public void BtnPrevious(int index)
    {
        Debug.Log($"previous index: {index}");
        // 현재 인덱스가 0 보다 작거나 같을 경우
        if (partsListIndexs[index] <= 0)
        {
            // 최소 인덱스 0으로 변경
            partsListIndexs[index] = 0;
        }

        // 아닐 경우
        else
        {
            // 현재 인덱스에서 1 감산
            partsListIndexs[index] -= 1;
        }

        // 현재 인덱스에 맞게 카테고리의 파츠를 업데이트하는 함수 호출
        UpdatePartForCategory(categorys[index], partsListIndexs[index]);

        // 커스터마이징 씬의 카테고리 텍스트를 전부 업데이트하는 함수 호출
        UpdateTxtForCategorys();
    }

    // 현재 인덱스에 맞게 카테고리의 파츠를 업데이트하는 함수
    private void UpdatePartForCategory(string category, int index)
    {
        // CustomizingManager_Choi에서 카테고리와 인덱스로 파츠를 토글하는 함수 호출
        CustomizingManager_Choi.instance.TogglePartForCategory(category, index, true);

        // 만약 매개변수로 받은 category가 CarFrames일 경우
        if (category == "CarFrames")
        {
            // 타이어의 포지션 위치를 재조정 하는 함수 호출
            // partsListIndexs[0]을 매개변수로 넣어 CarFrames의 현재 인덱스를 보냄
            CustomizingManager_Choi.instance.AdjustWheelsPosition(partsListIndexs[0]);
        }
    }

    // ##################################################################################################
    // ▶[이벤트 리스너 전용 메서드]
    // ##################################################################################################
    // OnValueChanged 리스너에 등록된 함수
    // 슬라이더 값이 변경될 때 마다 호출된다.
    // Zoom 슬라이더 값에 따라 카메라의 사이즈를 변경하는 함수
    // value라는 변경된 값을 매개변수로 받는다.
    private void UpdateCameraSizeFromSlider(float value)
    {
        // 카메라 사이즈를 기본 배율 상수 * 슬라이드 배율로 변경
        cameraSize = DEFAULT_CAMERA_ZOOM * value;
        // 변경된 사이즈를 render 카메라에 적용
        renderCamera.orthographicSize = cameraSize;
    }

    // OnValueChanged 리스너에 등록된 함수
    // 슬라이더 값이 변경될 때 마다 호출된다.
    // Rotate 슬라이더 값에 따라 오브젝트의 Rotation를 변경하는 함수
    // value라는 변경된 값을 매개변수로 받는다.
    private void UpdateObjectRotateFromSlider(float value)
    {
        // Rotate의 값을 보정해주는 함수를 호출
        float rotationY = AdjustRotation(value);
        // previewObject의 Rotate를 변경한다.
        previewObject.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    // ##################################################################################################
    // ▶[보조 메서드]
    // ##################################################################################################
    // Rotate의 값을 보정해주는 함수
    // 범위는 최소 180 ~ 최대 450이다.
    // 값의 기준이 되는 value의 범위는 0.0f ~ 1.0f
    private float AdjustRotation(float value)
    {
        // changeRotation 값을 (value * 배율) + 기본 Rotation으로 변경
        float changedRotatation = (value * DEFAULT_ROTATION_SCALE) + DEFAULT_ROTATION;

        // 변경된 changeRotation 값 반환
        return changedRotatation;
    }

    // 모든 카테고리의 최대 인덱스를 가져오는 함수
    private void GetCategoryMaxIndexs()
    {
        // dataDictionary에 저장된 카테고리별 최대 인덱스를 가져오는 함수를 호출
        partsListMaxIndexs = CustomizingManager_Choi.instance.GetMaxIndexForCategorys();
    }

    // Customizing_Manager를 통해 저장된 모든 파츠 인덱스들을
    // PlayerPrefab에서 배열로 가져온 후 partsListIndexs[]에 저장하는 함수
    public void GetIndexsForPlayerPrefab()
    {
        // CustomizingManager를 통해 PlayerPrefab에 저장되어 있는
        // 모든 파츠 인덱스들을 partListIndex에 저장
        partsListIndexs = CustomizingManager_Choi.instance.GetAllDataForPlayerPrefabs();
    }
}
