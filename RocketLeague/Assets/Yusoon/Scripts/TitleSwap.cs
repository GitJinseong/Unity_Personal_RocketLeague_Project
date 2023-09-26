using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSwap : MonoBehaviour
{
    public GameObject vcam1;
    public GameObject vcam2;
    public Canvas titleCanvas;
    public GameObject mainCanvas;
    bool introSkip=false;
    // Start is called before the first frame update
    void Start()
    {
        // isSkip이 true일 경우
        if (SceneData_Choi.isSkip == true)
        {
            // 스킵
            introSkip = true;
            vcam1.SetActive(true);
            vcam2.SetActive(false);
            titleCanvas.enabled = false;
            mainCanvas.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            if(!introSkip)
            {
             //PlayerPrefs.DeleteAll();


           introSkip = true;   
            vcam1.SetActive(true);
            vcam2.SetActive(false);
            titleCanvas.enabled=false;
            StartCoroutine(SwapRoutine());
            }
        }


    }
    private IEnumerator SwapRoutine()
    {
        yield return new WaitForSeconds(2);
        mainCanvas.SetActive(true);
        // 타이틀 스킵을 위해 SceneData.isSkip 값을 true로 변경
        SceneData_Choi.isSkip = true;
    }
}
