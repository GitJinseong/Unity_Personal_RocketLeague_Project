using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoostUI_Yoo : MonoBehaviour
{
    public float boost { get; private set; }

    private string boostString;
    private TMP_Text boostText;
    private Image boostImage;
    // Start is called before the first frame update
    void Start()
    {
        boostImage = transform.GetChild(1).GetComponent<Image>();
        boostText = transform.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        boostImage.fillAmount = 0.65f * (boost / 100);
        boostString = boost.ToString();
        boostText.text = boostString;
    }

    public void SetBoostGauge(float gauge)
    {
        boost = gauge;
    }
}
