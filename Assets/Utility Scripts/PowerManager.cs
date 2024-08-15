using UnityEngine;
using UnityEngine.Rendering;

public class PowerManager : MonoBehaviour
{
    public float powerSavingDelay = 3f;

    public int powerSavingFrameInterval = 3;


    public int powerSavingTargetFrameRate = 60;

    public int targetFrameRate = 0;

    [SerializeField] bool _disablePhysics;
    [SerializeField] bool _enableVSync;

    void Start()
    {
        Physics.autoSimulation = !_disablePhysics;
        lastActiveTime = Time.time;
    }

    public void EnablePowerSaving()
    {
        enabled = true;
    }

    public void DisablePowerSaving()
    {
        enabled = false;
    }

    void OnDisable()
    {
        StopPowerSaving();
    }

    public void ResetTimer()
    {
        lastActiveTime = Time.time;
    }

    public void SetDelay(float delay)
    {
        powerSavingDelay = delay;
    }

    public void SetPowerSavingInterval(int v)
    {
        powerSavingFrameInterval = v;
    }

    float lastActiveTime;
    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.mouseScrollDelta != Vector2.zero || Input.touchCount > 0)
        {
            lastActiveTime = Time.time;
        }

        if (Time.time - lastActiveTime < powerSavingDelay)
        {
            StopPowerSaving();
        }
        else
        {
            StartPowerSaving();
        }
    }

    bool isInPowerSavingMode;
    public void StartPowerSaving()
    {
        if (!isInPowerSavingMode)
        {
            isInPowerSavingMode = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = powerSavingTargetFrameRate;
            OnDemandRendering.renderFrameInterval = powerSavingFrameInterval;
            Debug.Log("StartPowerSaving");
        }
    }
    public void StopPowerSaving()
    {
        if (isInPowerSavingMode)
        {
            isInPowerSavingMode = false;
            QualitySettings.vSyncCount = _enableVSync ? 1 : 0;
            Application.targetFrameRate = targetFrameRate;
            OnDemandRendering.renderFrameInterval = 0;
            Debug.Log("StopPowerSaving");
        }
    }
}