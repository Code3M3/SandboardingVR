using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenderFixSphere : MonoBehaviour
{
    [SerializeField] public ArticulationBody sphere;
    [SerializeField] public float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position = (sphere.transform.position) + new Vector3(0, yOffset, 0);
    }
}
