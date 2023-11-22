using System;
using Controllers;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DisableWaypoint : MonoBehaviour
{
    public GameObject waypointToDeactivate;
    private BoxCollider boxCollider;

    private void Start()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider) boxCollider = new BoxCollider();
        boxCollider.isTrigger = true;
        boxCollider.size = Vector3.one * 2;
        if (!waypointToDeactivate) throw new Exception("NO WAYPOINTS SET!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        waypointToDeactivate.SetActive(false);
    }
}