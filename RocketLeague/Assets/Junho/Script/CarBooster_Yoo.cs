using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBooster_Yoo : MonoBehaviour
{
    public float boost { get; private set; }
    public float boostSpeed;

    public BoostUI_Yoo boostUI;
    public bool useBoost { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        boost = 33;
        boostSpeed = 100f;
        useBoost = false;
        boostUI = FindObjectOfType<BoostUI_Yoo>();
    }

    private void FixedUpdate()
    {
        if(useBoost)
        {
            UseBoost();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("현재 부스터 게이지: " +  boost);
        boostUI.SetBoostGauge(boost);

        if(Input.GetMouseButton(0))
        {
            if(boost == 0)
            {
                //useBoost = false;
                return;
            }
            useBoost = true;
        }

        if(Input.GetMouseButtonUp(0)) 
        {
            useBoost = false;
        }
    }

    // 부스터 게이지 충전 함수
    public void AddBoost(int boostGauge)
    {
        if (boost == 100)
        {
            return;
        }
        for (int i = 0; i < boostGauge; i ++)
        {
            boost += 1;
            if (boost > 100)
            {
                boost = 100;
            }
        }
    }

    // 부스터 게이지 사용 함수
    public void UseBoost()
    {
        if(boost == 0)
        {
            useBoost = false;
            return;
        }


        boost -= 1;
    }
}
