using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class update : MonoBehaviour
{
    [SerializeField] public GameObject physicsParent;
    [SerializeField] public Vector3 offset;

    // Start is called before the first frame update
    public void UpdateCamPos()
    {
        transform.position = physicsParent.transform.position + offset;
    }

    public void UpdateCamRotation()
    {
        transform.rotation = physicsParent.transform.rotation;
    }
}
