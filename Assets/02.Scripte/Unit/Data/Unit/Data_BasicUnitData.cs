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


