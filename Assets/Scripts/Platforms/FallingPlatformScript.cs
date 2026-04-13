using System.Collections;
using UnityEngine;

public class FallingPlatformScript : MonoBehaviour
{
    public float fallWait = 0.5f;
    public float fallDuration = 1f; 
    public float respawnTime = 5f; 

    private bool isFalling;
    private Rigidbody2D rb;
    private Collider2D col; 
    private SpriteRenderer sr; 
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        rb.bodyType = RigidbodyType2D.Static; 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isFalling && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FallAndRespawn());
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.fallingPlatform, 0.5f);
            }
        }
    }
    // instead of destroying the obejct we can just disable and re-enable it
    private IEnumerator FallAndRespawn()
    {
        isFalling = true;
        
        yield return new WaitForSeconds(fallWait);
        
        // makes the object free fall
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        yield return new WaitForSeconds(fallDuration);
        
        rb.linearVelocity = Vector2.zero; 
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static; // set back to static
        
        if (col != null) col.enabled = false;
        if (sr != null) sr.enabled = false;
        
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        yield return new WaitForSeconds(respawnTime);
        if (col != null) col.enabled = true;
        if (sr != null) sr.enabled = true;
        isFalling = false;
    }
}