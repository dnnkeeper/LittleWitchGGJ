using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InputAxisEventTrigger : MonoBehaviour
{
    [SerializeField]
    float axisValue;
#if ENABLE_INPUT_SYSTEM

    public PlayerInput playerInput;

#endif
    public string actionName = "Sprint";

    //When input performed. Invoked on Update with LegacyInput
    public UnityEvent<float> onInputPerformed;

    //Triggered when changed from True to False or vice versa
    public UnityEvent<bool> onInputTriggered;

    //When it treiggered with True
    public UnityEvent onInputButton;


    bool lastTrigger;

    public void SetLocked(bool v)
    {
        Debug.LogError("Deprecated call: SetLocked on InputAxisEventTrigger", this);
        // if (v){
        //     enabled = true;
        // }
        // else{
        //     enabled = false;
        // }
    }


    // #if ENABLE_INPUT_SYSTEM
    // void OnEnable(){
    //     playerInput.actions[actionName].performed += OnPerformed;
    // }
    // void OnDisable(){
    //     playerInput.actions[actionName].performed -= OnPerformed;
    // }
    // void OnPerformed(InputAction.CallbackContext context){
    //     axisValue = context.ReadValue<float>();
    //     Debug.Log(actionName+" performed "+axisValue);
    //     onInputPerformed.Invoke( axisValue ); 
    // }
    // #endif
    //     void Awake(){
    // #if ENABLE_INPUT_SYSTEM
    //         Debug.Log("[InputAxisTreigger] NEW_INPUT_SYSTEM " + actionName, this);
    // #else
    //         Debug.Log("[InputAxisTreigger] OLD_INPUT_SYSTEM " + actionName, this);
    // #endif
    //     }
    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        axisValue = playerInput.actions[actionName].ReadValue<float>();
        onInputPerformed.Invoke(axisValue);
        bool trigger = axisValue > 0;
        if (trigger != lastTrigger)
        {
            onInputTriggered.Invoke(trigger);
            if (trigger)
                onInputButton.Invoke();
            lastTrigger = trigger;
        }
#else
        axisValue = Input.GetAxis(actionName);
        onInputPerformed.Invoke( axisValue );
        bool trigger = axisValue > 0;
        if (trigger != lastTrigger){
            onInputTriggered.Invoke(trigger);
            if (trigger)
                onInputButton.Invoke();
            lastTrigger = trigger;
        }
#endif
    }
}
