using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSFX : MonoBehaviour
{
    [SerializeField] public AudioSource hitSound;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("sword collided with something");
        hitSound.Play();

        Debug.DrawLine(collision.GetContact(0).point, collision.GetContact(0).normal.normalized, Color.green, 5f);
    }
}
