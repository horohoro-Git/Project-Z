using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    PlayerInput playerInput;
    public void Setup(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
    }

}
