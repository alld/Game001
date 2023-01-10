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

        if (checkPattern != ePattern.Stand) AutoScheduler(checkPattern);
        ActionEnd();
    }


    #region ���� ���η���
    /// <summary>
    /// (���)
    /// <br> AI�� �Ǵ��ϴµ� �ʿ��� ��� �ڷḦ �����մϴ�. </br>
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


        // �ش� ���ֵ��� ������ ���� ��ť�� ������Ŵ. �켱�� / ü��, ���� / ���ù��� / �����Ϲ��� (�������� ����) / ����,���Ͱ��� ��ġ���� / ���ɼ�ġ����
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

    #region ���� ����
    protected bool Action_Move(ActionInfo action) // �ۼ���
    {
        if (action.ExitEvent == true)
        {
            return AutoScheduler(ePattern.Continue);
        }
        unitCtrl.Move(action.TargetDirectionVec3());
        return false;
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
