using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSkill_Yoo : MonoBehaviour
{
    private const float CAR_POWERUP_TIME = 10f;
    private const float CAR_SUDDEN_ACCELERATION_TIME = 8f;
    private WaitForSeconds carPowerUpDuration = new WaitForSeconds(CAR_POWERUP_TIME);
    private WaitForSeconds carAccelerateDuration = new WaitForSeconds(CAR_SUDDEN_ACCELERATION_TIME);

    public CheckCar_Yoo checkCar;
    public string[] skills;
    public string skill;
    public NewCar_Yoo myCar;
    public NewCar_Yoo carTarget;
    public CarBooster_Yoo carBoosterTarget;
    public Rigidbody carRigidTarget;

    private float timeAfterUseSkill;
    private float getSkillDelay;
    // Start is called before the first frame update
    void Start()
    {
        myCar = GetComponent<NewCar_Yoo>();
        checkCar = GetComponent<CheckCar_Yoo>();
        skill = null;
        getSkillDelay = 10f;
        timeAfterUseSkill = 0;
        skills = new string[3];
        skills[0] = "차량강화";
        skills[1] = "차량발차기";
        skills[2] = "차량급발진";
        skill = skills[2];
    }

    // Update is called once per frame
    void Update()
    {
        if(skill == null)
        {
            timeAfterUseSkill += Time.deltaTime;
        }

        if(timeAfterUseSkill >= getSkillDelay)
        {
            skill = skills[Random.Range(0, skills.Length)];
            timeAfterUseSkill = 0;
        }

        if(checkCar.targetCar == null)
        {
            carTarget = null;
            carBoosterTarget = null;
            carRigidTarget = null;
        }

        if(checkCar.targetCar != null)
        {
            carTarget = checkCar.targetCar.transform.GetChild(1).GetComponent<NewCar_Yoo>();
            carBoosterTarget = checkCar.targetCar.transform.GetChild(1).GetComponent<CarBooster_Yoo>();
            carRigidTarget = checkCar.targetCar.transform.GetChild(0).GetComponent<Rigidbody>();
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(skill != null)
                {
                    if(skill == "차량강화")
                    {
                        StartCoroutine(CarPowerUp());
                    }

                    if (skill == "차량발차기")
                    {
                        KickCar();
                    }

                    if (skill == "차량급발진")
                    {
                        StartCoroutine(CarAccelerate());
                    }
                }
            }
        }
    }

    IEnumerator CarAccelerate()
    {
        skill = null;
        timeAfterUseSkill = 0;
        NewCar_Yoo carTargetSave = carTarget;
        carTargetSave.compulsionBoost = true;
        yield return carAccelerateDuration;
        carTargetSave.compulsionBoost = false;
        yield break;
    }

    IEnumerator CarPowerUp()
    {
        skill = null;
        timeAfterUseSkill = 0;
        myCar.powerUp = true;
        yield return carPowerUpDuration;
        myCar.powerUp = false;
        yield break;
    }

    void KickCar()
    {
        skill = null;
        timeAfterUseSkill = 0;
        carRigidTarget.AddForce(checkCar.targetDir * checkCar.pushPower);
    }
}
