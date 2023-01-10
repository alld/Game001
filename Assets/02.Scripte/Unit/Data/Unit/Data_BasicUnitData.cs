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
        public ObjectType objectType;
        public UnitType unitType;

        public int TeamNumber;
        public int level;
        public int exp;

        public float maxHP;
        public float HP;
        public float maxMP;
        public float MP;
        public float price_gold;

        public float defensivePoint;
        public float offensivePoint;

        public float moveSpeed;
        public List<Buff> buffState;

        private int defaultPriority;
        private float defaultCognitiveRange;
        public float _cognitveRange;
        public int _currnetPriority;

        public UnitState(int ID)
        {
            this.ID = ID;
            this.objectType = ObjectType.Unit;
            this.unitType = UnitType.Human;

            this.TeamNumber = 0;
            this.level = 0;
            this.exp = 0;
            this.maxHP = 10;
            this.HP = this.maxHP;
            this.maxMP = 10;
            this.MP = this.maxMP;
            this.price_gold = 0;

            this.defensivePoint = 0;
            this.offensivePoint = 0;

            this.moveSpeed = 10;
            this.buffState = new List<Buff>();

            this.defaultPriority = 10;
            this.defaultCognitiveRange = 8f;
            this._cognitveRange = 8f;
            this._currnetPriority = 10;
        }

    }
}


