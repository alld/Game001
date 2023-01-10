using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[System.Serializable]
public class Graphic_BasicUnitAI : MonoBehaviour
{
    private const string tag_Trigger = "Trigger";

    #region 캐시처리
    [HideInInspector] public Panel_BasicUnitController unit;
    private CharacterController unitCtrl = null;
    [HideInInspector] public GameObject _targetObject = null;
    private WaitForSeconds delayTime = null;
    private WaitForSeconds awakeCheekTime = null;
    [HideInInspector] public SphereCollider _cognitiveRange = null;
    #endregion

    [Range(0.01f, 2f)]
    public float _loopCheckDelay = 0.01f;
    [Range(0.5f, 5f)]
    public float _loopSleepDelay = 3.0f;

    public eAIType _type = eAIType.Normal;
    protected List<ActionInfo> _actionList = new List<ActionInfo>();
    protected List<ePattern> _scheduler = new List<ePattern>();
    protected IEnumerator _currentPattern = null;
    protected bool isSkip = false;

    public int test = 0;
    protected List<Panel_BasicUnitController> _Info_nearbyUnits = new List<Panel_BasicUnitController>();
    protected List<Panel_BasicUnitController> _Info_nearbyTeam = new List<Panel_BasicUnitController>();
    protected List<Panel_BasicUnitController> _Info_nearbyEnemy = new List<Panel_BasicUnitController>();


    public struct ActionInfo
    {
        public bool ExitEvent;
        public int _targetID;
        public eAction _kind;

        public Vector3 _targetPositison;
        public Vector3 _position;

        public void SetExitEvent()
        {
            ExitEvent = true;
        }

        public ActionInfo(GameObject target, Vector3 position)
        {
            this.ExitEvent = false;
            this._targetID = target.GetInstanceID();
            this._kind = eAction.Idle;

            this._position = position;
            this._targetPositison = target.transform.position;
        }

        public float TargetDistance()
        {
            return Vector3.Distance(this._targetPositison, this._position);
        }

        public Vector3 TargetDirectionVec3()
        {
            return this._targetPositison - this._position;
        }

        public Quaternion TargetDirectionQuat()
        {
            return Quaternion.LookRotation(this._targetPositison);
        }
    }
    #region 타입 설정
    public enum eAIType
    {
        Normal = 0, // 일반
        UnArmed,    // 비무장
        Passive,    // 수동적
        Active,     // 적극적
        Aggressive, // 공격적
        Defensive   // 수비적
    }

    public enum ePattern
    {
        Continue = 0,
        Attacking,
        Avoidng,
        RunningAway,
        Flollow,
        Push,
        Stern,
        Skill,
        Death,
        Resurrection,
        Stand
    }

    public enum eAction
    {
        Idle = 0,
        Move,
        Attack,
        AttackMove,
        Skill,
        Search,
        Die,
    }
    #endregion


    public void InitAI()
    {
        delayTime = new WaitForSeconds(_loopCheckDelay);
        awakeCheekTime = new WaitForSeconds(_loopSleepDelay);

        unit = GetComponent<Panel_BasicUnitController>();
        _cognitiveRange = GetComponentInChildren<SphereCollider>();

    }


    #region 스케쥴러 관리
    public ePattern PatternSearch() // 작성중
    {
        DataCollection();
        return ePattern.Stand;
    }

    public bool AutoScheduler(ePattern AIPattern, bool rightoff = false)
    {
        if (ePattern.Death == AIPattern) // 사망처리는 어떤것보다 우선처리되어야해서 별도 제어됩니다. 
        {
            ImmediateInterruption();
            _currentPattern = PT_Death();
            StartCoroutine(_currentPattern);
            return false;
        }
        else if (ePattern.Continue == AIPattern)
        {
            switch (_scheduler[0])
            {
                case ePattern.Attacking:
                    _currentPattern = PT_Attacking();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.Avoidng:
                    _currentPattern = PT_Avoidng();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.RunningAway:
                    _currentPattern = PT_RunningAway();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.Flollow:
                    _currentPattern = PT_Flollow();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.Push:
                    _currentPattern = PT_Push();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.Stern:
                    _currentPattern = PT_Stern();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.Skill:
                    _currentPattern = PT_Skill();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.Resurrection:
                    _currentPattern = PT_Resurrection();
                    StartCoroutine(_currentPattern);
                    break;
                case ePattern.Stand:
                    _currentPattern = PT_Stand();
                    StartCoroutine(_currentPattern);
                    break;
                default:
                    return AutoScheduler(PatternSearch());
            }
            return true;
        }

        if (_scheduler.Count == 0) // 스케쥴러가 비어있으면 즉시성과 상관없이 바로 실행합니다. 
        {
            _scheduler.Add(AIPattern);
            return AutoScheduler(ePattern.Continue);
        }
        else if (rightoff == true)
        // 즉시 실행인 경우 현재 진행중인 액션을 멈추는 로직을 실행시키고, 끝나면 바로 추가된 액션을 진행합니다. 
        {
            _scheduler.Insert(1, AIPattern);
            InterruptionRequest();
            // 해당 로직을 멈추는 함수가 들어가야함 (온오프 변수 : 체크용 전환, 실행중인 애니메이션 중지 끝났는지 체크, 코루틴 중지) // 끝나는 부분에 스케쥴러 컨티뉴진행
            return true;
        }
        else // 일반적인 경우 
        {
            _scheduler.Add(AIPattern);
            return true;
        }
    }

    /// <summary>
    /// (기능)
    /// <br> 현재 진행중인 액션이 끝났을때 호출되는 함수입니다. </br>
    /// <br> 액션과정에서 사용되었던 내용들을 정리하는 기능을 포함하고있습니다.</br>
    /// <br> [주의] 스케쥴러 과정 가장 마지막에 호출되어야합니다.  </br>
    /// <br> 스케쥴러를 이어서 시작하는 기능이 포함되어있기 때문입니다.</br>
    /// </summary>
    public void ActionEnd()
    {
        isSkip = false;
        _targetObject = null;

        foreach (var action in _actionList)
        {
            if (action.ExitEvent == true) _actionList.Remove(action);
        }

        if (_currentPattern == null)
        {
            StopCoroutine(_currentPattern);
            _currentPattern = null;
        }


        if (_scheduler.Count != 0)
        {
            _scheduler.RemoveAt(0);
            AutoScheduler(ePattern.Continue);
        }
    }

    /// <summary>
    /// (기능)
    /// <br> 스케쥴러를 포함한 모든 행동들을 즉각 중단시킵니다.</br>
    /// </summary>
    public void ImmediateInterruption()
    {
        _scheduler.Clear();
        ActionEnd();
    }

    /// <summary>
    /// (기능)
    /// <br> 현재 진행중인 연출 및 AI 동작들을 중단하기위한 절차에 돌입합니다.</br>
    /// </summary>
    public void InterruptionRequest()
    {
        //모든 절차 종료후 스케쥴러 진행
        ActionEnd();
    }

    #region 패턴
    protected IEnumerator PT_Attacking()
    {

        yield return null;

        ActionEnd();
    }
    protected IEnumerator PT_Avoidng()
    {
        yield return null;
    }
    protected IEnumerator PT_RunningAway()
    {
        yield return null;
    }
    protected IEnumerator PT_Flollow()
    {
        yield return null;
    }
    protected IEnumerator PT_Push()
    {
        yield return null;
    }
    protected IEnumerator PT_Stern()
    {
        yield return null;
    }
    protected IEnumerator PT_Skill()
    {
        yield return null;
    }
    protected IEnumerator PT_Death()
    {
        yield return null;
    }
    protected IEnumerator PT_Resurrection()
    {
        yield return null;
    }
    /// <summary>
    /// (기능) 
    /// <br> 해당 유닛을 대기상태로 놔둡니다. 일정시간마다 스킵 또는 </br>
    /// </summary>
    /// <returns></returns>
    protected IEnumerator PT_Stand()
    {
        ePattern checkPattern = ePattern.Stand;
        while (checkPattern == ePattern.Stand)
        {
            yield return awakeCheekTime;
            if (isSkip == true) break;
            checkPattern = PatternSearch();
        }

        if (checkPattern != ePattern.Stand) AutoScheduler(checkPattern);
        ActionEnd();
    }


    #region 패턴 내부로직
    /// <summary>
    /// (기능)
    /// <br> AI가 판단하는데 필요한 모든 자료를 셋팅합니다. </br>
    /// </summary>
    protected void DataCollection()
    {
        if(_Info_nearbyUnits.Count == 0)
        {
            _targetObject = null;
            _actionList.Insert(0, new ActionInfo());
            return;
        }
        var temp_Team = _Info_nearbyTeam.OrderByDescending(Team => Team.unitState._currnetPriority).Select(Team => Team).FirstOrDefault();
        var temp_Enemy = _Info_nearbyEnemy.OrderByDescending(Enemy => Enemy.unitState._currnetPriority).Select(Enemy => Enemy).FirstOrDefault();

        _actionList.Insert(0, new ActionInfo(_targetObject, gameObject.transform.position));

        _Info_nearbyEnemy[0].unitState._currnetPriority = 0;

        List<Panel_BasicUnitController> temp1 = _Info_nearbyEnemy.OrderBy(x => x.unitState._currnetPriority).ToList();
        Debug.Log(temp1.Count);


        // 해당 유닛들의 정보를 토대로 린큐로 정리시킴. 우선도 / 체력, 위력 / 심플버전 / 디테일버전 (성격으로 구분) / 공포,사기와같은 수치적용 / 지능수치적용
    }

    public IEnumerable<Panel_BasicUnitController> SearchTargetTeamUnit = null;
    public IEnumerable<Panel_BasicUnitController> SearchTargetEnemyUnit = null;

    private Panel_BasicUnitController tempVar_Unit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tag_Trigger) == true) return;
        if (other.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) return;
        if (other.TryGetComponent<Panel_BasicUnitController>(out tempVar_Unit) == false) 
        {
            GameManager._instance._logManager.InputErrorLog(EnumError.ErrorKind.AIMissingComponent);
            return;
        }
        _Info_nearbyUnits.Add(tempVar_Unit);
        if(GameManager._instance._teamSetting.GetTeamStatus(unit.unitState.TeamNumber ,tempVar_Unit.unitState.TeamNumber) == true)
        {
            _Info_nearbyTeam.Add(tempVar_Unit);
        }
        if (GameManager._instance._teamSetting.GetEnemyStatus(unit.unitState.TeamNumber, tempVar_Unit.unitState.TeamNumber) == true)
        {
            _Info_nearbyEnemy.Add(tempVar_Unit);
        }
        tempVar_Unit = null;


        //List<Panel_BasicUnitController> temp1 = _Info_nearbyTeam.OrderByDescending(x => x.unitState._currnetPriority).ToList();
        //Debug.Log(temp1[0].unitState._currnetPriority + ": " + temp1[0].gameObject.name);
        //temp1.Clear();

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tag_Trigger) == true) return;
        if (other.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) return;
        if (other.TryGetComponent<Panel_BasicUnitController>(out tempVar_Unit) == false)
        {
            GameManager._instance._logManager.InputErrorLog(EnumError.ErrorKind.AIMissingComponent);
            return;
        }

        _Info_nearbyUnits.Remove(tempVar_Unit);
        _Info_nearbyTeam.Remove(tempVar_Unit);
        _Info_nearbyEnemy.Remove(tempVar_Unit);
        tempVar_Unit = null;
    }
    #endregion

    #endregion

    #endregion

    #region 유닛 동작
    protected bool Action_Move(ActionInfo action) // 작성중
    {
        if (action.ExitEvent == true)
        {
            return AutoScheduler(ePattern.Continue);
        }
        unitCtrl.Move(action.TargetDirectionVec3());
        return false;
    }
    #endregion


    public void OnChangingEvent(int objectID) // 작성중
    {
        foreach (var action in _actionList)
        {
            if (action._targetID == objectID)
            {
                action.SetExitEvent();
                ActionEnd();
            }
        }
    }
}
