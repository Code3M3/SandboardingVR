using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class update : MonoBehaviour
{
    [SerializeField] public GameObject physicsParent;

    // Start is called before the first frame update
    private void LateUpdate()
    {
        transform.position = physicsParent.transform.position;
    }
}
