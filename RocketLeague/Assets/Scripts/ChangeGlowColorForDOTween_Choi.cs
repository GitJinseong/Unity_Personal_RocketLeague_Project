using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGlowColorForDOTween_Choi : MonoBehaviour
{
    // 지정된 시간동안 TMP_Text의 Glow 색상을 변경하는 함수
    public static void DOColor(Material material, int id, Color targetColor, float t)
    {
        DOTween.To(() => material.GetColor(id),
            color => material.SetColor(id, color),
            targetColor, t);
    }
}
