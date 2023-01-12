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

    [Range(0.1f, 2f)]
    public float _loopCheckDelay = 0.1f;
    [Range(0.5f, 5f)]
    public float _loopSleepDelay = 3.0f;

    public eAIType _type = eAIType.Normal;
    protected List<ActionInfo> _actionList = new List<ActionInfo>();
    [SerializeField]
    public List<ePattern> _scheduler = new List<ePattern>();
    protected IEnumerator _currentPattern = null;
    protected bool isSkip = false;

    protected List<Panel_BasicUnitController> _Info_nearbyUnits = new List<Panel_BasicUnitController>();
    protected List<Panel_BasicUnitController> _Info_nearbyTeam = new List<Panel_BasicUnitController>();
    protected List<Panel_BasicUnitController> _Info_nearbyEnemy = new List<Panel_BasicUnitController>();

    private void OnEnable()
    {
        if (GameManager._instance != null)
        {
            GameManager._instance._unitManager.OnStateChangeUnit += OnChangingEvent;
        }
        else StartCoroutine(StandbyGameManager());
    }

    /// <summary>
    /// (예외대응) 
    /// <br> 에디터에 배치된 유닛들이 게임매니저의 인스턴스 생성과정보다 빠를 경우 이 함수가 실행됩니다. </br>
    /// </summary>
    /// <returns></returns>
    IEnumerator StandbyGameManager()
    {
        while (GameManager._instance == null) 
        {
            yield return awakeCheekTime;
        }
        GameManager._instance._logManager.InputErrorLog(EnumError.ErrorKind.DelegateSettingError);
        GameManager._instance._unitManager.OnStateChangeUnit += OnChangingEvent;
        AutoScheduler(ePattern.Continue);
    }

    private void OnDisable()
    {
        GameManager._instance._unitManager.OnStateChangeUnit -= OnChangingEvent;
    }


    public class ActionInfo
    {
        public bool ExitEvent;
        public int _targetID;
        public eAction _kind;
        public GameObject _targetObject;

        public void SetExitEvent()
        {
            ExitEvent = true;
        }

        public ActionInfo(GameObject target)
        {
            this._targetObject = target;
            this.ExitEvent = false;
            this._targetID = target.GetInstanceID();
            this._kind = eAction.Idle;
        }

        public float TargetDistance(Vector3 position)
        {
            return Vector3.Distance(this._targetObject.transform.position, position);
        }

        public Vector3 TargetDirectionVec3(Vector3 position)
        {
            return this._targetObject.transform.position - position;
        }

        public Quaternion TargetDirectionQuat()
        {
            return Quaternion.LookRotation(this._targetObject.transform.position);
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

        //임시

        if (_Info_nearbyEnemy.Count != 0) return ePattern.Attacking;
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
            if (_scheduler.Count == 0) return AutoScheduler(PatternSearch());
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

        foreach (var action in _actionList)
        {
            if (action.ExitEvent == true) _actionList.Remove(action);
        }

        if (_currentPattern != null)
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
        _actionList.Insert(0, new ActionInfo(_targetObject));

        while (_actionList[0].TargetDistance(gameObject.transform.position) > unit.unitState._attackRange)
        {
            Action_Move(_actionList[0]);
            if (isSkip == true) break;
            yield return delayTime;
        }

        // 리소스 관리 효율성을 위해서 해당 함수를 분리시키는 방법을 검토
        // 1안. Attacking 기능의 종료부분을 분리시켜서, 액션에서 반복하고, 종료됬을때 종료부분을 실행시키는 형태.
        // 해당 Action_Attack 내부에서 isSkip 기능 체크 
        Action_Attack(_actionList[0]);
        while (_actionList[0].ExitEvent == false)
        {
            if (isSkip == true) break;
            yield return delayTime;
        }
        _actionList[0].SetExitEvent();
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
        _actionList.Insert(0, new ActionInfo(this.gameObject));

        ePattern checkPattern = ePattern.Stand;
        while (checkPattern == ePattern.Stand)
        {
            yield return awakeCheekTime;
            if (isSkip == true) break;
            checkPattern = PatternSearch();
        }

        //if (checkPattern != ePattern.Stand) AutoScheduler(checkPattern);
        _actionList[0].SetExitEvent();

        ActionEnd();
    }


    #region 패턴 내부로직
    /// <summary>
    /// (기능)
    /// <br> AI가 판단하는데 필요한 모든 자료를 셋팅합니다. </br>
    /// </summary>
    protected void DataCollection()
    {
        _targetObject = null;

        if (_Info_nearbyUnits.Count == 0) // 주변에 유닛이 없는 경우
        {
            return;
        }
        var temp_Team = _Info_nearbyTeam.OrderByDescending(Team => Team.unitState._currentPriority).Select(Team => Team).FirstOrDefault();
        var temp_Enemy = _Info_nearbyEnemy.OrderByDescending(Enemy => Enemy.unitState._currentPriority).Select(Enemy => Enemy).FirstOrDefault();

        _targetObject = (temp_Enemy?.unitState._currentPriority < temp_Team?.unitState._currentPriority) ? temp_Team?.gameObject : temp_Enemy?.gameObject;
        if (_targetObject == null) _targetObject = this.gameObject;

        //_Info_nearbyEnemy[0].unitState._currentPriority = 0;

        //List<Panel_BasicUnitController> temp1 = _Info_nearbyEnemy.OrderBy(x => x.unitState._currnetPriority).ToList();


        // 해당 유닛들의 정보를 토대로 린큐로 정리시킴. 우선도 / 체력, 위력 / 심플버전 / 디테일버전 (성격으로 구분) / 공포,사기와같은 수치적용 / 지능수치적용
        // 상대방의 위치 정보를 부여 (층이 다른 곳에 유닛이 잇으면 해당 탐지에서 분리  (주변적을 우선 타격, 주변적이 없을경우 다른위치에잇는 유닛을 향해 이동)
        // 정신 능력치 (공포수치가 정신능력치보다 높을경우 성격에 따라 다른 양상 부여 (패닉 / 도주 등)
        // 공격대상이 아닌, 공격받는중이라면 우선도 퍼센트로 감소 
        // 공격방식 판단 // 이동 (동선 계산 방법 포함)// 스킬유무 판단 
        // 수비적인성향들은 진영을 유지하려고함. 아군으로부터 크게벗어나지않는방향 (적이 도망간다고 쫓아가지않음) 
        // 적극적인 경우 아군이공격 받을경우 영향을 받음 해당유닛을 공격하고자함
        // 우선도가 공격을 가하면 데미지량에 비례하여 가중치가 부여됨(자신을 공격하는 대상을 공격하는걸 구현할 방법 구상)
    }

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
        if (GameManager._instance._teamSetting.GetTeamStatus(unit.unitState._TeamNumber, tempVar_Unit.unitState._TeamNumber) == true)
        {
            _Info_nearbyTeam.Add(tempVar_Unit);
        }
        if (GameManager._instance._teamSetting.GetEnemyStatus(unit.unitState._TeamNumber, tempVar_Unit.unitState._TeamNumber) == true)
        {
            _Info_nearbyEnemy.Add(tempVar_Unit);
        }
        tempVar_Unit = null;
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
    protected void Action_Move(ActionInfo action) // 작성중
    {
        if (action.ExitEvent == true)
        {
            AutoScheduler(ePattern.Continue);
            return;
        }
        unitCtrl.Move(action.TargetDirectionVec3(gameObject.transform.position));
    }

    protected void Action_Attack(ActionInfo action) // 작성중
    {
        if (action.ExitEvent == true)
        {
            
        }
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
