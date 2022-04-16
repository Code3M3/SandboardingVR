using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]

// script that adds forces to articulation body since nothing for that is built into unity editor
public class ArtBodyConstantForce : MonoBehaviour
{
    [SerializeField] public Vector3 force;
    [SerializeField] public Vector3 relativeForce;
    [SerializeField] public Vector3 torque;
    [SerializeField] public Vector3 relativeTorque;

    ArticulationBody artBody;

    // Start is called before the first frame update
    void Start()
    {
        artBody = GetComponent<ArticulationBody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AddAllForces();
    }

    private void AddAllForces()
    {
        artBody.AddForce(force);
        artBody.AddRelativeForce(relativeForce);
        artBody.AddTorque(torque);
        artBody.AddRelativeTorque(relativeTorque);
    }
}
