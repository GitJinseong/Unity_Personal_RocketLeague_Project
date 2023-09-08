using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBooster_Yoo : MonoBehaviour
{
    public float boost { get; private set; }
    public float boostSpeed;

    private BoostUI_Yoo boostUI;
    private bool useBoost;
    private Rigidbody carRigid;
    private car car;
    // Start is called before the first frame update
    void Start()
    {
        boost = 33;
        boostSpeed = 100f;
        useBoost = false;
        carRigid = GetComponent<Rigidbody>();
        car = GetComponent<car>();
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
        Debug.Log("현재 부스터 게이지: " +  boost);
        boostUI.SetBoostGauge(boost);

        if(Input.GetMouseButtonDown(0))
        {
            if(boost == 0)
            {
                return;
            }
            useBoost = true;
        }

        if(Input.GetMouseButtonUp(0)) 
        {
            useBoost = false;
        }
    }

    public void AddBoost(int boostGauge)
    {
        if(boost == 100)
        {
            return;
        }

        boost += boostGauge;

        if(boost > 100)
        {
            boost = 100;
        }
    }

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
