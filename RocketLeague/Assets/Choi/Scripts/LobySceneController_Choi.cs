using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobySceneController_Choi : MonoBehaviour
{
    [Header("LobyEffect")]
    public GameObject[] objs; // 아래와 같은 오브젝트를 인덱스에 설정
                              // [0] = Txt_StartMsg, [1] = Btn_Start, [2] = Img_GameLogo, [3] = Img_BlackBg
    private bool isStart = false;

    void Start()
    {
        // 시작 버튼 액션 함수 호출
        float[] actionTimesForStartMsg = {1f, 1f, 2f};
        StartCoroutine(DOActionStartMsg(actionTimesForStartMsg));

        // 타이틀 게임 로고 액션 함수 호출
        float[] actionTimesForGameLogo = {4f};
        StartCoroutine(DOActionGameLogo(actionTimesForGameLogo));
    }

    // 시작 버튼 액션 코루틴 함수
    private IEnumerator DOActionStartMsg(float[] times)
    {
        yield return new WaitForSeconds(times[0]);
        objs[1].SetActive(true); // 시작 버튼 활성화

        Image blackBg = objs[3].GetComponent<Image>();
        blackBg.DOFade(0f, 1f).SetDelay(2f); // 2초간 검은 배경 페이드 아웃

        TMP_Text startMsg = objs[0].GetComponent<TMP_Text>();
        startMsg.DOFade(1f, 1f).SetDelay(2f); // 2초간 시작 텍스트 페이드인

        yield return new WaitForSeconds(times[1]);
        RectTransform startMsgRectTransform = startMsg.rectTransform;
        startMsgRectTransform.DOAnchorPos(new Vector3(0f, 330f, 0f), 2f); // 2초간 앵커 PosY 330f로 이동

        yield return new WaitForSeconds(times[2]);

        // 시작하기 전까지 랜덤하게 시작 텍스트의 색상을 변경 
        Color randomColor = new Color(0f, 0f, 0f);

        // startMsg의 마테리얼과 쉐이더를 가져옴
        Material startMsgMaterial = startMsg.fontSharedMaterial;
        Shader startMsgShader = startMsgMaterial.shader;

        // Shader에서 Glow의 아이디를 찾음
        int startMsgGlowId = Shader.PropertyToID("_GlowColor");

        float colorChangeTime = 3f;
        while (isStart == false)
        {
            randomColor = new Color(Random.value, 
                Random.value, Random.value); // Random.value를 사용하여 0과 1사이의 랜덤한 소수 값을 얻음
            ChangeGlowColorForDOTween_Choi.DOColor(startMsgMaterial, startMsgGlowId,
                randomColor, colorChangeTime); //지정된 시간 동안 glow의 색상을 endColor로 변경 
            yield return new WaitForSeconds(colorChangeTime); // 위의 동작이 끝날 때 까지 대기
        }
    }

    // 게임 로고 액션 코루틴 함수
    private IEnumerator DOActionGameLogo(float[] times)
    {
        yield return new WaitForSeconds(times[0]);
        objs[2].SetActive(true); // 게임 로고 활성화

        Image gameLogo = objs[2].GetComponent<Image>();
        gameLogo.DOFade(1f, 1f).SetDelay(1f); // 1초간 페이드 인

        RectTransform gameLogoRectTramsform = gameLogo.GetComponent<RectTransform>();
        gameLogoRectTramsform.DOAnchorPos(new Vector3(0f, -380f, 0f), 2f); // 2초간 아래로 이동
    }
}
