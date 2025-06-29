using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 20;
    public float knockbackForce = 5f;
    public AudioClip trapSound; // Declare a public AudioClip variable
    private AudioSource audioSource; // Declare a private AudioSource variable

    void Start()
    {
        // Get the AudioSource component on Awake
        audioSource = GetComponent<AudioSource>(); 
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on Trap GameObject!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            player.TakeDamage(damage, knockbackDir);

            // Play the sound effect when the trap is triggered
            if (audioSource != null && trapSound != null)
            {
                audioSource.PlayOneShot(trapSound);
            }
        }
    }
}