using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfingMovement : MonoBehaviour
{
    [Header("surfing")]
    public float extraGravity = 5f;

    [Header("friction")]
    public PhysicMaterial fricitionlessMat;
    public PhysicMaterial hiFricitionMat;
    private Collider locomotionCollider;

    [Header("detection")]
    public LayerMask Sand;
    private float VelY;

    [Header("values")]
    public float VelYThresh = 1f;
    public float minGroundDistance = 2f;

    [Header("references")]
    private PlayerMovement _pm;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        locomotionCollider = GetComponent<Collider>();
        _pm = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
        CloseToGround();
    }

    private bool CloseToGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, minGroundDistance, Sand);
    }

    private void StateMachine()
    {
        // get y vel
        VelY =_rb.velocity.y;

        // State 1 - Surfing
        if(CloseToGround() && VelY < -VelYThresh)
        {
            if (!_pm.surfing)
                StartSurf();
        }

        // State 3 - None
        else
        {
            if (_pm.surfing)
                StopSurf();
        }
    }

    private void FixedUpdate()
    {
        if (_pm.surfing)
            SurfMovement();
    }

    private void StartSurf()
    {
        _pm.surfing = true;

        // change friction
        locomotionCollider.material = fricitionlessMat;
    }

    private void SurfMovement()
    {
        // apply downward force
        _rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }

    private void StopSurf()
    {
        _pm.surfing = false;

        // change friction
        locomotionCollider.material = hiFricitionMat;
    }
}
