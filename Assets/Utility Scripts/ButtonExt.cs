using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonExt : MonoBehaviour
{
    public void Invoke()
    {
        GetComponent<Button>().onClick.Invoke();
    }
}
