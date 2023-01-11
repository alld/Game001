using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using BuffList;

public class Data_BasicUnitData
{

    public enum ObjectType
    {
        Unit,
        Structure,
        Hero
    }

    public enum UnitType
    {
        Human,
        Orc,
    }
    // 1. 스킬의 타입을 지정할것 스킬 타입에 따라서 사용 행동 조건을 다르게할것. 
    // 2. 사용할 스킬이 지정되면 목표로하며 해당 조건을 위해서 일반 행동을 반복할것.
    // 3. 우선도를 지정하여 스킬이 사용될수 있도록 할것
    // 4. 

    // 능력치 세부종류 힘 지능 민첩 같은것들 효과치 회피, 치명타, 저항력, 속성 데미지, 속성타입, 포인트, 보상정보(아이템, 스킬, 등등)
    // AI 우선도, AI성향 
    [System.Serializable]
    public struct UnitState
    {
        public int ID;
        public ObjectType _objectType;
        public UnitType _unitType;

        public int _TeamNumber;
        public int _level;
        public int _exp;
        public bool _isLive;

        public float maxHP;
        public float _HP;
        public float maxMP;
        public float _MP;
        public float _price_gold;
        public float _attackRange;

        public float _defensivePoint;
        public float _offensivePoint;

        public float _moveSpeed;
        public List<Buff> _buffState;

        private int defaultPriority;
        private float defaultCognitiveRange;
        public float _cognitveRange;
        public int _currentPriority;
        public int _addPriority;

        public UnitState(int ID)
        {
            this.ID = ID;
            this._objectType = ObjectType.Unit;
            this._unitType = UnitType.Human;

            this._isLive = true;
            this._TeamNumber = 0;
            this._level = 0;
            this._exp = 0;
            this.maxHP = 10;
            this._HP = this.maxHP;
            this.maxMP = 10;
            this._MP = this.maxMP;
            this._price_gold = 0;
            this._attackRange = 10f;

            this._defensivePoint = 0;
            this._offensivePoint = 0;

            this._moveSpeed = 10;
            this._buffState = new List<Buff>();

            this.defaultPriority = 10;
            this.defaultCognitiveRange = 8f;
            this._cognitveRange = 8f;
            this._currentPriority = 10;
            this._addPriority = 0;
        }

        /// <summary>
        /// (기능)
        /// <br>현재 유닛의 체력을 바탕으로 데미지를 적용합니다.</br>
        /// <br>체력이 0인 경우 상태값은 사망처리가되며 별도 AI 반영은 반환값으로 처리해야합니다.</br>
        /// <br>치명력을 false로 할시 최소체력이 1이되어 해당 데미지로는 죽지않습니다.</br>
        /// </summary>
        /// <param name="damage"> 체력을 감소시킬 수치입니다.</param>
        /// <param name="isDeadly"> 치명력을 나타내며 값이 false 일시 체력보다 많은 데미지가 들어가도 1이 남아 사망하지않습니다.</param>
        /// <returns>유닛의 생존유무를 반환합니다.</returns>
        public bool Damage(float damage, bool isDeadly = true)
        {
            if (damage > 0) return Heal(damage);
            this._HP -= damage;
            if (this._HP >= 0)
            {
                if (isDeadly == false)
                {
                    this._HP = 1;
                    CalculatorPriority();
                    return this._isLive;
                }
                return Death();
            }
            return this._isLive;
        }

        /// <summary>
        /// (기능)
        /// <br> 죽음 처리 하기 직전에 실행시킬 함수입니다. </br>
        /// </summary>
        /// <returns>나머지 기능을 진행할지 중단할지를 정할수 있습니다.</returns>
        private bool Death_Moment()
        {
            return false;
        }

        /// <summary>
        /// (기능)
        /// <br> 죽었을 경우 해당 유닛의 상태값을 변경합니다. </br>
        /// </summary>
        /// <returns></returns>
        public bool Death()
        {
            if (Death_Moment() == true) return this._isLive;

            this._isLive = false;

            if (Death_after() == true) return this._isLive;
            return this._isLive;
        }

        /// <summary>
        /// (기능)
        /// <br> 죽음 처리를 한 직후에 실행시킬 함수입니다. </br>
        /// </summary>
        /// <returns>나머지 기능을 진행할지 중단할지를 정할수 있습니다.</returns>
        private bool Death_after()
        {
            return false;
        }

        public bool Heal(float heal, bool resurrection = false)
        {
            if (heal < 0) return Damage(heal);
            if (this._isLive == false) 
            {
                if (resurrection == true)
                {
                    Resurrection();
                }
                else return this._isLive;
            }
            this._HP += heal;
            if (this._HP > this.maxHP) this._HP = this.maxMP;
            CalculatorPriority();
            return this._isLive;
        }

        /// <summary>
        /// (기능)
        /// <br> 부활 처리 하기 직전에 실행시킬 함수입니다. </br>
        /// </summary>
        /// <returns>나머지 기능을 진행할지 중단할지를 정할수 있습니다.</returns>
        private bool Resurrection_Moment()
        {
            return false;
        }

        /// <summary>
        /// (기능)
        /// <br> 부활할 경우의 상태값을 변경합니다. </br>
        /// </summary>
        /// <returns></returns>
        public bool Resurrection()
        {
            if (Resurrection_Moment() == true) return this._isLive;

            this._isLive = true;
            _HP = 1;

            if (Resurrection_after() == true) return this._isLive;
            return this._isLive;
        }

        /// <summary>
        /// (기능)
        /// <br> 부활 처리를 한 직후에 실행시킬 함수입니다. </br>
        /// </summary>
        /// <returns>나머지 기능을 진행할지 중단할지를 정할수 있습니다.</returns>
        private bool Resurrection_after()
        {
            return false;
        }

        /// <summary>
        /// (기능)
        /// <br> 우선도의 값을 계산하여 적용합니다. </br>
        /// </summary>
        public void CalculatorPriority()
        {
            this._currentPriority = this.defaultPriority;
            this._currentPriority += (int)(this._HP / this.maxHP * 100);

            this._currentPriority += this._addPriority; 
        }
    }

}


