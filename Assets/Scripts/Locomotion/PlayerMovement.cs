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
    [SerializeField] float gravityForce = 10f;
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

    Rigidbody _rb;
    float _cooldownTimer;

    private int currentVelocityFrameStep = 0;

    [Header("STATE")]
    public MovementState state;
    public enum MovementState
    {
        normal,
        surfing
    }

    public bool normal;
    public bool surfing;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        leftControllerMoveRef.action.canceled += ClearVelocities;
    }

    private void ClearVelocities(InputAction.CallbackContext obj)
    {
        ResetVelocityHistory();
    }

    private void StateHandler()
    {
        // Mode - Surfing
        if (surfing)
        {
            state = MovementState.surfing;
        }

        // Mode - Normal
        else if (normal)
        {
            state = MovementState.normal;
        }
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
        if (velocityFrames != null)
        {
            if (velocityAverage != null)
            {
                Vector3 localVelocity = velocityAverage; // total velocity
                localVelocity *= -1; // reverse direction

                //localVelocity = new Vector3(localVelocity.x, 0, localVelocity.z); //zero out jumping

                if (localVelocity.sqrMagnitude > minForce * minForce)
                {
                    // SLIDING MOVEMENT
                    _rb.AddForce(localVelocity * moveForce, ForceMode.Acceleration);

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
