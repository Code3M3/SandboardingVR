using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomExecutionOrder : MonoBehaviour
{
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] public update updateCam;
    [SerializeField] public BalanceCamOffset recenterCam;

    private void FixedUpdate()
    {
        playerMovement.PlayerMove();
    }

    private void Update()
    {
        updateCam.UpdateCamRotation();
        recenterCam.BalanceCamOrigin();
    }
    private void LateUpdate()
    {
        updateCam.UpdateCamPos();
    }
}
