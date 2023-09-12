using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGrab_Car : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody rb_;
    Vector3 dir;
    Vector3[] linePoints = new Vector3[2]; 
    // Start is called before the first frame update
    bool isGrab = false;
    void Start()
    {
        lineRenderer=GetComponent<LineRenderer>();


    }

    // Update is called once per frame
    void Update()
    {
        linePoints[0] = transform.position;
        linePoints[0].y=4;
        lineRenderer.SetPosition(0,linePoints[0]);

        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Ball_Ys ball = other.GetComponent<Ball_Ys>();
            if (ball!=null)
            {
                Renderer renderer = ball.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color= Color.blue;
                }
                linePoints[1]=other.transform.position;
                lineRenderer.SetPosition(1, linePoints[1]);
                if (isGrab)
                {
                    lineRenderer.enabled=true;
                    rb_.AddForce(-dir*30);

                }
                else
                {
                    lineRenderer.enabled=false;
                }

                dir = (other.transform.position-transform.position).normalized;
                if (Input.GetKeyDown(KeyCode.R))
                {
                    rb_ = ball.GetComponent<Rigidbody>();
                    Debug.Log("BallGrab_Carµø¿€¡ﬂ");
                    dir.y=dir.y-0.1f;
                    if (rb_!= null)
                    {
                       if(isGrab==false)
                        {
                            isGrab=true;
                            StartCoroutine(GrabBallRoutine());
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
            Ball_Ys ball = other.GetComponent<Ball_Ys>();
            if (ball!=null)
            {
                Renderer renderer = ball.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color= Color.white;
                }
                lineRenderer.enabled=false;
            }
        }
    }
    private IEnumerator GrabBallRoutine()
    {
        yield return new WaitForSeconds(5);
        isGrab = false;
    }
}
