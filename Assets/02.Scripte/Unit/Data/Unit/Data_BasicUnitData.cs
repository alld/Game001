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
    /// <summary> 확률 계산에 표준편차를 구하기위한 반복횟수입니다. </summary>
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

    // 능력치 세부종류 힘 지능 민첩 같은것들  , , ,  , , 포인트, 보상정보(아이템, 스킬, 등등)
    [System.Serializable]
    public struct UnitState
    {
        /// <summary> 게임오브젝트의 인스턴스 ID을 의미합니다. </summary>
        public int ID;
        /// <summary> 유닛, 영웅, 구조물과 같은 오브젝트 형태를 구분합니다. </summary>
        public ObjectType _objectType;
        /// <summary> 종족과 같이 실제 유닛의 타입을 구분합니다. </summary>
        public UnitType _unitType;

        /// <summary> 해당 유닛의 소유자를 구분합니다. 적, 아군을 구분하는데 주로 사용됩니다.</summary>
        public int _TeamNumber;
        /// <summary> 해당 유닛의 레벨을 나타냅니다. </summary>
        public int _level;
        /// <summary> 레벨을 상승시키기위한 경험치량입니다.</summary>
        public int _exp;
        /// <summary> (상태) 유닛이 살아있는지를 확인하는 변수입니다.</summary>
        public bool _isLive;

        /// <summary> 유닛의 최대체력입니다.</summary>
        public float maxHP;
        /// <summary> 유닛의 현재체력입니다.</summary>
        public float _HP;
        /// <summary> 유닛의 최대 마나입니다.</summary>
        public float maxMP;
        /// <summary> 유닛의 현재 마나입니다.</summary>
        public float _MP;
        /// <summary> 유닛의 판매 가격을 말합니다. 현상금과는 다릅니다.</summary>
        public float _price_gold;
        /// <summary> 유닛의 공격범위를 나타냅니다.</summary>
        public float _attackRange;

        /// <summary> 방어에 사용되는 수치입니다.</summary>
        public float _defensivePoint;
        /// <summary> 공격에 사용되는 수치입니다.</summary>
        public float _offensivePoint;

        /// <summary> 이동 속도를 반영하는 수치입니다.</summary>
        public float _moveSpeed;
        /// <summary> 현재 해당유닛이 걸려있는 버프 종류를 나타냅니다. 버프가 제거되면 해당효과도 사라집니다.</summary>
        public List<Buff> _buffState;

        /// <summary> 초기 우선도를 나타냅니다. 우선도는 AI가 판단하기에 가장 우선시해야되는 대상을 말합니다.</summary>
        private int defaultPriority;
        /// <summary> 인식 범위입니다. AI가 판단할수있는 범위를 말합니다. </summary>
        private float defaultCognitiveRange;
        /// <summary> 가변적인 인식범위입니다. 상황에따라 AI의 인식범위를 변경시킬수 있습니다.</summary>
        public float _cognitveRange;
        /// <summary> 가변적인 우선도입니다. 상황에 따라 AI의 우선도를 변경시킬수 있습니다. 주로 상태값들을 기반으로 반영됩니다. </summary>
        public int _currentPriority;
        /// <summary> 우선도에 반영되는 추가 가산치입니다. 해당 수치는 시간에 따른 변화나 외부환경에 영향을 많이받는 형태에 주로 사용됩니다.</summary>
        public int _addPriority;

        /// <summary> 정확도입니다. 회피를 계산하는것과 관계있습니다. 표준편차에 따른 확률이 적용됩니다. </summary>
        public int _accuracy;
        /// <summary> 회피 수치입니다. 적의 정확도에따라서 공격을 완전 회피할수 있습니다. 표준편차에 따른 확률이 적용됩니다. </summary>
        public int _avoid;

        /// <summary> 공격 타입입니다. 공격타입에 따른 상성효과가 존재하여 데미지량의 차이를 발생시킵니다. (배율) </summary>
        Data_UnitRelationship.eAttackType _attackType;
        /// <summary> 방어 타입입니다. 방어타입에 따른 상성효과가 존재하여 데미지량의 차이를 발생시킵니다. (배율) </summary>
        Data_UnitRelationship.eDefendType _defendType;
        /// <summary> 속성입니다. 공격대상과 방어대상의 속성에 따라 데미지량의 차이를 발생시킵니다. (배율) </summary>
        Data_UnitRelationship.eProperty _property;

        /// <summary> 급소 방어는 치명타 발생율을 줄여줍니다. 해당수치는 표준편차가 적용되지 않습니다. </summary>
        public int _vitaldefend;
        /// <summary> 치명타가 발생할 확률을 나타냅니다. 치명타가 발생하면 최종데미지를 증가시킵니다. (배율) </summary>
        public int _critical;
        /// <summary> 치명타가 발생했는지 여부를 확인합니다. 주로 유동텍스트를 표현할때 사용됩니다. </summary>
        public bool criticalHit;
        /// <summary> 치명타가 발생했을시 어느정도 배율의 데미지를 증가시킬것인지 나타내는 수치입니다. </summary>
        public float _criticalrate;

        /// <summary> 현재 보호막 수치입니다. 일반적인 경우 보호막 수치에 따라 피해를 경감시킬수 있습니다. </summary>
        public float _protectPoint;
        /// <summary> 보호막 수치의 기준값입니다. </summary>
        private float _defaultProtectPoint;

        /// <summary> 예리함 수치에 따라서 상대의 방어를 무시할수 있습니다. (가감) </summary>
        public float _sharp;

        /// <summary> AI가 판단하는데 사용되는 공포 수치입니다. 공포수치가 정신보다 높을 경우 해당유닛은 도주행동과 같은 기피행동을 합니다. </summary>
        public int _fear;
        /// <summary> AI가 판단하는데 사용되는 정신 수치입니다. 공포에 대한 저향력을 나타냅니다. </summary>
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
        /// <br>보호막에 피해를 가합니다.</br> 
        /// </summary>
        /// <param name="damage"></param>
        /// <returns>보호막에 피해를 가하고 남은 데미지</returns>
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
        /// (기능)
        /// <br> 우선도의 값을 계산하여 적용합니다. </br>
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
        /// (기능)
        /// <br> 명중률과 회피를 계산합니다. 회피될시 데미지 계산을 패스하고 회피연출을 시작합니다.</br>
        /// </summary>
        /// <param name="accuracy"></param>
        /// <param name="avoid"></param>
        /// <returns><br>true : 회피 성공</br><br>false : 회피 실패</br></returns>
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
        /// (기능)
        /// <br> 치명타율을 계산합니다. </br>
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="critical"></param>
        /// <param name="_criticalrate"></param>
        /// <returns>치명타율이 계산된 데미지</returns>
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
        /// (기능) 
        /// <br>방어 수치를 계산합니다. 예리함수치에 따라 방어계산량을 감소시킵니다.</br>
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="sharp"></param>
        /// <param name="returnDamage"></param>
        /// <returns><br>true : 방어 성공</br><br>false : 방어 실패</br></returns>
        private bool CalculatorDefende(float damage, float sharp, out float returnDamage)
        {
            returnDamage = damage - ((sharp < this._defensivePoint) ? this._defensivePoint - sharp : 0);

            if(returnDamage != 0) return false;
            EffectBlock();
            return true;
        }

        /// <summary>
        /// (연출)
        /// </summary>
        private void EffectAvoid()
        {
            
        }


        /// <summary>
        /// (연출)
        /// </summary>
        private void EffectBlock()
        {

        }
    }
}


