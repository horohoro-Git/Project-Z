using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCalculateTest : MonoBehaviour
{
    public GameObject target;
    MoveCalculator calculator = new MoveCalculator();
    // Start is called before the first frame update
    void Start()
    {

        Invoke("Delay", 2);
    }
    void Delay()
    {
        DebugLine(calculator.Calculate(transform.position, target.transform.position));
    }
    float timer = 0;
    List<Vector3> vector3s = new List<Vector3>();
    // Update is called once per frame
    void Update()
    {
       /* if(timer + 1 < Time.time)
        {
            timer = Time.time;
            moveTrace = calculator.Calculate(transform.position, target.transform.position);
           *//* if (moveTrace != null)
            {
                Vector3 dir = DebugLine(moveTrace) - transform.position;
                dir = dir.normalized;
                transform.Translate(dir * 100 * Time.deltaTime);
            }*//*
            
        }*/


    }


    IEnumerator MoveTest()
    {
        int index = 0;
       // Debug.Log(moveTrace.Count);
        // moveTrace.Pop();
        while (vector3s.Count > index)
        {
            Debug.Log("A");
            Vector3 v = vector3s[index];
           
//            Debug.Log(distance);
            while(true)
            {
                Vector3 movedDir= v- transform.position;
                float distance = movedDir.magnitude;
                movedDir.Normalize();
                if (distance < 0.1f)
                {
                    transform.position = v;
                    break;
                }
                else transform.Translate(movedDir * 5 * Time.deltaTime);
                yield return null;
            }
            index++;
        }
    }
    Vector3 DebugLine(Stack<Vector3> calculates)
    {
        Vector3 loc = transform.position;
        // Debug.Log(moveTrace.Count);
        //  calculates.Pop();
        Vector3 s = transform.position;
        while (calculates.Count > 0)
        {
          //  calculates.Pop();
            Vector3 toLoc = calculates.Pop();
            Vector3 dir = toLoc - loc;
            float dis = dir.magnitude;
            dir = Vector3.Normalize(dir);
            Debug.Log(toLoc); 
            Quaternion rotation = Quaternion.LookRotation(dir);
            //   if (Physics.CheckBox(loc, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6)) break;

            if (calculates.Count == 0)
            {
                vector3s.Add(toLoc);
                break;
            }
            //Debug.DrawLine(loc,toLoc, Color.red, 5);
            if (!Physics.BoxCast(loc, new Vector3(0.5f, 0.5f, 0.5f), dir, Quaternion.identity, dis, 1 << 6) && !Physics.CheckBox(s,new Vector3(0.5f,0.5f,0.5f), Quaternion.identity, 1<< 6))
            {
           //     Debug.DrawLine(loc, loc + dir * dis, Color.green, 5f);
                // Debug.Log("Hit");
            }
            else
            {
                Vector3 dir2 = toLoc - s;
                float dis2 = dir2.magnitude;
                dir2 = Vector3.Normalize(dir2);
              //  Debug.DrawLine(s, s + dir2 * dis2, Color.green, 5f);
                
                vector3s.Add(s);
                loc = toLoc;

               
            }
            s = toLoc;
           // loc = toLoc;
            // transform.position = toLoc;
        }


        int index = 0;
        Vector3 start = transform.position;
        while (vector3s.Count > index)
        {
            Vector3 target = vector3s[index];

            Debug.DrawLine(start, target, Color.red, 5);

            start = target;
            index++;
        }
        StartCoroutine(MoveTest());    
        return loc; 
    }
}
