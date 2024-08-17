using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectsCollection : MonoBehaviour
{
    public PositionOnMap3D[] mapObjects;

    private void Reset()
    {
        mapObjects = GetComponentsInChildren<PositionOnMap3D>();
    }

}
