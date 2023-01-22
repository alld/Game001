using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Panel_BasicUnitAI : MonoBehaviour
{
    private const string tag_Trigger = "Trigger";

    #region 캐시처리
    [HideInInspector] public Panel_BasicUnitController unit;
    private CharacterController unitCtrl = null;
    private WaitForSeconds delayTime = null;
    private WaitForSeconds awakeCheekTime = null;
    private WaitForSeconds rotdelayTime = null;
    public WaitForSeconds delayAttackTiming = null;
    public WaitForSeconds delayAttackEnd = null;


    [HideInInspector] public SphereCollider _cognitiveRange = null;
    #endregion

    [Range(0.1f, 2f)]
    public float _loopCheckDelay = 0.1f;
    [Range(0.5f, 5f)]
    public float _loopSleepDelay = 3.0f;
    [Range(1f, 30f)]
    public float _loopRotDelay = 5.0f;

    protected List<ActionInfo> _actionList = new List<ActionInfo>();
    [SerializeField]
    public List<ePattern> _scheduler = new List<ePattern>();
    protected Coroutine _currentPattern = null;
    protected Coroutine _currentAction = null;
    protected bool isSkip = false;

    protected List<Panel_BasicUnitController> _Info_nearbyUnits = new List<Panel_BasicUnitController>();
    protected List<Panel_BasicUnitController> _Info_nearbyTeam = new List<Panel_BasicUnitController>();
    protected List<Panel_BasicUnitController> _Info_nearbyEnemy = new List<Panel_BasicUnitController>();




    public class ActionInfo
    {
        public bool ExitEvent;
        public int _targetID;
        public eAction _kind;
        public ePattern _pattern;
        public GameObject _targetObject;
        public Panel_BasicUnitController _unit;

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
            this._unit = target.GetComponent<Panel_BasicUnitController>();
        }

        public float TargetDistance(Vector3 position)
        {
            return Vector3.Distance(this._targetObject.transform.position, position);
        }

        public Vector3 TargetDirectionVec3(Vector3 position)
        {
            return this._targetObject.transform.position - position;
        }

        public Vector3 TargetMovePointVec3(Vector3 position, float moveSpeed)
        {
            return Vector3.Normalize(TargetDirectionVec3(position)) * moveSpeed * Time.deltaTime;
        }

        public Quaternion TargetDirectionQuat()
        {
            return Quaternion.LookRotation(this._targetObject.transform.position);
        }

        public void SetTargetObject(GameObject target)
        {
            this._targetObject = target;
            this._targetID = target.GetInstanceID();
            this._unit = target.GetComponent<Panel_BasicUnitController>();
        }

        public void SetPatternType(ePattern pattern)
        {
            if (this._pattern == pattern) return;
            this._pattern = pattern;

            switch (pattern)
            {
                case ePattern.Continue:
                    break;
                case ePattern.Attacking:
                    break;
                case ePattern.Avoidng:
                    break;
                case ePattern.RunningAway:
                    break;
                case ePattern.Flollow:
                    break;
                case ePattern.Push:
                    break;
                case ePattern.Stern:
                    break;
                case ePattern.Skill:
                    break;
                case ePattern.Death:
                    break;
                case ePattern.Resurrection:
                    break;
                case ePattern.Stand:
                    this._kind = eAction.Idle;
                    break;
                default:
                    break;
            }


        }
    }
    #region 타입 설정


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
        Stand,
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
        rotdelayTime = new WaitForSeconds(_loopRotDelay);

        unit = GetComponent<Panel_BasicUnitController>();
        _cognitiveRange = GetComponentInChildren<SphereCollider>();
        unitCtrl = GetComponent<CharacterController>();
    }


    #region 스케쥴러 관리
    public ePattern PatternSearch() // 작성중
    {
        ActionInfo temp_actionInfo = DataCollection();
        if (_actionList.Count != 0)
        {
            if (temp_actionInfo._kind == _actionList[0]?._kind && temp_actionInfo._targetID == _actionList[0]?._targetID) return temp_actionInfo._pattern;
        }

        _actionList.Add(temp_actionInfo);


        return _actionList.Last()._pattern;
    }

    public bool AutoScheduler(ePattern AIPattern, bool rightoff = false)
    {
        if (ePattern.Death == AIPattern) // 사망처리는 어떤것보다 우선처리되어야해서 별도 제어됩니다. 
        {
            ImmediateInterruption();
            _currentPattern = StartCoroutine(PT_Death());
            return false;
        }
        else if (ePattern.Continue == AIPattern)
        {
            if (_scheduler.Count == 0) return AutoScheduler(PatternSearch());
            switch (_scheduler[0])
            {
                case ePattern.Attacking:
                    _currentPattern = StartCoroutine(PT_Attacking());
                    break;
                case ePattern.Avoidng:
                    _currentPattern = StartCoroutine(PT_Avoidng());
                    break;
                case ePattern.RunningAway:
                    _currentPattern = StartCoroutine(PT_RunningAway());
                    break;
                case ePattern.Flollow:
                    _currentPattern = StartCoroutine(PT_Flollow());
                    break;
                case ePattern.Push:
                    _currentPattern = StartCoroutine(PT_Push());
                    break;
                case ePattern.Stern:
                    _currentPattern = StartCoroutine(PT_Stern());
                    break;
                case ePattern.Skill:
                    _currentPattern = StartCoroutine(PT_Skill());
                    break;
                case ePattern.Resurrection:
                    _currentPattern = StartCoroutine(PT_Resurrection());
                    break;
                case ePattern.Stand:
                    _currentPattern = StartCoroutine(PT_Stand());
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
    /// (기능) 스케쥴러를 종료시키는 함수입니다. 부수처리는 외부함수에서 처리됩니다.
    /// </summary>
    public void ShutdownScheduler()
    {
        _actionList.Clear();

        isSkip = true;
    }

    /// <summary>
    /// (기능) 스케쥴러를 종료시키는 함수입니다. 매개변수가 담길경우 모든 처리를 해당함수에서 담당합니다. 
    /// </summary>
    /// <param name="check"></param>
    public void ShutdownScheduler(bool check)
    {
        isSkip = true;

        _actionList.Clear();

        CoroutineArrange();
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

        RemoveActionList();

        CoroutineArrange();

        if (_scheduler.Count != 0)
        {
            _scheduler.RemoveAt(0);
            AutoScheduler(ePattern.Continue);
        }
    }

    /// <summary>
    /// (기능) 액션 리스트에 제거되야 할 값을 찾아 지웁니다.
    /// </summary>
    private void RemoveActionList()
    {
        for (int i = _actionList.Count - 1; i >= 0; i--)
        {
            if (_actionList[i].ExitEvent == true)
            {
                _Info_nearbyUnits.Remove(_actionList[i]._unit);
                _Info_nearbyTeam.Remove(_actionList[i]._unit);
                _Info_nearbyEnemy.Remove(_actionList[i]._unit);
                _actionList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// (기능) 현재 진행중인 잔여 코루틴들을 정리합니다. 
    /// </summary>
    private void CoroutineArrange()
    {
        if (_currentPattern != null)
        {
            StopCoroutine(_currentPattern);
            _currentPattern = null;
        }

        if (_currentAction != null)
        {
            StopCoroutine(_currentAction);
            _currentAction = null;
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
        do
        {
            if (isSkip == true || _actionList[0].ExitEvent == true)
            {
                StopCoroutine(_currentAction);
                _currentAction = null;
                break;
            }
            if (_currentAction == null) _currentAction = StartCoroutine(Action_Move());
            yield return delayTime;
        }
        while (_currentAction != null);

        // 리소스 관리 효율성을 위해서 해당 함수를 분리시키는 방법을 검토
        // 1안. Attacking 기능의 종료부분을 분리시켜서, 액션에서 반복하고, 종료됬을때 종료부분을 실행시키는 형태.
        // 해당 Action_Attack 내부에서 isSkip 기능 체크 
        do
        {
            if (isSkip == true || _actionList[0].ExitEvent == true)
            {
                StopCoroutine(_currentAction);
                _currentAction = null;
                break;
            }
            if (_currentAction == null) _currentAction = StartCoroutine(Action_Attack());
            yield return delayAttackTiming;
            yield return delayAttackEnd;
        }
        while (_currentAction != null);
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
        unit.unitState.Death();

        if (unit.unitState._isRotting == true) yield return rotdelayTime;
        else yield return new WaitForSeconds(1f); // 애니메이션 타임
        ShutdownScheduler();

        // <- (추가) 유닛매니저 풀링에서 해당 유닛 해제 및 연결되어잇는 내용물 제거
        // 풀링에서 제거될시 해당 유닛을 디시블이나 오브젝터 처리

        CoroutineArrange();
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

        //if (checkPattern != ePattern.Stand) AutoScheduler(checkPattern);
        _actionList[0].SetExitEvent();

        ActionEnd();
    }


    #region 패턴 내부로직
    /// <summary>
    /// (기능)
    /// <br> AI가 판단하는데 필요한 모든 자료를 셋팅합니다. </br>
    /// </summary>
    protected ActionInfo DataCollection()
    {

        ActionInfo temp_ActionInfo = new ActionInfo(gameObject);
        if(unit.unitState._isLive == false)
        {
            temp_ActionInfo.SetPatternType(ePattern.Death);
            return temp_ActionInfo;
        } else if (_actionList.Count != 0)
        {
            if (_actionList[0]._pattern == ePattern.Death && unit.unitState._isLive == true)
            {
                // 부활 로직 실행 // 여기서 할지 외부에서 이벤트식으로 부활시 스케쥴러로 처리할지 고민..
            }
        }
        if (_Info_nearbyUnits.Count == 0) // 주변에 유닛이 없는 경우
        {
            temp_ActionInfo.SetPatternType(ePattern.Stand);
            return temp_ActionInfo;
        }

        switch (unit.unitState._AIType)
        {
            case Data_BasicUnitData.eAIType.Normal:
                var temp_Team = _Info_nearbyTeam.OrderByDescending(Team => Team.unitState._currentPriority).Select(Team => Team).FirstOrDefault();
                var temp_Enemy = _Info_nearbyEnemy.OrderByDescending(Enemy => Enemy.unitState._currentPriority).Select(Enemy => Enemy).FirstOrDefault();

                if (temp_Enemy == null)
                {
                    if (unit.unitState._beneficial == false || temp_Team == null)
                    {
                        temp_ActionInfo.SetPatternType(ePattern.Stand);
                        break;
                    }
                    // 추가로 이로운효과 판단 요소가 필요함

                    //임시
                    temp_ActionInfo.SetTargetObject(temp_Team.gameObject);
                    // 보유스킬 목록을 찾아서 스킬우선도가 높은걸 먼저 실행함 해당 스킬에서 스킬우선도 변경 조건이잇으면 거기서 반영
                    temp_ActionInfo.SetPatternType(ePattern.Skill);
                    break;
                }
                else if (temp_Team == null)
                {
                    temp_ActionInfo.SetTargetObject(temp_Enemy.gameObject);
                    temp_ActionInfo.SetPatternType(ePattern.Attacking);
                    break;
                }



                if (temp_Enemy.unitState._currentPriority < temp_Team.unitState._currentPriority)
                {
                    temp_ActionInfo.SetTargetObject(temp_Team.gameObject);
                }
                else
                {
                    temp_ActionInfo.SetTargetObject(temp_Enemy.gameObject);
                    temp_ActionInfo.SetPatternType(ePattern.Attacking);
                }
                break;
            case Data_BasicUnitData.eAIType.UnArmed:
                break;
            case Data_BasicUnitData.eAIType.Passive:
                break;
            case Data_BasicUnitData.eAIType.Active:
                break;
            case Data_BasicUnitData.eAIType.Aggressive:
                break;
            case Data_BasicUnitData.eAIType.Defensive:
                break;
        }

        return temp_ActionInfo;
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
        if (tempVar_Unit.unitState._isLive == false) return;

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
        if (tempVar_Unit.unitState._isLive == false) return;

        _Info_nearbyUnits.Remove(tempVar_Unit);
        _Info_nearbyTeam.Remove(tempVar_Unit);
        _Info_nearbyEnemy.Remove(tempVar_Unit);
        tempVar_Unit = null;
    }
    #endregion

    #endregion

    #endregion

    #region 유닛 동작

    protected IEnumerator Action_Move() // 작성중
    {
        WaitForSeconds temp_deltaTime = new WaitForSeconds(Time.deltaTime);
        while (_actionList[0].TargetDistance(gameObject.transform.position) > unit.unitState._attackRange)
        {
            unitCtrl.Move(_actionList[0].TargetMovePointVec3(gameObject.transform.position, unit.unitState._moveSpeed));
            HPBarMove();
            if (isSkip == true || _actionList[0].ExitEvent == true) break;
            yield return temp_deltaTime;
        }
        _currentAction = null;
    }


    protected IEnumerator Action_Attack() // 작성중
    {
        while (_actionList[0].TargetDistance(gameObject.transform.position) <= unit.unitState._attackRange)
        {
            yield return delayAttackTiming;
            if (isSkip == true || _actionList[0].ExitEvent == true) break;
            if (_actionList[0]._unit.EventDamage(unit) == false)
            {
                _actionList[0].SetExitEvent(); // 어디까지 대응을 해줘야할지 검토가 필요함 1. 사망처리(자동 호출로 통일 가능함), AI타겟변동, 행동종료
                break;
            }
            yield return delayAttackEnd;
        }
        _currentAction = null;
    }
    #endregion

    private static Vector3 HpbarHeight = new Vector3(0, 1.5f, 0);
    public void HPBarMove()
    {
        unit._HPbar._bar.transform.position = Camera.main.WorldToScreenPoint(transform.position + HpbarHeight);
    }


    /// <summary>
    /// (기능) 
    /// <br> 외부요인에 의해서 AI패턴이 변경되어야할 상황을 재검토합니다. </br>
    /// <br> 패턴 대상이 사망하거나, 이동중 새로운 적이 등장, 또는 자신을 공격하는등 우선도가 변경되는 상횡시 호출됩니다.</br>
    /// </summary>
    /// <param name="objectID"></param>
    public void OnChangingEvent(int objectID) // 작성중
    {
        foreach (var action in _actionList)
        {
            if (action._targetID == objectID)
            {
                action.SetExitEvent();
            }
        }
    }
}
