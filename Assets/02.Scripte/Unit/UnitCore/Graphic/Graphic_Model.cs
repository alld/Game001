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
            case eUnitKind.None:
                _meshRenderer.sharedMesh = null;
                _meshRenderer.material = null;
                break;
            case eUnitKind.A:
                _meshRenderer.sharedMesh = GameManager._instance._unitManager._meshList[0];
                _meshRenderer.material = GameManager._instance._unitManager._materialList[0];
                break;
            case eUnitKind.B:
                _meshRenderer.sharedMesh = GameManager._instance._unitManager._meshList[0];
                _meshRenderer.material = GameManager._instance._unitManager._materialList[0];
                break;
            case eUnitKind.C:
                _meshRenderer.sharedMesh = GameManager._instance._unitManager._meshList[0];
                _meshRenderer.material = GameManager._instance._unitManager._materialList[0];
                break;
            default:
                break;
        }
    }
}
