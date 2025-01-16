using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    const string debugAction = "F3";

    PlayerInput playerInput;
    Debugging debugging;

    [NonSerialized]
    public Vector3 worldPosition;
    public void Setup(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        if (debugging)
        {
            debugging = GameInstance.Instance.app.debugging;
            debugging.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if(debugging) playerInput.actions[debugAction].performed += DebugOn;
    }

    private void Update()
    {
         Vector2 mousePos = Input.mousePosition;
         worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
         Vector3 cameraPosition = Camera.main.transform.position;
        worldPosition -= cameraPosition;
       // worldPosition = Quaternion.Euler(-60,0,0) * worldPosition;
        worldPosition.y = 0;
         Debug.Log(worldPosition); 
        if(GameInstance.Instance.drawGrid )
        {
            GameInstance.Instance.drawGrid.Select(worldPosition);
        }
    }

    private void OnDisable()
    {
        if (playerInput != null && debugging)
        {
            playerInput.actions[debugAction].performed -= DebugOn;
        }
    }

    void DebugOn(InputAction.CallbackContext callback)
    {
        if (debugging.gameObject.activeSelf) debugging.gameObject.SetActive(false);
        else debugging.gameObject.SetActive(true);
    }
 
}
