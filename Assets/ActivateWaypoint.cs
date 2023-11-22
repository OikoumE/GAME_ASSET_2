using System;
using Controllers;
using StateMachine;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ActivateWaypoint : MonoBehaviour
{
    public GameObject waypointToActivate;
    public GameStateName requiredGameStateName;
    private BoxCollider boxCollider;
    private GameBaseState reqState;

    private void Start()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider) boxCollider = new BoxCollider();
        boxCollider.isTrigger = true;
        if (!waypointToActivate) throw new Exception("NO WAYPOINTS SET!");
        reqState = GameStateMachine.Instance.GetState(requiredGameStateName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!GameStateMachine.Instance.IsCurrentState(requiredGameStateName))
            return;
        if (!other.TryGetComponent(out IPlayer iP))
            return;
        waypointToActivate.SetActive(true);
    }
}