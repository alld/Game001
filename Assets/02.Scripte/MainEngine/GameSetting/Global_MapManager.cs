using MapSample;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Global_MapManager : MonoBehaviour
{
    public Material[] materials; // 임시 테스트용

    /// <summary>
    /// 한 장소이 가지고있는 고유 특징
    /// </summary>
    public enum eMapTileKind
    {
        None = 0,
        /// <summary> 플레이어 </summary>
        Player,
        /// <summary> 마을 </summary>
        Village,
        /// <summary> 숲 </summary>
        Forest,
        /// <summary> 사막 </summary>
        Desert,
        /// <summary> 평원 </summary>
        Plain,
        /// <summary> 물 </summary>
        Water,
        /// <summary> 용암 </summary>
        Lava,
        /// <summary> 늪 </summary>
        Swampy,
        /// <summary> 암석 </summary>
        Rock
    }
    /// <summary>
    /// 여러 장소를 특징지어 나타낼수있는 특징
    /// </summary>
    public enum eMapAreaKind
    {
        None,
        /// <summary> 호수 </summary>
        Lake,
        /// <summary> 강 </summary>
        River,
        /// <summary> 바다 </summary>
        Sea,
        /// <summary> 초원 </summary>
        Pasture,
        /// <summary> 밀림 </summary>
        Jungle,
        /// <summary> 늪지대 </summary>
        Marsh,
        /// <summary> 암석지대 </summary>
        rocky,
        /// <summary> 화산 </summary>
        Volcano,
        /// <summary> 동굴 </summary>
        Cave
    }
    public GameObject prefab_MapData;
    public List<MapData> MapDataList = new List<MapData>();
    public bool _editorSetting;
    public int _initMapPointCount = 10;
    public List<MapData> DungeonList = new List<MapData>();

    public MapData[,] MapDataXY = new MapData[MapSizeX, MapSizeY];

    public const int MapSizeX = 11;
    public const int MapSizeY = 11;

    public eMapAreaKind[] StandardMap_1th = new eMapAreaKind[3];
    private Dictionary<eMapAreaKind, int> StandardMap_2th = new Dictionary<eMapAreaKind, int>();
    private List<eMapTileKind> StandardMap_3th = new List<eMapTileKind>();

    public bool _completSetting = false;
    public bool _completReady = false;

    public Vector3 GetRegenPoint(bool isRandom = false, bool isRepeat = false, bool isdungeon = false)
    {
        int temp_int = 0;

        if (isdungeon == false)
        {
            if (isRandom == true) // 전체 랜덤
            {
                while (true)
                {
                    temp_int = Random.Range(0, MapDataList.Count);
                    if (isRepeat == false && MapDataList[temp_int]._isRegen == true)
                    {
                        continue;
                    }
                    if (MapDataList[temp_int]._thisMapTile != eMapTileKind.Player)
                    {
                        break;
                    }
                }
                MapDataList[temp_int]._isRegen = true;
                return MapDataList[temp_int]._thisRegenPoint.position;
            }
            else // 외곽 랜덤(물제외) // 수정요망
            {
                if(Random.Range(0,2) == 0)
                {
                    temp_int = Random.Range(0, MapSizeX);
                    MapDataXY[temp_int, 0]._isRegen = true;
                    return MapDataXY[temp_int, 0]._thisRegenPoint.position;
                }
                else
                {
                    temp_int = Random.Range(0, MapSizeY);
                    MapDataXY[0, temp_int]._isRegen = true;
                    return MapDataXY[0, temp_int]._thisRegenPoint.position;
                }
            }
        }
        else
        {
            if (DungeonList.Count == 0) return Vector3.zero; // 던전이 1개도 없는 경우
            if (isRepeat == false) // 던전 중복 불가
            {
                bool checkDungeon = false;
                foreach (var mapData in DungeonList)
                {
                    if (mapData._isRegen == false) checkDungeon = true;
                }
                if (checkDungeon == true) // 던전중 아직 생성되지않은곳이 있는 경우
                {
                    while (true)
                    {
                        temp_int = Random.Range(0, DungeonList.Count);
                        if (DungeonList[temp_int]._isRegen == false) break;
                    }

                    MapDataList[temp_int]._isRegen = true;
                    return MapDataList[temp_int]._thisRegenPoint.position;
                }
                else // 기존 모든 던전에서 1회이상 생성이 된 경우
                {
                    temp_int = Random.Range(0, DungeonList.Count);
                    MapDataList[temp_int]._isRegen = true;
                    return MapDataList[temp_int]._thisRegenPoint.position;
                }
            }
            else // 던전 중복허용
            {
                temp_int = Random.Range(0, DungeonList.Count);
                DungeonList[temp_int]._isRegen = true;
                return DungeonList[temp_int]._thisRegenPoint.position;
            }
        }
    }

    public void WaveFieldReset()
    {
        foreach (var mapData in MapDataList)
        {
            mapData._isRegen = false;
        }
    }

    public void CreateField(Transform field)
    {
        GameObject temp_object;
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                temp_object = Instantiate(prefab_MapData, field);
                temp_object.name += string.Format("{0},{1}", x, y);
            }
        }
    }

    private void SetInitRandomTiledData()
    {
        for (int i = 0; i < StandardMap_1th.Length; i++)
        {
            StandardMap_2th.Add(StandardMap_1th[i], Random.Range(0, 100));
        }

        var mapKind_2th = StandardMap_2th.Max(maxSelect => maxSelect.Key);
        switch (mapKind_2th)
        {
            case eMapAreaKind.River:
                StandardMap_3th.Add(eMapTileKind.Water);
                StandardMap_3th.Add(eMapTileKind.Swampy);
                StandardMap_3th.Add(eMapTileKind.Forest);
                StandardMap_3th.Add(eMapTileKind.Plain);
                StandardMap_3th.Add(eMapTileKind.Village);
                StandardMap_3th.Add(eMapTileKind.Rock);
                StandardMap_3th.Add(eMapTileKind.Desert);
                StandardMap_3th.Add(eMapTileKind.Lava);
                break;
            case eMapAreaKind.Pasture:
                StandardMap_3th.Add(eMapTileKind.Plain);
                StandardMap_3th.Add(eMapTileKind.Forest);
                StandardMap_3th.Add(eMapTileKind.Water);
                StandardMap_3th.Add(eMapTileKind.Swampy);
                StandardMap_3th.Add(eMapTileKind.Rock);
                StandardMap_3th.Add(eMapTileKind.Village);
                StandardMap_3th.Add(eMapTileKind.Desert);
                StandardMap_3th.Add(eMapTileKind.Lava);
                break;
            case eMapAreaKind.Jungle:
                StandardMap_3th.Add(eMapTileKind.Forest);
                StandardMap_3th.Add(eMapTileKind.Swampy);
                StandardMap_3th.Add(eMapTileKind.Water);
                StandardMap_3th.Add(eMapTileKind.Rock);
                StandardMap_3th.Add(eMapTileKind.Plain);
                StandardMap_3th.Add(eMapTileKind.Desert);
                StandardMap_3th.Add(eMapTileKind.Village);
                StandardMap_3th.Add(eMapTileKind.Lava);
                break;
            case eMapAreaKind.rocky:
                StandardMap_3th.Add(eMapTileKind.Rock);
                StandardMap_3th.Add(eMapTileKind.Desert);
                StandardMap_3th.Add(eMapTileKind.Plain);
                StandardMap_3th.Add(eMapTileKind.Swampy);
                StandardMap_3th.Add(eMapTileKind.Water);
                StandardMap_3th.Add(eMapTileKind.Lava);
                StandardMap_3th.Add(eMapTileKind.Forest);
                StandardMap_3th.Add(eMapTileKind.Village);
                break;
        }

        SetRandomMapPoint();
    }

    private void SetRandomMapPoint()
    {
        int MapPointCount = _initMapPointCount;
        foreach (var mapKind in StandardMap_3th)
        {
            for (int count = 0; count < Random.Range(0, 3); count++)
            {
                while (!MapDataXY[Random.Range(0, MapSizeX), Random.Range(0, MapSizeY)].SetAddChangePoint(mapKind, 100)) ;
                MapPointCount--;
                if (MapPointCount == 0) return;
            }
        }
    }


    public void SettingCountMapData(MapData mapData)
    {
        MapDataList.Add(mapData);

        if (MapDataList.Count == MapSizeX * MapSizeY) StartCoroutine(SettingMapData());
    }

    protected void SetMapDataPosition()
    {
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                MapDataXY[x, y] = MapDataList[(x * MapSizeY) + y];
                MapDataXY[x, y].SetPosition(x, y);
                MapDataXY[x, y].SetMapDataCache();
            }
        }
    }

    protected IEnumerator SettingMapData()
    {
        yield return null;
        SetMapDataPosition();

        SetInitRandomTiledData();

        SetDungeon();

        SetPlayerPosition();

        _completSetting = true;
    }

    protected void SetPlayerPosition()
    {
        MapDataXY[(int)Mathf.Floor(MapSizeX * 0.5f), (int)Mathf.Floor(MapSizeY * 0.5f)].ChangeTiled(eMapTileKind.Player);
    }

    protected void SetDungeon()
    {
        foreach (var mapData in MapDataList)
        {
            if (mapData._isDungeon) DungeonList.Add(mapData);
        }
    }

    /*

    * 
    * 10의 랜덤 수치를 분배해서 자원, 특수 공간 배치됨
    * 
    * 10의 주요 지형 배치된 형태에따라 주변의 값을 변경시킴
    * 
    * 동일한 주요 지형이 인접하게 양쪽에 있을경우 해당 지형도 주요지형으로 변경
    * 동일한 주요지형이 3개이상 묶여있을경우 거대지구로 변경
    * 거대지구에서는 특별한 대상이 존재함. 
    * 
    * 물지형은 고립되어있으면 호수
    * 외부까지 연결되어있으면 바다
    * 물이 서로 연결되어있으면 강으로 분류함 
    * 
    * 지형의 종류는 늪지대, 화산지대, 사막지대, 초원, 물, 숲, 평원
    * 
    * 던전, 적기지, NPC마을, 
    *
    *
    *
    */
}

namespace MapSample
{
    using static Global_MapManager;

    [System.Serializable]
    public class MapData
    {

        public eMapTileKind _thisMapTile = eMapTileKind.None;
        public Transform _thisObject;
        public Transform _thisRegenPoint;
        public GameObject _mapFloor;
        public bool _isAssignAddPoint = false;
        public Dictionary<eMapTileKind, int> _thisTilePoint = new Dictionary<eMapTileKind, int>();
        private MapData[,] mm_MapData; // 맵 매니저의 캐시
        public bool _isDungeon;
        public bool _isBoundaryConnect;
        public bool _isRegen;

        public MeshRenderer tempMesh; // 임시 테스트용

        private float positionX;
        public float _positionX
        {
            get
            {
                return positionX;
            }
            set
            {
                positionX = value;
                _thisObject.position = new Vector3(value * 20, _thisObject.position.y, _thisObject.position.z);
                if (value == 0 || value == MapSizeX - 1) _isBoundaryConnect = true;
            }
        }

        private float positionY;
        public float _positionY
        {
            get
            {
                return positionY;
            }
            set
            {
                positionY = value;
                _thisObject.position = new Vector3(_thisObject.position.x, _thisObject.position.y, value * 20);
                if (value == 0 || value == MapSizeY - 1) _isBoundaryConnect = true;

            }
        }

        public void SetPosition(float x, float y)
        {
            _positionX = x;
            _positionY = y;
        }

        public void SetMapDataCache()
        {
            mm_MapData = GameManager._instance._mapManager.MapDataXY;
        }

        public bool SetAddChangePoint(eMapTileKind mapTileKind, int point)
        {
            if (_isAssignAddPoint == true) return false;
            _isAssignAddPoint = true;

            if (_thisTilePoint.TryAdd(mapTileKind, point) == false) _thisTilePoint[mapTileKind] += point;

            AddPointAdjacentTile(mapTileKind, (int)(point * 0.5));
            ChangeTiled();
            return true;
        }

        private void AddPointAdjacentTile(eMapTileKind mapTileKind, int addPoint)
        {
            EachAddPointAdjacentTile((int)_positionX - 1, (int)_positionY, mapTileKind, addPoint);
            EachAddPointAdjacentTile((int)_positionX + 1, (int)_positionY, mapTileKind, addPoint);
            EachAddPointAdjacentTile((int)_positionX, (int)_positionY - 1, mapTileKind, addPoint);
            EachAddPointAdjacentTile((int)_positionX, (int)_positionY + 1, mapTileKind, addPoint);
        }

        private void EachAddPointAdjacentTile(int posX, int posY, eMapTileKind mapTileKind, int addPoint)
        {
            if (TryGetPosition(posX, posY))
            {
                mm_MapData[posX, posY].SetAddPoint(mapTileKind, addPoint);
                mm_MapData[posX, posY].ChangeTiled();
            }
        }

        public void SetAddPoint(eMapTileKind mapTileKind, int point)
        {
            if (_thisTilePoint.TryAdd(mapTileKind, point) == false) _thisTilePoint[mapTileKind] += point;
            ChangeTiled();
        }

        private bool TryGetPosition(int positionX, int positionY)
        {
            if (positionX < 0 || positionY < 0) return false;
            if (positionX >= MapSizeX || positionY >= MapSizeY) return false;
            return true;
        }

        public void ChangeTiled()
        {
            _thisMapTile = _thisTilePoint.Max(x => x.Key);
            CheckTiled();
            tempMesh.material = GameManager._instance._mapManager.materials[(int)_thisMapTile];
        }
        public void ChangeTiled(eMapTileKind setTile)
        {
            _thisMapTile = setTile;
            CheckTiled();
            tempMesh.material = GameManager._instance._mapManager.materials[(int)_thisMapTile];
        }

        private void CheckTiled()
        {
            CheckWater();

        }
        private void CheckWater()
        {
            if (_isBoundaryConnect == true) return;
            _isBoundaryConnect = MapConnectConditionCheck(eMapTileKind.Water);
        }

        private bool MapConnectConditionCheck(eMapTileKind sel_map)
        {
            if (_thisMapTile == eMapTileKind.None) return false;
            if (MapStatCondition((int)_positionX - 1, (int)_positionY, sel_map)) return true;
            if (MapStatCondition((int)_positionX + 1, (int)_positionY, sel_map)) return true;
            if (MapStatCondition((int)_positionX, (int)_positionY - 1, sel_map)) return true;
            if (MapStatCondition((int)_positionX, (int)_positionY + 1, sel_map)) return true;
            return false;
        }

        public bool MapStatCondition(int posX, int posY, eMapTileKind sel_map)
        {
            if (TryGetPosition(posX, posY) && mm_MapData[posX, posY]._isBoundaryConnect && mm_MapData[posX, posY]._thisMapTile == _thisMapTile)
            {
                return true;
            }
            return false;
        }

    }
}