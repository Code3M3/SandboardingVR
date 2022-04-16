using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

// https://gamedev.stackexchange.com/questions/188571/how-to-make-gameobject-copy-position-of-other-gameobject-but-react-to-physics-pe

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject _locoSphere; 

    private Rigidbody _playerRB;

    // Start is called before the first frame update
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Calculate your desired position however you like.
        Vector3 desiredPosition = _locoSphere.transform.position;

        // What velocity gets us there in one timestep?
        Vector3 desiredVelocity = (desiredPosition - transform.position) / Time.deltaTime;

        // However, if the velocity is too high then it can still cause wonky physics, so in my case changing the last line to
        _playerRB.AddForce((desiredVelocity - _playerRB.velocity) / 1000, ForceMode.VelocityChange);
    }
}
