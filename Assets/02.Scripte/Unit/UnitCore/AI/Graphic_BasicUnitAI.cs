using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Graphic_BasicUnitAI
{
    private int defaultPriority = 10;
    private float defaultCognitiveRange = 0;
    public float _cognitveRange = 0;
    public int _currnetPriority = 10;
    public AIType _type = AIType.A;

    public enum AIType
    {
        A = 0,
        B,
        C
    }

    public enum Pattern
    {
        A
    }
    
    public enum Action
    {
        A
    }

    public bool AutoScheduler(Pattern AIpattern)
    {
        return true;
    }
}
