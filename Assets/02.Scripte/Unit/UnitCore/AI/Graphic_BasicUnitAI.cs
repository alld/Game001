using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Graphic_BasicUnitAI : MonoBehaviour
{

    #region ĳ��ó��
    private CharacterController unitCtrl = null;
    [HideInInspector]
    public GameObject _thisObject = null;
    private WaitForSeconds delayTime = null;
    private WaitForSeconds awakeCheekTime = null;
    #endregion

    [Range(0.01f, 2f)]
    public float _loopCheckDelay = 0.01f;
    [Range(0.5f, 5f)]
    public float _loopSleepDelay = 3.0f;

    public AIType _type = AIType.A;
    protected List<ActionInfo> _actionList = new List<ActionInfo>();
    protected List<Pattern> _scheduler = new List<Pattern>();
    protected IEnumerator _currentPattern = null;
    protected bool isSkip = false;

    public struct ActionInfo
    {
        public bool ExitEvent;
        public int _targetID;
        public Action _kind;

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
            this._kind = Action.Idle;

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
    public enum AIType
    {
        A = 0,
        B,
        C
    }

    public enum Pattern
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

    public enum Action
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
    }


    #region �����췯 ����
    public Pattern PatternSearch() // �ۼ���
    {
        return Pattern.Stand;
    }

    public bool AutoScheduler(Pattern AIPattern, bool rightoff = false)
    {
        if (Pattern.Death == AIPattern) // ���ó���� ��ͺ��� �켱ó���Ǿ���ؼ� ���� ����˴ϴ�. 
        {
            ImmediateInterruption();
            _currentPattern = PT_Death();
            StartCoroutine(_currentPattern);
            return false;
        }
        else if (Pattern.Continue == AIPattern)
        {
            switch (_scheduler[0])
            {
                case Pattern.Attacking:
                    _currentPattern = PT_Attacking();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.Avoidng:
                    _currentPattern = PT_Avoidng();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.RunningAway:
                    _currentPattern = PT_RunningAway();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.Flollow:
                    _currentPattern = PT_Flollow();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.Push:
                    _currentPattern = PT_Push();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.Stern:
                    _currentPattern = PT_Stern();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.Skill:
                    _currentPattern = PT_Skill();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.Resurrection:
                    _currentPattern = PT_Resurrection();
                    StartCoroutine(_currentPattern);
                    break;
                case Pattern.Stand:
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
            return AutoScheduler(Pattern.Continue);
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

        if(_currentPattern == null)
        {
            StopCoroutine(_currentPattern);
            _currentPattern = null;
        }


        if (_scheduler.Count != 0)
        {
            _scheduler.RemoveAt(0);
            AutoScheduler(Pattern.Continue);
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
        Pattern checkPattern = Pattern.Stand;
        while(checkPattern == Pattern.Stand)
        {
            yield return awakeCheekTime;
            if (isSkip == true) break;
            checkPattern = PatternSearch();
        }

        if(checkPattern != Pattern.Stand) AutoScheduler(checkPattern);
        ActionEnd();
    }


    #region ���� ���η���

    #endregion 

    #endregion

    #endregion

    #region ���� ����
    protected bool Action_Move(ActionInfo action) // �ۼ���
    {
        if (action.ExitEvent == true)
        {
            return AutoScheduler(Pattern.Continue);
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
