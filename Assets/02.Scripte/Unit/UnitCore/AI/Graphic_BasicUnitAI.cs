using Codice.Client.Common.GameUI;
using Codice.CM.WorkspaceServer.Tree.GameUI.Checkin.Updater;
using JetBrains.Annotations;
using log4net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class Graphic_BasicUnitAI
{

    [SerializeField]
    private CharacterController UnitCtrl = null;

    public AIType _type = AIType.A;

    protected List<ActionInfo> _actionList = new List<ActionInfo>();

    public GameObject _thisObject = null;

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

    public enum AIType
    {
        A = 0,
        B,
        C
    }

    public enum Pattern
    {
        Attacking,
        Avoidng,
        RunningAway,
        Flollow,
        Push,
        Stern,
        Skill,
        SpecialSkill,
        Death,
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

    public bool AutoScheduler(Pattern AIpattern)
    {
        return true;
    }

    #region ¿Ø¥÷ µø¿€
    protected bool Action_Move(ActionInfo action)
    {
        UnitCtrl.Move(action.TargetDirectionVec3());
        return false;
    }
    #endregion
    

    public void OnChangingEvent(int objectID)
    {
        foreach (var action in _actionList)
        {
            if(action._targetID == objectID)
            {
                action.SetExitEvent();
            } 
        }
    }

}
