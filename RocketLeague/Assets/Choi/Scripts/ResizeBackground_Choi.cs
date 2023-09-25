using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeBackground_Choi : MonoBehaviour
{
    // 이미지의 기본 가로/세로 값과 비율을 저장하는 변수 선언
    private float imageWidth, imageHeight, imageAspectRatio;

    void Start()
    {
        // 이미지의 기본 정보를 저장하기 위해 랙트 트랜스폼 가져옴
        RectTransform rectTransform = GetComponent<RectTransform>();

        // 이미지의 기본 정보를 저장
        imageWidth = rectTransform.rect.width;
        imageHeight = rectTransform.rect.height;

        // 이미지 (가로 / 세로)로 비율을 계산 후 저장
        imageAspectRatio = imageWidth / imageHeight;

        // 클라이언트 가로 값을 받아옴
        float clientWidth = Screen.width;

        // 클라이언트의 가로 값에 맞춰 이미지 rectTransform의
        // 가로/세로 크기를 조정
        Vector2 resizeSizeDelta = rectTransform.sizeDelta;
        resizeSizeDelta.x = clientWidth;
        // 가로 기준으로 비율을 맞추기 위해
        // clientWidth / imageAspectRatio로 계산
        resizeSizeDelta.y = clientWidth / imageAspectRatio;

        // 변경된 크기를 적용
        rectTransform.sizeDelta = resizeSizeDelta;
    }
}
