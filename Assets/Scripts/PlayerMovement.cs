using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

// using this tutorial https://www.youtube.com/watch?v=ViQzKZvYdgE as a base
public class PlayerMovement : MonoBehaviour
{
    [Header("values")]
    [SerializeField] float moveForce = 2f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float steerForce = 2f;
    [SerializeField] float minForce;
    [SerializeField] float minTimeBetweenStrokes;

    public Vector3[] velocityFrames;
    public Vector3[] angularVelocityFrames;

    [Header("player references")]
    [SerializeField] GameObject leftController;
    [SerializeField] InputActionReference leftControllerMoveRef;
    [SerializeField] InputActionReference leftControllerVelocity;
    [SerializeField] InputActionReference leftControllerPosition;
    [SerializeField] InputActionReference leftControllerRotationRef;

    [SerializeField] InputActionReference leftControllerAngularVelocity;
    [SerializeField] InputActionReference rightControllerMoveRef;
    [SerializeField] InputActionReference rightControllerVelocity;
    [SerializeField] Transform forwardRef;

    [Header("player rotation")]
    [SerializeField] UnicornController unicornController;

    Rigidbody _rb;
    float _cooldownTimer;

    Vector3 _previousPosition;

    Quaternion _steerStartRotation;
    Vector3 _steerStartPosition;

    private int currentVelocityFrameStep = 0;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        leftControllerMoveRef.action.started += GetOriginalRotationPosition;
        leftControllerMoveRef.action.canceled += ClearVelocities;
    }

    private void ClearVelocities(InputAction.CallbackContext obj)
    {
        ResetVelocityHistory();
    }

    private void GetOriginalRotationPosition(InputAction.CallbackContext obj)
    {
        _steerStartRotation = unicornController.transform.rotation;
        _steerStartPosition = unicornController.transform.position;
    }

    public void PlayerMove()
    {
        VelocityUpdate();

        _cooldownTimer += Time.fixedDeltaTime;
        if (_cooldownTimer > minTimeBetweenStrokes
            && leftControllerMoveRef.action.IsPressed())
        {
            AddVelocityHistory();
        }

        if (_rb.velocity.sqrMagnitude > 0.01f)
        {
            _rb.AddForce(-_rb.velocity * dragForce, ForceMode.Acceleration);
        }
    }

    private void FixedUpdate()
    {
    }
    void AddVelocityHistory()
    {
        Vector3 velocityAverage = GetVectorAverage(velocityFrames);
        Vector3 angVelAverage = GetVectorAverage(angularVelocityFrames);
        if (velocityFrames != null)
        {
            if (velocityAverage != null)
            {
                Vector3 localVelocity = velocityAverage; // total velocity
                localVelocity *= -1; // reverse direction

                if (localVelocity.sqrMagnitude > minForce * minForce)
                {
                    // SLIDING MOVEMENT
                    // remap y velocity to right and forward vector
                    Vector3 worldVelocity = forwardRef.TransformDirection(localVelocity.x + localVelocity.y, 0 , localVelocity.z + localVelocity.y);
                    _rb.AddForce(worldVelocity * moveForce, ForceMode.Acceleration);

                    // ROTATING CHEST RB

                    // !!!Important that you use localVel here!!!
                    int TorqueDir = 1;

                    // subtract difference in player's starting rotation with current rotation from velocity
                    Quaternion currentSteerRotation = unicornController.transform.rotation;
                    Quaternion rotationOffset = currentSteerRotation * Quaternion.Inverse(_steerStartRotation);

                    // important to use local velocity here
                    // times 2 because an important reason that i know intuitively but can't put into words!! figuring out i need the times two ruined me...
                    if ((rotationOffset * (localVelocity * 2)).x < 0)
                    {
                        TorqueDir = -1;
                    }

                    // adding extra multipliers and clamps for more controlled and intuitive feel
                    float rotateForce = localVelocity.magnitude;
                    float angularThreshold = .6f;
                    float angularMultiplier;

                    Vector3 angVelIgnoreAxis = new Vector3(0, angVelAverage.y, angVelAverage.z/2);

                    if (angVelIgnoreAxis.magnitude - angularThreshold < 0)
                    {
                        angularMultiplier = 0f;
                    }
                    else
                    {
                        angularMultiplier = angVelIgnoreAxis.magnitude - angularThreshold;
                    }

                    unicornController.RotateUnicorn(TorqueDir * rotateForce * angularMultiplier);

                    _cooldownTimer = 0f;
                }
            }
        }
    }

    private void VelocityUpdate()
    {
        if (velocityFrames != null)
        {
            currentVelocityFrameStep++;

            if (currentVelocityFrameStep >= velocityFrames.Length)
            {
                currentVelocityFrameStep = 0;
            }

            // set velocity at current frame step to equal the crrent velocity and angular velocity of the object's rigidbody
            velocityFrames[currentVelocityFrameStep] = leftControllerVelocity.action.ReadValue<Vector3>();
            angularVelocityFrames[currentVelocityFrameStep] = leftControllerAngularVelocity.action.ReadValue<Vector3>();
        }
    }

    Vector3 GetVectorAverage(Vector3[] vectors)
    {
        float
            x = 0f,
            y = 0f,
            z = 0f;

        int numVectors = 0;

        for (int i = 0; i < vectors.Length; i++)
        {
            if (vectors[i] != null)
            {
                x += vectors[i].x;
                y += vectors[i].y;
                z += vectors[i].z;

                numVectors++;
            }
        }

        if (numVectors > 0)
        {
            Vector3 average = new Vector3(x / numVectors, y / numVectors, z / numVectors);
            return average;
        }

        return Vector3.one;

    }

    void ResetVelocityHistory()
    {
        currentVelocityFrameStep = 0;

        if (velocityFrames != null && velocityFrames.Length > 0)
        {
            velocityFrames = new Vector3[velocityFrames.Length];
            angularVelocityFrames = new Vector3[velocityFrames.Length];
        }
    }
}
