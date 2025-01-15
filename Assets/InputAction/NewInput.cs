//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/InputAction/NewInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @NewInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @NewInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""NewInput"",
    ""maps"": [
        {
            ""name"": ""OnMove"",
            ""id"": ""d16730bd-0746-4758-938c-64a478cb87a9"",
            ""actions"": [
                {
                    ""name"": ""WASD"",
                    ""type"": ""Value"",
                    ""id"": ""30b22a76-75d0-477e-984a-0faa7db1aa79"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""36089e5f-9c70-4c26-b778-27bedde81249"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""29f0ba1b-ce4a-4202-b550-fa2975d2c71a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";keyboard;touch"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""aa67b948-90d2-4c07-8db1-55903af51255"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";keyboard;touch"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""02c1c577-cffc-4d3c-9b8c-8e2d51210e78"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";keyboard;touch"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b602dbcc-48fd-400c-b94b-d72c53926831"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";keyboard;touch"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""OnSprint"",
            ""id"": ""001d3432-2816-4fd8-9e51-5f0181c2deff"",
            ""actions"": [
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""c974f057-af7b-4687-88fe-2338e09a2bd1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3f8293af-7113-4de6-a310-ace8529c6e69"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";keyboard;touch"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""keyboard"",
            ""bindingGroup"": ""keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""touch"",
            ""bindingGroup"": ""touch"",
            ""devices"": []
        }
    ]
}");
        // OnMove
        m_OnMove = asset.FindActionMap("OnMove", throwIfNotFound: true);
        m_OnMove_WASD = m_OnMove.FindAction("WASD", throwIfNotFound: true);
        // OnSprint
        m_OnSprint = asset.FindActionMap("OnSprint", throwIfNotFound: true);
        m_OnSprint_Run = m_OnSprint.FindAction("Run", throwIfNotFound: true);
    }

    ~@NewInput()
    {
        UnityEngine.Debug.Assert(!m_OnMove.enabled, "This will cause a leak and performance issues, NewInput.OnMove.Disable() has not been called.");
        UnityEngine.Debug.Assert(!m_OnSprint.enabled, "This will cause a leak and performance issues, NewInput.OnSprint.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // OnMove
    private readonly InputActionMap m_OnMove;
    private List<IOnMoveActions> m_OnMoveActionsCallbackInterfaces = new List<IOnMoveActions>();
    private readonly InputAction m_OnMove_WASD;
    public struct OnMoveActions
    {
        private @NewInput m_Wrapper;
        public OnMoveActions(@NewInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @WASD => m_Wrapper.m_OnMove_WASD;
        public InputActionMap Get() { return m_Wrapper.m_OnMove; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OnMoveActions set) { return set.Get(); }
        public void AddCallbacks(IOnMoveActions instance)
        {
            if (instance == null || m_Wrapper.m_OnMoveActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_OnMoveActionsCallbackInterfaces.Add(instance);
            @WASD.started += instance.OnWASD;
            @WASD.performed += instance.OnWASD;
            @WASD.canceled += instance.OnWASD;
        }

        private void UnregisterCallbacks(IOnMoveActions instance)
        {
            @WASD.started -= instance.OnWASD;
            @WASD.performed -= instance.OnWASD;
            @WASD.canceled -= instance.OnWASD;
        }

        public void RemoveCallbacks(IOnMoveActions instance)
        {
            if (m_Wrapper.m_OnMoveActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IOnMoveActions instance)
        {
            foreach (var item in m_Wrapper.m_OnMoveActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_OnMoveActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public OnMoveActions @OnMove => new OnMoveActions(this);

    // OnSprint
    private readonly InputActionMap m_OnSprint;
    private List<IOnSprintActions> m_OnSprintActionsCallbackInterfaces = new List<IOnSprintActions>();
    private readonly InputAction m_OnSprint_Run;
    public struct OnSprintActions
    {
        private @NewInput m_Wrapper;
        public OnSprintActions(@NewInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Run => m_Wrapper.m_OnSprint_Run;
        public InputActionMap Get() { return m_Wrapper.m_OnSprint; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OnSprintActions set) { return set.Get(); }
        public void AddCallbacks(IOnSprintActions instance)
        {
            if (instance == null || m_Wrapper.m_OnSprintActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_OnSprintActionsCallbackInterfaces.Add(instance);
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
        }

        private void UnregisterCallbacks(IOnSprintActions instance)
        {
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
        }

        public void RemoveCallbacks(IOnSprintActions instance)
        {
            if (m_Wrapper.m_OnSprintActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IOnSprintActions instance)
        {
            foreach (var item in m_Wrapper.m_OnSprintActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_OnSprintActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public OnSprintActions @OnSprint => new OnSprintActions(this);
    private int m_keyboardSchemeIndex = -1;
    public InputControlScheme keyboardScheme
    {
        get
        {
            if (m_keyboardSchemeIndex == -1) m_keyboardSchemeIndex = asset.FindControlSchemeIndex("keyboard");
            return asset.controlSchemes[m_keyboardSchemeIndex];
        }
    }
    private int m_touchSchemeIndex = -1;
    public InputControlScheme touchScheme
    {
        get
        {
            if (m_touchSchemeIndex == -1) m_touchSchemeIndex = asset.FindControlSchemeIndex("touch");
            return asset.controlSchemes[m_touchSchemeIndex];
        }
    }
    public interface IOnMoveActions
    {
        void OnWASD(InputAction.CallbackContext context);
    }
    public interface IOnSprintActions
    {
        void OnRun(InputAction.CallbackContext context);
    }
}
