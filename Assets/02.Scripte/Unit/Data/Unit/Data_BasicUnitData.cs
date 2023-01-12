using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using BuffList;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Build.Content;
using System.ComponentModel;

public class Data_BasicUnitData
{
    /// <summary> Ȯ�� ��꿡 ǥ�������� ���ϱ����� �ݺ�Ƚ���Դϴ�. </summary>
    public static int standardDeviation = 3;
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

    // �ɷ�ġ �������� �� ���� ��ø �����͵�  , , ,  , , ����Ʈ, ��������(������, ��ų, ���)
    [System.Serializable]
    public struct UnitState
    {
        /// <summary> ���ӿ�����Ʈ�� �ν��Ͻ� ID�� �ǹ��մϴ�. </summary>
        public int ID;
        /// <summary> ����, ����, �������� ���� ������Ʈ ���¸� �����մϴ�. </summary>
        public ObjectType _objectType;
        /// <summary> ������ ���� ���� ������ Ÿ���� �����մϴ�. </summary>
        public UnitType _unitType;

        /// <summary> �ش� ������ �����ڸ� �����մϴ�. ��, �Ʊ��� �����ϴµ� �ַ� ���˴ϴ�.</summary>
        public int _TeamNumber;
        /// <summary> �ش� ������ ������ ��Ÿ���ϴ�. </summary>
        public int _level;
        /// <summary> ������ ��½�Ű������ ����ġ���Դϴ�.</summary>
        public int _exp;
        /// <summary> (����) ������ ����ִ����� Ȯ���ϴ� �����Դϴ�.</summary>
        public bool _isLive;

        /// <summary> ������ �ִ�ü���Դϴ�.</summary>
        public float maxHP;
        /// <summary> ������ ����ü���Դϴ�.</summary>
        public float _HP;
        /// <summary> ������ �ִ� �����Դϴ�.</summary>
        public float maxMP;
        /// <summary> ������ ���� �����Դϴ�.</summary>
        public float _MP;
        /// <summary> ������ �Ǹ� ������ ���մϴ�. ����ݰ��� �ٸ��ϴ�.</summary>
        public float _price_gold;
        /// <summary> ������ ���ݹ����� ��Ÿ���ϴ�.</summary>
        public float _attackRange;

        /// <summary> �� ���Ǵ� ��ġ�Դϴ�.</summary>
        public float _defensivePoint;
        /// <summary> ���ݿ� ���Ǵ� ��ġ�Դϴ�.</summary>
        public float _offensivePoint;

        /// <summary> �̵� �ӵ��� �ݿ��ϴ� ��ġ�Դϴ�.</summary>
        public float _moveSpeed;
        /// <summary> ���� �ش������� �ɷ��ִ� ���� ������ ��Ÿ���ϴ�. ������ ���ŵǸ� �ش�ȿ���� ������ϴ�.</summary>
        public List<Buff> _buffState;

        /// <summary> �ʱ� �켱���� ��Ÿ���ϴ�. �켱���� AI�� �Ǵ��ϱ⿡ ���� �켱���ؾߵǴ� ����� ���մϴ�.</summary>
        private int defaultPriority;
        /// <summary> �ν� �����Դϴ�. AI�� �Ǵ��Ҽ��ִ� ������ ���մϴ�. </summary>
        private float defaultCognitiveRange;
        /// <summary> �������� �νĹ����Դϴ�. ��Ȳ������ AI�� �νĹ����� �����ų�� �ֽ��ϴ�.</summary>
        public float _cognitveRange;
        /// <summary> �������� �켱���Դϴ�. ��Ȳ�� ���� AI�� �켱���� �����ų�� �ֽ��ϴ�. �ַ� ���°����� ������� �ݿ��˴ϴ�. </summary>
        public int _currentPriority;
        /// <summary> �켱���� �ݿ��Ǵ� �߰� ����ġ�Դϴ�. �ش� ��ġ�� �ð��� ���� ��ȭ�� �ܺ�ȯ�濡 ������ ���̹޴� ���¿� �ַ� ���˴ϴ�.</summary>
        public int _addPriority;

        /// <summary> ��Ȯ���Դϴ�. ȸ�Ǹ� ����ϴ°Ͱ� �����ֽ��ϴ�. ǥ�������� ���� Ȯ���� ����˴ϴ�. </summary>
        public int _accuracy;
        /// <summary> ȸ�� ��ġ�Դϴ�. ���� ��Ȯ�������� ������ ���� ȸ���Ҽ� �ֽ��ϴ�. ǥ�������� ���� Ȯ���� ����˴ϴ�. </summary>
        public int _avoid;

        /// <summary> ���� Ÿ���Դϴ�. ����Ÿ�Կ� ���� ��ȿ���� �����Ͽ� ���������� ���̸� �߻���ŵ�ϴ�. (����) </summary>
        Data_UnitRelationship.eAttackType _attackType;
        /// <summary> ��� Ÿ���Դϴ�. ���Ÿ�Կ� ���� ��ȿ���� �����Ͽ� ���������� ���̸� �߻���ŵ�ϴ�. (����) </summary>
        Data_UnitRelationship.eDefendType _defendType;
        /// <summary> �Ӽ��Դϴ�. ���ݴ��� ������� �Ӽ��� ���� ���������� ���̸� �߻���ŵ�ϴ�. (����) </summary>
        Data_UnitRelationship.eProperty _property;

        /// <summary> �޼� ���� ġ��Ÿ �߻����� �ٿ��ݴϴ�. �ش��ġ�� ǥ�������� ������� �ʽ��ϴ�. </summary>
        public int _vitaldefend;
        /// <summary> ġ��Ÿ�� �߻��� Ȯ���� ��Ÿ���ϴ�. ġ��Ÿ�� �߻��ϸ� ������������ ������ŵ�ϴ�. (����) </summary>
        public int _critical;
        /// <summary> ġ��Ÿ�� �߻��ߴ��� ���θ� Ȯ���մϴ�. �ַ� �����ؽ�Ʈ�� ǥ���Ҷ� ���˴ϴ�. </summary>
        public bool criticalHit;
        /// <summary> ġ��Ÿ�� �߻������� ������� ������ �������� ������ų������ ��Ÿ���� ��ġ�Դϴ�. </summary>
        public float _criticalrate;

        /// <summary> ���� ��ȣ�� ��ġ�Դϴ�. �Ϲ����� ��� ��ȣ�� ��ġ�� ���� ���ظ� �氨��ų�� �ֽ��ϴ�. </summary>
        public float _protectPoint;
        /// <summary> ��ȣ�� ��ġ�� ���ذ��Դϴ�. </summary>
        private float _defaultProtectPoint;

        /// <summary> ������ ��ġ�� ���� ����� �� �����Ҽ� �ֽ��ϴ�. (����) </summary>
        public float _sharp;

        /// <summary> AI�� �Ǵ��ϴµ� ���Ǵ� ���� ��ġ�Դϴ�. ������ġ�� ���ź��� ���� ��� �ش������� �����ൿ�� ���� �����ൿ�� �մϴ�. </summary>
        public int _fear;
        /// <summary> AI�� �Ǵ��ϴµ� ���Ǵ� ���� ��ġ�Դϴ�. ������ ���� ������� ��Ÿ���ϴ�. </summary>
        public int _mental;

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

            this._accuracy = 0;
            this._avoid = 0;

            this._attackType = Data_UnitRelationship.eAttackType.A;
            this._defendType = Data_UnitRelationship.eDefendType.A;
            this._property = Data_UnitRelationship.eProperty.A;

            this._vitaldefend = 0;
            this._critical = 0;
            this._criticalrate = 0;
            this.criticalHit = false;

            this._protectPoint = 0;
            this._defaultProtectPoint = 0;

            this._sharp = 0;

            this._fear = 0;
            this._mental = 0;
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
        /// <br>��ȣ���� ���ظ� ���մϴ�.</br> 
        /// </summary>
        /// <param name="damage"></param>
        /// <returns>��ȣ���� ���ظ� ���ϰ� ���� ������</returns>
        public float DamageProtect(float damage)
        {
            if (this._protectPoint <= damage) 
            { 
                damage -= this._protectPoint;
                this._protectPoint = 0;
            }
            else
            {
                this._protectPoint -= damage;
                damage = 0;
            }
            return damage;
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

        public bool CalculatorDamage(UnitState opponent, float damage = 0)
        {
            this.criticalHit = false;
            if (damage == 0) damage = opponent._offensivePoint;
            if (CalculatorAvoid(opponent._accuracy)) return _isLive;

            damage = DamageProtect(damage);

            if (CalculatorDefende(damage, opponent._sharp, out damage)) return _isLive;

            damage *= Data_UnitRelationship.GetRelationship(opponent._attackType, this._defendType) * Data_UnitRelationship.GetProperty(opponent._property, this._property);

            return Damage(CalculatorCritical(damage, opponent._critical, opponent._criticalrate));
        }


        public bool CalculatorDamage(UnitState opponent, bool IgnoreDefend, bool IgnoreProtect, float damage = 0)
        {
            if (damage == 0) damage = opponent._offensivePoint;
            if (CalculatorAvoid(opponent._accuracy)) return _isLive;

            if(IgnoreProtect== false) DamageProtect(damage);

            if (IgnoreDefend == false)
            {
                if (CalculatorDefende(damage, opponent._sharp, out damage)) return _isLive;
            }

            damage *= Data_UnitRelationship.GetRelationship(opponent._attackType, this._defendType) * Data_UnitRelationship.GetProperty(opponent._property, this._property);

            return Damage(CalculatorCritical(damage, opponent._critical, opponent._criticalrate));
        }



        /// <summary>
        /// (���)
        /// <br> ���߷��� ȸ�Ǹ� ����մϴ�. ȸ�ǵɽ� ������ ����� �н��ϰ� ȸ�ǿ����� �����մϴ�.</br>
        /// </summary>
        /// <param name="accuracy"></param>
        /// <param name="avoid"></param>
        /// <returns><br>true : ȸ�� ����</br><br>false : ȸ�� ����</br></returns>
        private bool CalculatorAvoid(int accuracy)
        {
            int ran_accuracy = 0, ran_avoid = 0;
            for (int i = 0; i < standardDeviation; i++)
            {
                ran_accuracy += Random.Range(0, accuracy);
                ran_avoid += Random.Range(0, this._avoid);
            }
            ran_accuracy /= standardDeviation;
            ran_avoid /= standardDeviation;

            if (ran_accuracy < ran_avoid)
            {
                EffectAvoid();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// (���)
        /// <br> ġ��Ÿ���� ����մϴ�. </br>
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="critical"></param>
        /// <param name="_criticalrate"></param>
        /// <returns>ġ��Ÿ���� ���� ������</returns>
        private float CalculatorCritical(float damage, int critical, float _criticalrate)
        {
            if (Random.Range(0, critical) < Random.Range(0, this._vitaldefend))
            {
                this.criticalHit = true;
                return damage * _criticalrate;
            }
            else return damage;
        }

        /// <summary>
        /// (���) 
        /// <br>��� ��ġ�� ����մϴ�. �����Լ�ġ�� ���� ����귮�� ���ҽ�ŵ�ϴ�.</br>
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="sharp"></param>
        /// <param name="returnDamage"></param>
        /// <returns><br>true : ��� ����</br><br>false : ��� ����</br></returns>
        private bool CalculatorDefende(float damage, float sharp, out float returnDamage)
        {
            returnDamage = damage - ((sharp < this._defensivePoint) ? this._defensivePoint - sharp : 0);

            if(returnDamage != 0) return false;
            EffectBlock();
            return true;
        }

        /// <summary>
        /// (����)
        /// </summary>
        private void EffectAvoid()
        {
            
        }


        /// <summary>
        /// (����)
        /// </summary>
        private void EffectBlock()
        {

        }
    }
}


