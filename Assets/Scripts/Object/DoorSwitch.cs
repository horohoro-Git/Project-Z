using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    [SerializeField]
    Door door;
    Vector3 origin;
    Vector3 current;
    Quaternion originQua;

   

    [SerializeField]
    AnimationClip openAnimation_Horizontal; 
    [SerializeField]
    AnimationClip openAnimation_Vertical;

    bool isOpen = false;
    bool isWorking = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            pc.RegisterAction(DoorInteraction);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            pc.RemoveLastAction(DoorInteraction);
        }
    }

    public void Start()
    {
      
    }

    void DoorInteraction(PlayerController pc)
    {
        //if(door.GetAnimation.GetInteger("state") != 0) return;
        if (isWorking) return;
        if (isOpen == false)
        {
            isOpen = true;
            OpenDoor(pc.Transforms.position);
        }
        else
        {
            isOpen = false;
            CloseDoor();
        }
    }

    void OpenDoor(Vector3 player)
    {
        isWorking = true;
        door.BoxCol.enabled = false;
        Vector3 vector3 = player - transform.position;
        vector3 = vector3.normalized;
        if (door.isHorizontal)
        {
            if (vector3.x > 0)
            {
                door.GetAnimation.SetInteger("state", 2);
             //   Debug.Log("오른쪽");
            }
            else
            {
                door.GetAnimation.SetInteger("state", 1);
            //    Debug.Log("왼쪽");
            }
        }
        else
        {
            if (vector3.z > 0)
            {
                door.GetAnimation.SetInteger("state", 1);
            //    Debug.Log("위");
            }
            else
            {
                door.GetAnimation.SetInteger("state", 2);
            //    Debug.Log("아래");
            }
        }
        Invoke("DoorTimer", 1);
    }



    void CloseDoor()
    {
        isWorking = true;
        door.GetAnimation.SetInteger("state", 3);
        Invoke("DoorTimer", 1);
    }


    void DoorTimer()
    {
        isWorking = false;
        if(!isOpen)
        {
            door.BoxCol.enabled = true;
        }
    }
  /*//  IEnumerator OpenDoor()
    {

     //   door.GetComponent<BoxCollider>().enabled = false;
   *//*     Vector3 dir = door.transform.position - current;
      //  door.transform.rotation = Quaternion.Euler(-90, 0, 0);
        Quaternion targetQua = Quaternion.Euler(-90, 0, 0);
        float elapsedTime = 0f;
        float openSpeed = 1f;
      //  Vector3 targetPosition = origin + new Vector3(0.7f, 0, -0.8f);

       // door.transform.Translate(door.transform.position.x + 0.7f, door.transform.position.y - 0.8f, door.transform.position.z, Space.Self);
        while (elapsedTime < openSpeed)
        {
            float t = elapsedTime / openSpeed;

            elapsedTime += Time.deltaTime;
     //       door.transform.position = Vector3.Lerp(origin, targetPosition, t);
            door.transform.rotation = Quaternion.Lerp(originQua, targetQua, t);

            yield return null;
        }
  //      door.transform.position = targetPosition;
        door.transform.rotation = targetQua;*//*
    }*/
}
