using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PIDJoint : MonoBehaviour
{
    [Header("PID")]
    [SerializeField] float frequency = 50f;
    [SerializeField] float damping = 1f;
    [SerializeField] float rotFrequency = 100f;
    [SerializeField] float rotDamping = 0.9f;
    [SerializeField] Rigidbody playerRigidbodyMovement;
    [SerializeField] public ActionBasedController controller; //target is the controller
    [SerializeField] public GameObject target;

    [Space]

    [Header("Springs")]
    [SerializeField] float climbForce = 1000f;
    [SerializeField] float climbDrag = 500f;

    [Space]

    [Header("Values")]
    [SerializeField] float distance = 50f;

    Vector3 _previousPlayerPosition;

    Vector3 _previousPosition;
    Rigidbody _rigidbody;
    bool _isColliding;

    private Collision _collision;
    // Start is called before the first frame update
    void Start()
    {
        //Initialization
        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;

        //Setup
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.maxAngularVelocity = float.PositiveInfinity;

        _previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        PIDMovement();
        PIDRotation();
        if (_isColliding) HookesLaw(); 
        DistanceCheck();
    }

    private void DistanceCheck()
    {
        if (Math.Abs(Vector3.Distance(target.transform.position, transform.position)) > distance)
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
        }
    }

    void PIDMovement()
    {
        float kp = (6f * frequency) * (6f * frequency) * 0.25f;
        float kd = 4.5f * frequency * damping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Vector3 forceMovement = (target.transform.position - transform.position) * ksg + ((playerRigidbodyMovement.transform.position - _previousPlayerPosition) - _rigidbody.velocity) * kdg;

        _rigidbody.AddForce(forceMovement, ForceMode.Acceleration);

        _previousPlayerPosition = playerRigidbodyMovement.transform.position;
    }

    void PIDRotation()
    {
        float kp = (6f * rotFrequency) * (6f * rotFrequency) * 0.25f;
        float kd = 4.5f * rotFrequency * rotDamping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Quaternion q = target.transform.rotation * Quaternion.Inverse(transform.rotation);
        if (q.w < 0)
        {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        Vector3 torque = ksg * axis * angle + -_rigidbody.angularVelocity * kdg;

        _rigidbody.AddTorque(torque, ForceMode.Acceleration);
    }
    private void OnCollisionEnter(Collision collision)
    {
        _isColliding = true;
        _collision = collision;
    }

    private void OnCollisionExit(Collision collision)
    {
        _isColliding = false;
        _collision = null;
    }

    public void HookesLaw()
    {
        Vector3 displacementFromResting = transform.position - target.transform.position;
        Vector3 force = displacementFromResting * climbForce;

        float drag = GetDrag();

        playerRigidbodyMovement.AddForce(force, ForceMode.Acceleration);
        playerRigidbodyMovement.AddForce(drag * -playerRigidbodyMovement.velocity * climbDrag, ForceMode.Acceleration);
    }

    float GetDrag()
    {
        Vector3 handVelocity = (target.transform.localPosition - _previousPosition) / Time.fixedDeltaTime; //prevpos is from prev frame
        float drag = 1 / handVelocity.magnitude + 0.01f; //add .01 bc we don't want it to ever be 0
        drag = drag > 1 ? 1 : drag; //if drag is greater than 1 set to 1 otherwise it's just drag
        drag = drag < 0.03f ? 0.03f : drag;

        _previousPosition = transform.position;
        return drag;
    }

}
