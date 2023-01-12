using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_UnitRelationship
{
    public enum eAttackType
    {
        A,
        B,
        C
    }

    public enum eDefendType
    {
        A,
        B,
        C
    }

    public enum eProperty
    {
        A,
        B,
        C
    }

    public static float[,] BasicRelationship = {
        { 1, 1, 1 },
        { 1, 1, 1 },
        { 1, 1, 1 }
    };

    public static float GetRelationship(eAttackType TypeA, eDefendType TypeD)
    {
        return BasicRelationship[(int)TypeA, (int)TypeD];
    }

    public static float[,] BasicProperty = {
        { 1, 1, 1 },
        { 1, 1, 1 },
        { 1, 1, 1 }
    };

    public static float GetProperty(eProperty proA, eProperty proB)
    {
        return BasicProperty[(int)proA, (int)proB];
    }
}
