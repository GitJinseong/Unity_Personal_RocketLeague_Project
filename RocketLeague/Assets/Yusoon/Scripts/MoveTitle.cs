using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveTitle : MonoBehaviour
{
    public RectTransform titleRect;
    Vector3 initPosition;
    Vector3 titlePosition = new Vector3(30, -200, 0);
    Vector3 currentPosition=new Vector3(30,600,0);
    // Start is called before the first frame update
    void Start()
    {
        initPosition=titleRect.position;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.deltaTime*2;
        currentPosition = Vector3.Lerp(currentPosition, titlePosition, t);

        // titleRect의 위치를 업데이트합니다.
        titleRect.anchoredPosition = currentPosition;

    }
}
