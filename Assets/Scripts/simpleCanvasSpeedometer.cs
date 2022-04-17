using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class simpleCanvasSpeedometer : MonoBehaviour
{
    public Text speedText;
    [SerializeField] Rigidbody playerRB;
    [SerializeField] InputActionReference leftControllerVelocity;

    String originalText;

    // Start is called before the first frame update
    void Start()
    {
        speedText = GetComponent<Text>();
        speedText.text += "\n \n";

        originalText = speedText.text;
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = originalText + playerRB.velocity.magnitude.ToString("F0") + "\n" + leftControllerVelocity.action.ReadValue<Vector3>();
    }
}
