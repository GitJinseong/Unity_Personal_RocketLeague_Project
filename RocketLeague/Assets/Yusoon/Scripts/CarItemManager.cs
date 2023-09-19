using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Xml.Serialization;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Concurrent;

public class CarItemManager : MonoBehaviour
{
    public GameObject magnetParticle;
    public GameObject PowerUpParticle;
    public Animator kickAnimator;
    public Animator punchAnimator;
    public Transform kickTransform;
    public Transform punchTransform;
    public GameObject myCollider;
    public GameObject skillSlider;
    public Image skillSliderbg;
    public TMP_Text cooltimeText;
    public float skillCoolTime;
    public Image[] skillIcon;
    private string targetTag;
    bool skillActive = false;
    float radius = 50f;
    Vector3 newPosition;
    private Collider nearestCollider;
    private float coolTimeMax = 10f;
    Rigidbody ballRigidBody;
    Vector3 ballDir;
    LineRenderer lineRenderer;
    const int MAGNET = 0;
    const int KICK = 1;
    const int PUNCH = 2;
    const int ICE = 3;
    const int ENEMY_BOOST = 4;
    const int POWER_UP = 5;


    float currentCooltime;
    int randomIdx;
    public bool magnetOn = false;
    bool isPunch = false;
    Vector3[] linePoints = new Vector3[2];
    // Start is called before the first frame update
    void Start()
    {

        skillCoolTime=0;
        StartCoroutine(SkillCoolTimeRoutine());
        if (myCollider.tag=="OrangeCar")
        {
            targetTag="BlueCar";

        }
        else
        {
            targetTag="OrangeCar";

        }
        DeActiveIcons();
        lineRenderer=GetComponent<LineRenderer>();
        lineRenderer.enabled=false;
    }

    // Update is called once per frame
    void Update()
    {
        
        linePoints[0] = transform.position;
        linePoints[0].y=4;
        lineRenderer.SetPosition(0, linePoints[0]);

        currentCooltime=coolTimeMax- skillCoolTime;
        cooltimeText.text = currentCooltime.ToString();
        skillSliderbg.fillAmount = skillCoolTime/coolTimeMax;

        newPosition =transform.position;
        newPosition.y=2.4f;
        Collider[] colliders = Physics.OverlapSphere(newPosition, radius);

        if(magnetOn)
        {
            lineRenderer.enabled=true;
        }
        else
        {
            lineRenderer.enabled=false;

        }
        if (skillCoolTime==10)
        {
            skillSlider.SetActive(false);

            skillActive = true;
        }
        else
        {
            skillActive = false;

        }
        if(skillActive)
        {    
            if(randomIdx==POWER_UP)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {

                    PowerUpParticle.SetActive(true);
                StartCoroutine(SkillUseCoolTime(5));
                }
            }
            }
        float nearestDistance = float.MaxValue;
        foreach (Collider col in colliders)
        {
            if (myCollider.tag==col.tag) continue;
            if (col.tag==targetTag)
            {
                //Debug.LogFormat("내 태그 {0} 상대 태그 {1}", myCollider.tag, col.tag);

                float distance = Vector3.Distance(newPosition, col.transform.position);
                if (distance < nearestDistance)
                {
                    nearestCollider = col;
                    nearestDistance = distance;
                }
                if (nearestCollider != null)
                {
                    //Debug.LogFormat("내 태그 {0}, 가장 가까운 상대 태그 {1}", myCollider.tag, nearestCollider.tag);
                    Rigidbody rb = nearestCollider.GetComponent<Rigidbody>();
                    Vector3 dir = (col.transform.position-transform.position).normalized;
                    if (rb != null)
                    {
                        if (skillActive)
                        {

                            if (Input.GetKeyDown(KeyCode.R))
                            {

                                switch (randomIdx)

                                {
                                   

                                    case KICK:
                                        kickTransform.LookAt(col.transform.position);
                                        kickAnimator.Play("KickAnimation");
                                        rb.AddForce(dir*350, ForceMode.Impulse);
                                        StartCoroutine(SkillUseCoolTime(1));
                                        break;                                
                                    case ENEMY_BOOST:
                                        CarParent colParent=col.GetComponentInParent<CarParent>();
                                        if(colParent != null)
                                        {
                                            NewCar newCar=colParent.GetComponentInChildren<NewCar>();
                                            if(newCar != null)
                                            {
                                                newCar.outOfControl=true;
                                        StartCoroutine(SkillUseCoolTime(5));

                                            }
                                        }
                                        break;
                                

                                }


                            }
                        }

                    }
                    // nearestCollider를 사용하여 필요한 작업을 수행할 수 있습니다.
                }
            }



        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballRigidBody=other.GetComponent<Rigidbody>();
            ballDir=(other.transform.position - transform.position).normalized;
            if (ballRigidBody != null)
            {
            }

                linePoints[1]=other.transform.position;
                lineRenderer.SetPosition(1, linePoints[1]);
            if (magnetOn)
            {
                
                ballRigidBody.AddForce(-ballDir*30);
            }
            
            if (skillActive)
            {

                if (randomIdx==PUNCH)
                {
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        if(isPunch==false)
                        {
                            isPunch=true;
                        punchAnimator.Play("PunchAnimation");
                        punchTransform.LookAt(other.transform.position);
                        ballRigidBody.velocity=Vector3.zero;
                        ballRigidBody.AddForce(ballDir*70, ForceMode.Impulse);
                        StartCoroutine(SkillUseCoolTime(1));
                        }
                    }
                }
                if (randomIdx==MAGNET)
                {

                    if (Input.GetKeyDown(KeyCode.R))
                    {

                        if (magnetOn==false)
                        {
                            magnetOn = true;
                            magnetParticle.SetActive(true);
                            StartCoroutine(SkillUseCoolTime(8));
                        }

                    }
                }
                if (randomIdx==ICE)
                {
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        Ball_Ys ball = other.GetComponent<Ball_Ys>();
                        if (ball!=null)
                        {
                            ball.FreezeBall();
                            StartCoroutine(SkillUseCoolTime(3));

                        }
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            lineRenderer.enabled=false;
        }
    }

    private void DeActiveIcons()
    {
        for (int i = 0; i<skillIcon.Length; i++)
        {
            skillIcon[i].enabled = false;
        }
    }
    private IEnumerator SkillUseCoolTime(int second)
    {
        yield return new WaitForSeconds(second);
        DeActiveIcons();
        skillSlider.SetActive(true);
        skillCoolTime=0;
        if(magnetParticle!=null)
        {
        magnetParticle.SetActive(false);

        }
        if(PowerUpParticle!=null)
        {
            PowerUpParticle.SetActive(false);

        }
        isPunch=false;
        magnetOn =false;
        StartCoroutine(SkillCoolTimeRoutine());

    }
    private IEnumerator SkillCoolTimeRoutine()
    {
        while (skillCoolTime<10)
        {
            yield return new WaitForSeconds(1);
            skillCoolTime+=1;
        }
        randomIdx = Random.Range(0, 6);
        
        skillIcon[randomIdx].enabled = true;
    }




    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(newPosition, radius);
    //}

}
