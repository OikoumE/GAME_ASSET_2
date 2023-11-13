using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class KeepPlayerAsChild : MonoBehaviour
{
    private Collider mCollider;
    void Start()
    {
        mCollider = GetComponent<Collider>();
    }
    void Update()
    {
        
    }
}
