using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizingSceneController_Choi : MonoBehaviour
{
    [Header("Preview")]
    public Slider zoomSlider; // 줌 슬라이드
    public Slider rotationSlider; // 로테이션 슬라이드
    public Camera renderCamera; // 프리뷰 랜더 카메라
    public GameObject previewObject; // 프리뷰 오브젝트
    private float cameraSize = 10.0f; // 기본 카메라 사이즈 10.0f
    private const float DEFAULT_CAMERA_ZOOM = 20.0f; // 기본 카메라 배율
    private const float DEFAULT_ROTATION = 90.0f; // 기본 Rotation 값
    private const float DEFAULT_ROTATION_SCALE = 360f; // 기본 Rotation 배율

    [Header("PartsList")]
    private int[] partsListIndexs;

    void Start()
    {
        // 슬라이더 값이 변경될 때 마다 함수가 호출되게 하기 위해
        // 이벤트 리스너 등록(onValueChanged)
        zoomSlider.onValueChanged.AddListener(UpdateCameraSizeFromSlider);
        rotationSlider.onValueChanged.AddListener(UpdateObjectRotateFromSlider);
    }


    // ##################################################################################################
    // ▶[버튼 메서드]
    // ##################################################################################################


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
        float rotationY = AdjustRotation(value);
        // previewObject의 Rotate를 변경한다.
        previewObject.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    // ##################################################################################################
    // ▶[보조 메서드]
    // ##################################################################################################
    // Rotate의 값을 보정해주는 함수
    // 범위는 최소 90 ~ 최대 450이다.
    // 값의 기준이 되는 value의 범위는 0.0f ~ 1.0f
    private float AdjustRotation(float value)
    {
        // changeRotation 값을 (value * 배율) + 기본 Rotation으로 변경
        float changedRotatation = (value * DEFAULT_ROTATION_SCALE) + DEFAULT_ROTATION;

        // 변경된 changeRotation 값 반환
        return changedRotatation;
    }
}
