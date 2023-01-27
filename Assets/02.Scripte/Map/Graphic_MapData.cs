using MapSample;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Graphic_MapData : MonoBehaviour
{
    public MapData mapData = null;
    void Start()
    {
        GameManager._instance._mapManager.SettingCountMapData(mapData);

    }
}
