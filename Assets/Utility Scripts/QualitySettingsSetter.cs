using UnityEngine;

public class QualitySettingsSetter : MonoBehaviour
{
    public void SetQualitySettings(int qualityLevel)
    {
        Debug.Log($"[SetQualitySettings] SetQualitySettings {qualityLevel}");
        QualitySettings.SetQualityLevel(qualityLevel);
    }
}
