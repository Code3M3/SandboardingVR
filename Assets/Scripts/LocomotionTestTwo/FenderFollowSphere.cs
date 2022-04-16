using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenderFollowSphere : MonoBehaviour
{
    [SerializeField] public ArticulationBody sphere;
    [SerializeField] public float yOffset;

    private void FixedUpdate()
    {
        transform.position = ( sphere.transform.position + sphere.velocity * Time.deltaTime ) + new Vector3 (0, yOffset, 0);
    }

}
