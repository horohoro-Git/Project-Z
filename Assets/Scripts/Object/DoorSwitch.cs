using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    [SerializeField]
    GameObject door;
    Vector3 origin;
    Vector3 current;
    Quaternion originQua;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            current = other.transform.position;
            Debug.Log("A");
            StartCoroutine(OpenDoor());
        }
    }

    public void Start()
    {
        originQua = door.transform.rotation;
        origin = door.transform.position;
    }

    IEnumerator OpenDoor()
    {
        Vector3 dir = door.transform.position - current;
      //  door.transform.rotation = Quaternion.Euler(-90, 0, 0);
        Quaternion targetQua = Quaternion.Euler(-90, 0, 0);
        float elapsedTime = 0f;
        float openSpeed = 1f;
        Vector3 targetPosition = origin + new Vector3(0.7f, 0, -0.8f);

       // door.transform.Translate(door.transform.position.x + 0.7f, door.transform.position.y - 0.8f, door.transform.position.z, Space.Self);
        while (elapsedTime < openSpeed)
        {
            float t = elapsedTime / openSpeed;

            elapsedTime += Time.deltaTime;
            door.transform.position = Vector3.Lerp(origin, targetPosition, t);
            door.transform.rotation = Quaternion.Lerp(originQua, targetQua, t);

            yield return null;
        }
        door.transform.position = targetPosition;
        door.transform.rotation = targetQua;
    }
}
