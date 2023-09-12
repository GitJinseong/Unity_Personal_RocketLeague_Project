using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBurst_Yoo : MonoBehaviour
{
    public NewCar_Yoo myCar;
    public GameObject myCarObject;
    public List<NewCar_Yoo> otherCars;
    public List<GameObject> otherCarObjects;
    // Start is called before the first frame update
    void Start()
    {
        myCar = transform.parent.GetComponentInChildren<NewCar_Yoo>();
        myCarObject = transform.parent.gameObject;
        otherCars = new List<NewCar_Yoo>();
        otherCarObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(otherCars.Count != 0)
        {
            for(int i = 0; i < otherCars.Count; i++)
            {
                //if (otherCars[i].powerUp == true)
                //{
                    
                //}

                if(myCar.powerUp == true)
                {
                    for(int j = 0; j < otherCarObjects[i].transform.childCount; j++)
                    {
                        otherCarObjects[i].transform.GetChild(j).gameObject.SetActive(false);
                    }
                    otherCarObjects.RemoveAt(i);
                    otherCars.RemoveAt(i);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Car"))
        {
            if(collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                otherCars.Add(collision.transform.parent.GetComponentInChildren<NewCar_Yoo>());
                otherCarObjects.Add(collision.transform.parent.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Car"))
        {
            if (collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                for(int i = 0; i < otherCars.Count; i ++)
                {
                    if (otherCars[i] == collision.transform.parent.GetComponentInChildren<NewCar_Yoo>())
                    { 
                        otherCars.RemoveAt(i);
                        otherCarObjects.RemoveAt(i);
                    }
                }
            }
        }
    }
}
