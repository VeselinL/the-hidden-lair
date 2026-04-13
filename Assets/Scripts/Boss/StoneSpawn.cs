using Unity.Cinemachine;
using UnityEngine;

public class StoneSpawn : MonoBehaviour
{
    [Header("Stone Prefab & Spawn Area")]
    public GameObject stonePrefab;
    public Transform spawnPointLeft; 
    public Transform spawnPointRight; 

    [Header("Stone Settings")] 
    public float normalFallSpeed = 10f;
    public float enragedFallSpeed = 20f; 
    private float currentFallSpeed; 

    public CinemachineImpulseSource impulseSource;
    
    private void Start()
    {
        currentFallSpeed = normalFallSpeed;
    }
    
    public void SetEnraged(bool on)
    {
        currentFallSpeed = on ? enragedFallSpeed : normalFallSpeed;
    }
    
    public void SpawnStones(int count)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.fallingPlatform);
        }
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(spawnPointLeft.position.x, spawnPointRight.position.x),
                spawnPointLeft.position.y
            );

            GameObject stone = Instantiate(stonePrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.down * currentFallSpeed; 
            }
        }
    }
}