using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Panel_BasicUnitController;

public class Graphic_Model : MonoBehaviour
{
    public SkinnedMeshRenderer _meshRenderer;
    public Animator _animator;

    public void SetMesh(eUnitKind unittype)
    {
        switch (unittype)
        {
            case eUnitKind.A:
                _meshRenderer.sharedMesh = GameManager._instance._unitManager._meshList[0];
                _meshRenderer.material = GameManager._instance._unitManager._materialList[0];
                break;
            case eUnitKind.B:
                break;
            case eUnitKind.C:
                break;
            default:
                break;
        }
    }
}
