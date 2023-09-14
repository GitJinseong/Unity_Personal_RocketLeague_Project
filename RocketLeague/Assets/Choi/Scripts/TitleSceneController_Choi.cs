using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController_Choi : MonoBehaviour
{
    [Header("TitleEffect")]
    public GameObject[] objs; // 아래와 같은 오브젝트를 인덱스에 설정
                              // [0] = Img_TitleBg, [1] = Img_TitleCompanyLogo


    void Start()
    {
        // 타이틀 배경 액션 함수 호출
        float[] actionTimesForTitleBg = {1f, 3f};
        StartCoroutine(DOActionTitleBg(actionTimesForTitleBg));

        // 회사 로고 액션 함수 호출
        float[] actionTimesForCompanyLogo = {7f, 2f, 1f, 4f, 1f};
        StartCoroutine(DOActionCompanyLogo(actionTimesForCompanyLogo));
    }

    // 타이틀 배경 액션 코루틴 함수
    private IEnumerator DOActionTitleBg(float[] times)
    {
        yield return new WaitForSeconds(times[0]);
        objs[0].SetActive(true); // 배경 활성화

        yield return new WaitForSeconds(times[1]);
        Image titleBg = objs[0].GetComponent<Image>();
        titleBg.DOFade(0f, 1f).SetDelay(1f); // 1초간 페이드 아웃
    }

    // 회사 로고 액션 코루틴 함수
    private IEnumerator DOActionCompanyLogo(float[] times)
    {
        yield return new WaitForSeconds(times[0]);
        objs[1].SetActive(true); // 로고 활성화

        Image companyLogo = objs[1].GetComponent<Image>();
        companyLogo.DOColor(Color.white, 2f); // 2초간 페이드 인 / DOFade로 페이드인시 실제로는 동작하나
                                              // 제대로 보이지 않는 현상으로 DOColor로 동작하게 변경
        yield return new WaitForSeconds(times[1]);
        Transform companyLogoTransform = objs[1].GetComponent<Transform>();
        companyLogoTransform.DOScale(new Vector3(1.05f, 1.05f, 1f), 3.0f); //3초간 스케일 x 1.05

        yield return new WaitForSeconds(times[2]);
        companyLogo.DOFade(0f, 1f).SetDelay(1f); // 1초간 페이드 아웃

        yield return new WaitForSeconds(times[3]);
        // 씬 로드
    }
}
