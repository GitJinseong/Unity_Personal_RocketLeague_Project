using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class BoostUI_Yoo : MonoBehaviourPun
{
    public float boost { get; private set; }
    private int boostInt;
    private string boostString;
    private TMP_Text boostText;
    private Image boostImage;
    // Start is called before the first frame update
    void Start()
    {
        boostImage = transform.GetChild(1).GetComponent<Image>();
        boostText = transform.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>();
        boost = 33;
        boostImage.fillAmount = 0.65f * (boost / 100);
        boostInt = (int)boost;
        boostString = boostInt.ToString();
        boostText.text = boostString;
    }

    // Update is called once per frame
    void Update()
    {
        boostImage.fillAmount = 0.65f * (boost / 100);

        boostInt = (int)boost;
        boostString = boostInt.ToString();
        boostText.text = boostString;
    }

    public void SetBoost(float nBoost)
    {
        boost = nBoost;
    }
}
