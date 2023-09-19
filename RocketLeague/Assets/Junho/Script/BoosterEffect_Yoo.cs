using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterEffect_Yoo : MonoBehaviour
{
    NewCar_Yoo car;
    CarBooster_Yoo booster;
    public GameObject boostLevel1;
    public GameObject boostLevel2;
    // Start is called before the first frame update
    void Start()
    {
        car = GetComponent<NewCar_Yoo>();
        booster = GetComponent<CarBooster_Yoo>();
        boostLevel1 = transform.GetChild(0).GetChild(0).GetChild(6).gameObject;
        boostLevel2 = transform.GetChild(0).GetChild(0).GetChild(7).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(booster.useBoost == false && car.useSecondBoost == false && car.compulsionBoost == false)
        {
            boostLevel1.SetActive(false);
            boostLevel2.SetActive(false);
        }

        if(booster.useBoost == true)
        {
            boostLevel1.SetActive(true);
        }

        if(car.compulsionBoost == true)
        {
            boostLevel1.SetActive(true);
            boostLevel2.SetActive(true);
        }

        if(car.useSecondBoost == true)
        {
            boostLevel1.SetActive(true);
            boostLevel2.SetActive(true);
        }
    }
}
