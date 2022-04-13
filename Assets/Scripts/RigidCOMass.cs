using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class RigidCOMass : MonoBehaviour
{
    [SerializeField] GameObject centerOfMass;
    Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = transform.InverseTransformPoint(centerOfMass.transform.position);
    }

}
