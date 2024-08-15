using System.Collections.Generic;
using UnityEngine;

public class TransformSaveRestore : MonoBehaviour
{
    Dictionary<Transform, Vector3> savedPositions;

    [ContextMenu("SavePose")]
    public void SavePose()
    {
        // get all child transforms
        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        // Collect position of all transforms (declare this dict somewere where you can access it later)
        savedPositions = new Dictionary<Transform, Vector3>();
        foreach (Transform t in transforms)
            savedPositions.Add(t, t.localPosition);

        Debug.Log("Saved " + savedPositions.Count + " positions");
    }

    [ContextMenu("RestorePose")]
    public void RestorePose()
    {
        foreach (KeyValuePair<Transform, Vector3> entry in savedPositions)
        {
            entry.Key.localPosition = entry.Value;
        }
        Debug.Log("Restored " + savedPositions.Count + " positions");
    }
}
