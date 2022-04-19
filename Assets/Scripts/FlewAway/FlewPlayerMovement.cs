using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlewPlayerMovement : MonoBehaviour
{
    [SerializeField] public Transform glideSphere;
    [SerializeField] Transform center;

    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();    
    }

    private void FixedUpdate()
    {
        int leftOrRight = -1;
        _rb.AddForce(leftOrRight * Vector3.right * 50, ForceMode.Acceleration);
        _rb.AddForceAtPosition(leftOrRight * (center.transform.position - _rb.transform.position).normalized * 5, glideSphere.transform.position, ForceMode.Acceleration);
    }
}
