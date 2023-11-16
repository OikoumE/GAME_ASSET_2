using System;
using UnityEngine;

public class WireDistanceConstraint : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float minRange, maxRange;

    private void Start()
    {
        if (!targetTransform) throw new Exception("TARGET NOT SET");
    }

    private void Update()
    {
        var currPos = transform.position;
        // Clamp position within the allowed range relative to the target transform
        var targetPosition = targetTransform.position;
        var clampedX = Mathf.Clamp(
            currPos.x,
            targetPosition.x + minRange,
            targetPosition.x + maxRange
        );
        var clampedY = Mathf.Clamp(
            currPos.y,
            targetPosition.y + minRange,
            targetPosition.y + maxRange
        );
        var clampedZ = Mathf.Clamp(
            currPos.z,
            targetPosition.z + minRange,
            targetPosition.z + maxRange
        );
        // Set the object's position
        transform.position = new Vector3(clampedX, clampedY, clampedZ);
    }
}