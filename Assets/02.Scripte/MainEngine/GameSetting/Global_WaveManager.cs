using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Global_WaveManager : MonoBehaviour
{
    public List<WaveInfo> WaveLevel = new List<WaveInfo>();
    public int _currentWaveLevel = 0;
    public List<int> _currentUnitGroup = new List<int>();
    public int _currentUnitCount = 0;

    [Range(10, 600)]
    public float _fisrtStartWaveDelayTime = 10;


    [System.Serializable]
    public struct WaveInfo
    {
        public string _name;
        public WaitForSeconds _delayStart;
        public Vector3 _field;
        public List<WaveUnitInfo> _units;

        public bool _LastStage;
        public bool _isReward;
    }

    [System.Serializable]
    public struct WaveUnitInfo
    {
        public Global_UnitManager.eUnitKind _unitKind;
        public int _count;
    }

    public void Init()
    {
        if (WaveLevel.Count == 0) return;
        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(_fisrtStartWaveDelayTime);
        CheckWaveUnitDieCount();

    }

    public IEnumerator CreateWave(WaveInfo waveInfo)
    {
        if(waveInfo._field == Vector3.zero)
        {
            waveInfo._field = GameManager._instance._mapManager.GetRegenPoint();
            if (waveInfo._field == Vector3.zero)
            {
                NextWave();
                yield break;
            }
        }
        _currentUnitCount = 0;
        _currentUnitGroup.Clear();
        GameManager._instance._mapManager.WaveFieldReset();
        yield return waveInfo._delayStart;
        for (int i = 0; i < waveInfo._units.Count; i++)
        {
            _currentUnitCount++;
            _currentUnitGroup.Add(GameManager._instance._unitManager.CreateUnit(waveInfo._units[i]._unitKind, waveInfo._field, 3, true));
        }
    }

    public void CheckWaveUnitDieCount()
    {
        _currentUnitCount--;
        if (_currentUnitCount <= 0)
        {
            if (WaveLevel[_currentWaveLevel]._LastStage)
            {
                LastStage();
                return;
            }
            _currentWaveLevel++;
            StartCoroutine(CreateWave(WaveLevel[_currentWaveLevel]));
        }
    }

    public void NextWave()
    {
        foreach (var unit in _currentUnitGroup)
        {
            GameManager._instance._unitManager.PoolRemoveUnit(unit);
        }
        _currentUnitCount = 0;
        _currentUnitGroup.Clear();

        CheckWaveUnitDieCount();
    }

    private void LastStage()
    {
        //최종 처리
    }
}
