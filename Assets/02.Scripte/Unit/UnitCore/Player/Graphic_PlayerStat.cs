using System.Collections;
using System.Collections.Generic;
//using UnityEngine.InputSystem;
using UnityEngine;

public class Graphic_PlayerStat : MonoBehaviour
{
    public Animator _animator = null;
    public Panel_BasicUnitController _player = null;
    private CharacterController unitCtrl;

    private Coroutine _currentAction = null;

    private bool _isMoving = false;
    private bool _isSkip = false;

    public void Init()
    {
        _player = GetComponent<Panel_BasicUnitController>();

        unitCtrl = GetComponent<CharacterController>();
    }

    private static Vector3 HpbarHeight = new Vector3(0, 1.5f, 0);
    public void HPBarMove()
    {
        _player._HPbar._bar.transform.position = Camera.main.WorldToScreenPoint(transform.position + HpbarHeight);
    }

    public void OnUnitMove(RaycastHit hit)
    {
        if (_currentAction != null) StopCoroutine(_currentAction);
        _currentAction = StartCoroutine(Action_Move(hit));
    }

    protected IEnumerator Action_Move(RaycastHit hit) // 작성중
    {
        _isMoving = true;
        WaitForSeconds temp_deltaTime = new WaitForSeconds(Time.deltaTime);
        //공중에 잇는경우 심플무브 작동... 그라운드..체크..
        while (_isMoving)
        {
            unitCtrl.Move(TargetMovePointVec3(hit.point, _player.unitState._moveSpeed));
            HPBarMove();
            if (_isSkip == true) break;
            if (Vector3.Distance(transform.position, hit.point) < 0.1f) _isMoving = false;
            yield return temp_deltaTime;
        }
        _currentAction = null;
    }


    public Vector3 TargetDirectionVec3(Vector3 position)
    {
        return (position - transform.position).normalized;
    }

    public Vector3 TargetMovePointVec3(Vector3 position, float moveSpeed)
    {
        return TargetDirectionVec3(position) * moveSpeed * Time.deltaTime;
    }
}
