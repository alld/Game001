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
    // 1. ��ų�� Ÿ���� �����Ұ� ��ų Ÿ�Կ� ���� ��� �ൿ ������ �ٸ����Ұ�. 
    // 2. ����� ��ų�� �����Ǹ� ��ǥ���ϸ� �ش� ������ ���ؼ� �Ϲ� �ൿ�� �ݺ��Ұ�.
    // 3. �켱���� �����Ͽ� ��ų�� ���ɼ� �ֵ��� �Ұ�
    // 4. 

    // �ɷ�ġ �������� �� ���� ��ø �����͵� ȿ��ġ ȸ��, ġ��Ÿ, ���׷�, �Ӽ� ������, �Ӽ�Ÿ��, ����Ʈ, ��������(������, ��ų, ���)
    // AI �켱��, AI���� 
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
        /// (���)
        /// <br>���� ������ ü���� �������� �������� �����մϴ�.</br>
        /// <br>ü���� 0�� ��� ���°��� ���ó�����Ǹ� ���� AI �ݿ��� ��ȯ������ ó���ؾ��մϴ�.</br>
        /// <br>ġ����� false�� �ҽ� �ּ�ü���� 1�̵Ǿ� �ش� �������δ� �����ʽ��ϴ�.</br>
        /// </summary>
        /// <param name="damage"> ü���� ���ҽ�ų ��ġ�Դϴ�.</param>
        /// <param name="isDeadly"> ġ����� ��Ÿ���� ���� false �Ͻ� ü�º��� ���� �������� ���� 1�� ���� ��������ʽ��ϴ�.</param>
        /// <returns>������ ���������� ��ȯ�մϴ�.</returns>
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
        /// (���)
        /// <br> ���� ó�� �ϱ� ������ �����ų �Լ��Դϴ�. </br>
        /// </summary>
        /// <returns>������ ����� �������� �ߴ������� ���Ҽ� �ֽ��ϴ�.</returns>
        private bool Death_Moment()
        {
            return false;
        }

        /// <summary>
        /// (���)
        /// <br> �׾��� ��� �ش� ������ ���°��� �����մϴ�. </br>
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
        /// (���)
        /// <br> ���� ó���� �� ���Ŀ� �����ų �Լ��Դϴ�. </br>
        /// </summary>
        /// <returns>������ ����� �������� �ߴ������� ���Ҽ� �ֽ��ϴ�.</returns>
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
        /// (���)
        /// <br> ��Ȱ ó�� �ϱ� ������ �����ų �Լ��Դϴ�. </br>
        /// </summary>
        /// <returns>������ ����� �������� �ߴ������� ���Ҽ� �ֽ��ϴ�.</returns>
        private bool Resurrection_Moment()
        {
            return false;
        }

        /// <summary>
        /// (���)
        /// <br> ��Ȱ�� ����� ���°��� �����մϴ�. </br>
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
        /// (���)
        /// <br> ��Ȱ ó���� �� ���Ŀ� �����ų �Լ��Դϴ�. </br>
        /// </summary>
        /// <returns>������ ����� �������� �ߴ������� ���Ҽ� �ֽ��ϴ�.</returns>
        private bool Resurrection_after()
        {
            return false;
        }

        /// <summary>
        /// (���)
        /// <br> �켱���� ���� ����Ͽ� �����մϴ�. </br>
        /// </summary>
        public void CalculatorPriority()
        {
            this._currentPriority = this.defaultPriority;
            this._currentPriority += (int)(this._HP / this.maxHP * 100);

            this._currentPriority += this._addPriority; 
        }
    }

}


