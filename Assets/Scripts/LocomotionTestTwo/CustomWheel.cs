using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomWheel : MonoBehaviour
{
    private Transform _dummyWheel;
    private Rigidbody _rb;

    private float _forwardSlip;
    private float _sidewaysSlip;
    private Vector3 _totalForce;
    private Vector3 _center; //The center of the wheel, measured in the object's local space.
    private Vector3 _prevPosition;

    private float _wheelMotorTorque; //Motor torque on the wheel axle. Positive or negative depending on direction.
    private float _wheelBrakeTorque; //Brake torque. Must be positive.
    private float _wheelSteerAngle; //Steering angle in degrees, always around the local y-axis.
    private float _wheelAngularVelocity; //Current wheel axle rotation speed, in rotations per minute (Read Only).
    private float _wheelRotationAngle;
    private float _wheelRadius; //The radius of the wheel, measured in local space.
    private float _wheelMass; //The mass of the wheel. Must be larger than zero.

    private float m_suspensionDistance; //Maximum extension distance of wheel suspension, measured in local space.

    //Standard accessor and mutator properties
    public Vector3 Center
    {
        set
        {
            _center = value;
            _dummyWheel.localPosition = transform.localPosition + _center;
            _prevPosition = _dummyWheel.localPosition;
        }
        get
        {
            return _center;
        }
    }

    public float WheelRadius
    {
        set
        {
            _wheelRadius = value;
        }
        get
        {
            return _wheelRadius;
        }
    }

    public float RPM
    {
        get
        {
            return _wheelAngularVelocity;
        }
    }
    

    private void Awake()
    {
        _dummyWheel = new GameObject("DummyWheel").transform;
        _dummyWheel.transform.parent = this.transform.parent;
        Center = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Find the rigidbody associated with the wheel
        GameObject parent = this.gameObject;
        while (parent != null)
        {
            if (parent.GetComponent<Rigidbody>() != null)
            {
                _rb = parent.GetComponent<Rigidbody>();
                break;
            }
            parent = parent.transform.parent.gameObject;
        }

        if (_rb == null)
        {
            Debug.LogError("WheelColliderSource: Unable to find associated Rigidbody.");
        }
    }

    private void FixedUpdate()
    {
        CalculateSlips();

        CalculateForcesFromSlips();

        _rb.AddForceAtPosition(_totalForce, transform.position);
    }

    private void CalculateSlips()
    {
        //Calculate the wheel's linear velocity
        Vector3 velocity = (_dummyWheel.position - _prevPosition) / Time.fixedDeltaTime;
        _prevPosition = _dummyWheel.position;

        //Store the forward and sideways direction to improve performance
        Vector3 forward = _dummyWheel.forward;
        Vector3 sideways = -_dummyWheel.right;

        //Calculate the forward and sideways velocity components relative to the wheel
        Vector3 forwardVelocity = Vector3.Dot(velocity, forward) * forward;
        Vector3 sidewaysVelocity = Vector3.Dot(velocity, sideways) * sideways;

        //Calculate the slip velocities. 
        //Note that these values are different from the standard slip calculation.
        _forwardSlip = -Mathf.Sign(Vector3.Dot(forward, forwardVelocity)) * forwardVelocity.magnitude + (_wheelAngularVelocity * Mathf.PI / 180.0f * _wheelRadius);
        _sidewaysSlip = -Mathf.Sign(Vector3.Dot(sideways, sidewaysVelocity)) * sidewaysVelocity.magnitude + (_wheelAngularVelocity * Mathf.PI / 180.0f * _wheelRadius);

    }

    private void CalculateForcesFromSlips()
    {
        //Forward slip force
        _totalForce = _dummyWheel.forward * Mathf.Sign(_forwardSlip);

        //Lateral slip force
        _totalForce -= _dummyWheel.right * Mathf.Sign(_sidewaysSlip);

    }
}
