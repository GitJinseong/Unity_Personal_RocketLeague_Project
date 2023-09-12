using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCar_Yoo : MonoBehaviour
{
    public List<GameObject> cars;
    public GameObject car;
    public float dis;

    public GameObject targetCar { get; private set; }
    public Vector3 targetDir { get; private set; }
    public Rigidbody targetRigid { get; private set; }
    public float pushPower { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        cars = new List<GameObject>();
        car = null;
        targetCar = null;
        dis = 0;
        targetDir = new Vector3();
        targetRigid = null;
        pushPower = 2500000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (cars.Count == 0)
        {
            targetCar = null;
            targetRigid = null;
            targetDir = Vector3.zero;
        }

        if (cars.Count != 0)
        {
            for (int i = 0; i < cars.Count; i++)
            {
                float tempDis;
                tempDis = Vector3.Distance(transform.position, cars[i].transform.GetChild(1).position);
                //if (i == 0)
                //    Debug.Log("1번" + tempDis);
                //if(i == 1)
                //    Debug.Log("2번" + tempDis);
                if (dis == 0)
                {
                    //Debug.Log("처음 찾은 차 인식");
                    targetCar = cars[i];
                }

                if (tempDis < dis)
                {
                    //Debug.Log("다시 비교하긴함?");
                    targetCar = cars[i];
                }
            }
        }

        if (targetCar != null)
        {
            dis = Vector3.Distance(transform.position, targetCar.transform.GetChild(1).position);
            targetDir = (targetCar.transform.GetChild(1).position - transform.position).normalized;
            targetRigid = targetCar.transform.GetChild(0).gameObject.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Car"))
        {
            //Debug.Log("여기 엔터 들어옴?");
            if(collision.gameObject.transform.parent.gameObject != transform.parent.gameObject)
            {
                car = collision.gameObject.transform.parent.gameObject;
                cars.Add(car);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Car"))
        {
            //Debug.Log("여기 나가기 들어옴?");
            car = collision.gameObject.transform.parent.gameObject;
            foreach (GameObject gameObject in cars)
            {
                if (car == gameObject)
                {
                    cars.Remove(gameObject);
                }
            }
        }
    }
}
