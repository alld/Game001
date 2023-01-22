using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Panel_BasicUnitAI : MonoBehaviour
{
    private const string tag_Trigger = "Trigger";

    #region ĳ��ó��
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
    #region Ÿ�� ����


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


    #region �����췯 ����
    public ePattern PatternSearch() // �ۼ���
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
        if (ePattern.Death == AIPattern) // ���ó���� ��ͺ��� �켱ó���Ǿ���ؼ� ���� ����˴ϴ�. 
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

        if (_scheduler.Count == 0) // �����췯�� ��������� ��ü��� ������� �ٷ� �����մϴ�. 
        {
            _scheduler.Add(AIPattern);
            return AutoScheduler(ePattern.Continue);
        }
        else if (rightoff == true)
        // ��� ������ ��� ���� �������� �׼��� ���ߴ� ������ �����Ű��, ������ �ٷ� �߰��� �׼��� �����մϴ�. 
        {
            _scheduler.Insert(1, AIPattern);
            InterruptionRequest();
            // �ش� ������ ���ߴ� �Լ��� ������ (�¿��� ���� : üũ�� ��ȯ, �������� �ִϸ��̼� ���� �������� üũ, �ڷ�ƾ ����) // ������ �κп� �����췯 ��Ƽ������
            return true;
        }
        else // �Ϲ����� ��� 
        {
            _scheduler.Add(AIPattern);
            return true;
        }
    }

    /// <summary>
    /// (���) �����췯�� �����Ű�� �Լ��Դϴ�. �μ�ó���� �ܺ��Լ����� ó���˴ϴ�.
    /// </summary>
    public void ShutdownScheduler()
    {
        _actionList.Clear();

        isSkip = true;
    }

    /// <summary>
    /// (���) �����췯�� �����Ű�� �Լ��Դϴ�. �Ű������� ����� ��� ó���� �ش��Լ����� ����մϴ�. 
    /// </summary>
    /// <param name="check"></param>
    public void ShutdownScheduler(bool check)
    {
        isSkip = true;

        _actionList.Clear();

        CoroutineArrange();
    }


    /// <summary>
    /// (���)
    /// <br> ���� �������� �׼��� �������� ȣ��Ǵ� �Լ��Դϴ�. </br>
    /// <br> �׼ǰ������� ���Ǿ��� ������� �����ϴ� ����� �����ϰ��ֽ��ϴ�.</br>
    /// <br> [����] �����췯 ���� ���� �������� ȣ��Ǿ���մϴ�.  </br>
    /// <br> �����췯�� �̾ �����ϴ� ����� ���ԵǾ��ֱ� �����Դϴ�.</br>
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
    /// (���) �׼� ����Ʈ�� ���ŵǾ� �� ���� ã�� ����ϴ�.
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
    /// (���) ���� �������� �ܿ� �ڷ�ƾ���� �����մϴ�. 
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
    /// (���)
    /// <br> �����췯�� ������ ��� �ൿ���� �ﰢ �ߴܽ�ŵ�ϴ�.</br>
    /// </summary>
    public void ImmediateInterruption()
    {
        _scheduler.Clear();
        ActionEnd();
    }

    /// <summary>
    /// (���)
    /// <br> ���� �������� ���� �� AI ���۵��� �ߴ��ϱ����� ������ �����մϴ�.</br>
    /// </summary>
    public void InterruptionRequest()
    {
        //��� ���� ������ �����췯 ����
        ActionEnd();
    }

    #region ����
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

        // ���ҽ� ���� ȿ������ ���ؼ� �ش� �Լ��� �и���Ű�� ����� ����
        // 1��. Attacking ����� ����κ��� �и����Ѽ�, �׼ǿ��� �ݺ��ϰ�, ��������� ����κ��� �����Ű�� ����.
        // �ش� Action_Attack ���ο��� isSkip ��� üũ 
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
        else yield return new WaitForSeconds(1f); // �ִϸ��̼� Ÿ��
        ShutdownScheduler();

        // <- (�߰�) ���ָŴ��� Ǯ������ �ش� ���� ���� �� ����Ǿ��մ� ���빰 ����
        // Ǯ������ ���ŵɽ� �ش� ������ ��ú��̳� �������� ó��

        CoroutineArrange();
    }
    protected IEnumerator PT_Resurrection()
    {
        yield return null;
    }
    /// <summary>
    /// (���) 
    /// <br> �ش� ������ �����·� ���Ӵϴ�. �����ð����� ��ŵ �Ǵ� </br>
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


    #region ���� ���η���
    /// <summary>
    /// (���)
    /// <br> AI�� �Ǵ��ϴµ� �ʿ��� ��� �ڷḦ �����մϴ�. </br>
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
                // ��Ȱ ���� ���� // ���⼭ ���� �ܺο��� �̺�Ʈ������ ��Ȱ�� �����췯�� ó������ ���..
            }
        }
        if (_Info_nearbyUnits.Count == 0) // �ֺ��� ������ ���� ���
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
                    // �߰��� �̷ο�ȿ�� �Ǵ� ��Ұ� �ʿ���

                    //�ӽ�
                    temp_ActionInfo.SetTargetObject(temp_Team.gameObject);
                    // ������ų ����� ã�Ƽ� ��ų�켱���� ������ ���� ������ �ش� ��ų���� ��ų�켱�� ���� ������������ �ű⼭ �ݿ�
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


        // �ش� ���ֵ��� ������ ���� ��ť�� ������Ŵ. �켱�� / ü��, ���� / ���ù��� / �����Ϲ��� (�������� ����) / ����,���Ͱ��� ��ġ���� / ���ɼ�ġ����
        // ������ ��ġ ������ �ο� (���� �ٸ� ���� ������ ������ �ش� Ž������ �и�  (�ֺ����� �켱 Ÿ��, �ֺ����� ������� �ٸ���ġ���մ� ������ ���� �̵�)
        // ���� �ɷ�ġ (������ġ�� ���Ŵɷ�ġ���� ������� ���ݿ� ���� �ٸ� ��� �ο� (�д� / ���� ��)
        // ���ݴ���� �ƴ�, ���ݹ޴����̶�� �켱�� �ۼ�Ʈ�� ���� 
        // ���ݹ�� �Ǵ� // �̵� (���� ��� ��� ����)// ��ų���� �Ǵ� 
        // �������μ������ ������ �����Ϸ�����. �Ʊ����κ��� ũ�Թ�����ʴ¹��� (���� �������ٰ� �Ѿư�������) 
        // �������� ��� �Ʊ��̰��� ������� ������ ���� �ش������� �����ϰ�����
        // �켱���� ������ ���ϸ� ���������� ����Ͽ� ����ġ�� �ο���(�ڽ��� �����ϴ� ����� �����ϴ°� ������ ��� ����)
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

    #region ���� ����

    protected IEnumerator Action_Move() // �ۼ���
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


    protected IEnumerator Action_Attack() // �ۼ���
    {
        while (_actionList[0].TargetDistance(gameObject.transform.position) <= unit.unitState._attackRange)
        {
            yield return delayAttackTiming;
            if (isSkip == true || _actionList[0].ExitEvent == true) break;
            if (_actionList[0]._unit.EventDamage(unit) == false)
            {
                _actionList[0].SetExitEvent(); // ������ ������ ��������� ���䰡 �ʿ��� 1. ���ó��(�ڵ� ȣ��� ���� ������), AIŸ�ٺ���, �ൿ����
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
    /// (���) 
    /// <br> �ܺο��ο� ���ؼ� AI������ ����Ǿ���� ��Ȳ�� ������մϴ�. </br>
    /// <br> ���� ����� ����ϰų�, �̵��� ���ο� ���� ����, �Ǵ� �ڽ��� �����ϴµ� �켱���� ����Ǵ� ��Ⱦ�� ȣ��˴ϴ�.</br>
    /// </summary>
    /// <param name="objectID"></param>
    public void OnChangingEvent(int objectID) // �ۼ���
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
