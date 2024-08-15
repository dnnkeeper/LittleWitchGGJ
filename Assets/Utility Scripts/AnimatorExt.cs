using UnityEngine;

public class AnimatorExt : MonoBehaviour
{
    public void SetBoolTrue(string paramName)
    {
        GetComponent<Animator>().SetBool(paramName, true);
    }
    public void SetBoolFalse(string paramName)
    {
        GetComponent<Animator>().SetBool(paramName, false);
    }

    public void ToggleBool(string paramName)
    {
        GetComponent<Animator>().SetBool(paramName, !GetComponent<Animator>().GetBool(paramName));
    }
}
