using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomExecutionOrder : MonoBehaviour
{
    [SerializeField] public PlayerMovement playerMovement;
    [SerializeField] public update updateCam;

    private void FixedUpdate()
    {
        playerMovement.PlayerMove();
    }

    private void LateUpdate()
    {
        updateCam.UpdateCamPos();
    }
}
