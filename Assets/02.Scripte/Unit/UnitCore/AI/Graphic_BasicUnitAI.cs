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

    #region ĳ��ó��
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
    /// (���ܴ���) 
    /// <br> �����Ϳ� ��ġ�� ���ֵ��� ���ӸŴ����� �ν��Ͻ� ������������ ���� ��� �� �Լ��� ����˴ϴ�. </br>
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
    #region Ÿ�� ����
    public enum eAIType
    {
        Normal = 0, // �Ϲ�
        UnArmed,    // ����
        Passive,    // ������
        Active,     // ������
        Aggressive, // ������
        Defensive   // ������
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


    #region �����췯 ����
    public ePattern PatternSearch() // �ۼ���
    {
        DataCollection();

        //�ӽ�

        if (_Info_nearbyEnemy.Count != 0) return ePattern.Attacking;
        return ePattern.Stand;
    }

    public bool AutoScheduler(ePattern AIPattern, bool rightoff = false)
    {
        if (ePattern.Death == AIPattern) // ���ó���� ��ͺ��� �켱ó���Ǿ���ؼ� ���� ����˴ϴ�. 
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
    /// (���)
    /// <br> ���� �������� �׼��� �������� ȣ��Ǵ� �Լ��Դϴ�. </br>
    /// <br> �׼ǰ������� ���Ǿ��� ������� �����ϴ� ����� �����ϰ��ֽ��ϴ�.</br>
    /// <br> [����] �����췯 ���� ���� �������� ȣ��Ǿ���մϴ�.  </br>
    /// <br> �����췯�� �̾ �����ϴ� ����� ���ԵǾ��ֱ� �����Դϴ�.</br>
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
        _actionList.Insert(0, new ActionInfo(_targetObject));

        while (_actionList[0].TargetDistance(gameObject.transform.position) > unit.unitState._attackRange)
        {
            Action_Move(_actionList[0]);
            if (isSkip == true) break;
            yield return delayTime;
        }

        // ���ҽ� ���� ȿ������ ���ؼ� �ش� �Լ��� �и���Ű�� ����� ����
        // 1��. Attacking ����� ����κ��� �и����Ѽ�, �׼ǿ��� �ݺ��ϰ�, ��������� ����κ��� �����Ű�� ����.
        // �ش� Action_Attack ���ο��� isSkip ��� üũ 
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
    /// (���) 
    /// <br> �ش� ������ �����·� ���Ӵϴ�. �����ð����� ��ŵ �Ǵ� </br>
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


    #region ���� ���η���
    /// <summary>
    /// (���)
    /// <br> AI�� �Ǵ��ϴµ� �ʿ��� ��� �ڷḦ �����մϴ�. </br>
    /// </summary>
    protected void DataCollection()
    {
        _targetObject = null;

        if (_Info_nearbyUnits.Count == 0) // �ֺ��� ������ ���� ���
        {
            return;
        }
        var temp_Team = _Info_nearbyTeam.OrderByDescending(Team => Team.unitState._currentPriority).Select(Team => Team).FirstOrDefault();
        var temp_Enemy = _Info_nearbyEnemy.OrderByDescending(Enemy => Enemy.unitState._currentPriority).Select(Enemy => Enemy).FirstOrDefault();

        _targetObject = (temp_Enemy?.unitState._currentPriority < temp_Team?.unitState._currentPriority) ? temp_Team?.gameObject : temp_Enemy?.gameObject;
        if (_targetObject == null) _targetObject = this.gameObject;

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

    #region ���� ����
    protected void Action_Move(ActionInfo action) // �ۼ���
    {
        if (action.ExitEvent == true)
        {
            AutoScheduler(ePattern.Continue);
            return;
        }
        unitCtrl.Move(action.TargetDirectionVec3(gameObject.transform.position));
    }

    protected void Action_Attack(ActionInfo action) // �ۼ���
    {
        if (action.ExitEvent == true)
        {
            
        }
    }
    #endregion


    public void OnChangingEvent(int objectID) // �ۼ���
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
