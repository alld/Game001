using MapSample;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Global_MapManager : MonoBehaviour
{
    public Material[] materials; // �ӽ� �׽�Ʈ��

    /// <summary>
    /// �� ����� �������ִ� ���� Ư¡
    /// </summary>
    public enum eMapTileKind
    {
        None = 0,
        /// <summary> �÷��̾� </summary>
        Player,
        /// <summary> ���� </summary>
        Village,
        /// <summary> �� </summary>
        Forest,
        /// <summary> �縷 </summary>
        Desert,
        /// <summary> ��� </summary>
        Plain,
        /// <summary> �� </summary>
        Water,
        /// <summary> ��� </summary>
        Lava,
        /// <summary> �� </summary>
        Swampy,
        /// <summary> �ϼ� </summary>
        Rock
    }
    /// <summary>
    /// ���� ��Ҹ� Ư¡���� ��Ÿ�����ִ� Ư¡
    /// </summary>
    public enum eMapAreaKind
    {
        None,
        /// <summary> ȣ�� </summary>
        Lake,
        /// <summary> �� </summary>
        River,
        /// <summary> �ٴ� </summary>
        Sea,
        /// <summary> �ʿ� </summary>
        Pasture,
        /// <summary> �и� </summary>
        Jungle,
        /// <summary> ������ </summary>
        Marsh,
        /// <summary> �ϼ����� </summary>
        rocky,
        /// <summary> ȭ�� </summary>
        Volcano,
        /// <summary> ���� </summary>
        Cave
    }
    public GameObject prefab_MapData;
    public List<MapData> MapDataList = new List<MapData>();
    public bool _editorSetting;
    public int _initMapPointCount = 10;

    public MapData[,] MapDataXY = new MapData[MapSizeX, MapSizeY];

    public const int MapSizeX = 10;
    public const int MapSizeY = 10;

    public eMapAreaKind[] StandardMap_1th = new eMapAreaKind[3];
    private Dictionary<eMapAreaKind, int> StandardMap_2th = new Dictionary<eMapAreaKind, int>();
    private List<eMapTileKind> StandardMap_3th = new List<eMapTileKind>();

    public bool _completSetting = false;
    public bool _completReady = false;

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

        SetPlayerPosition();

        _completSetting = true;
    }

    protected void SetPlayerPosition()
    {
        MapDataXY[(int)Mathf.Round(MapSizeX * 0.5f), (int)Mathf.Round(MapSizeY * 0.5f)].ChangeTiled(eMapTileKind.Player);
    }


    /*
    * �߾� 5,5 ������ ����Ʈ �÷��̾� ����, Ư���� ��� ���� ��Ż
    * �÷��̾������� ��� ��ġ�� ���� ����, ������ ���� ��ġ��
    * 
    * 10�� ���� ��ġ�� �й��ؼ� �ڿ�, Ư�� ���� ��ġ��
    * 
    * 10�� �ֿ� ���� ��ġ�� ���¿����� �ֺ��� ���� �����Ŵ
    * 
    * ������ �ֿ� ������ �����ϰ� ���ʿ� ������� �ش� ������ �ֿ��������� ����
    * ������ �ֿ������� 3���̻� ����������� �Ŵ������� ����
    * �Ŵ����������� Ư���� ����� ������. 
    * 
    * �������� ���Ǿ������� ȣ��
    * �ܺα��� ����Ǿ������� �ٴ�
    * ���� ���� ����Ǿ������� ������ �з��� 
    * 
    * ������ ������ ������, ȭ������, �縷����, �ʿ�, ��, ��, ���
    * 
    * ����, ������, NPC����, 
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
        public GameObject _mapFloor;
        public bool _isAssignAddPoint = false;
        public Dictionary<eMapTileKind, int> _thisTilePoint = new Dictionary<eMapTileKind, int>();
        private MapData[,] mm_MapData; // �� �Ŵ����� ĳ��
        public bool _isDungeon;
        public bool _isBoundaryConnect;

        public MeshRenderer tempMesh; // �ӽ� �׽�Ʈ��

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