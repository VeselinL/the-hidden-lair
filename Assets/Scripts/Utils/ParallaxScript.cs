using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    public Transform camTransform; 
    
    [Header("Parallax Speed")]
    public float parallaxEffectX = 0.5f; 
    public float parallaxEffectY = 0.5f; 
    
    private float initialCameraX;
    private float initialPositionX;
    private float initialCameraY;
    private float initialPositionY; 

    void Start()
    {
        if (camTransform == null)
        {
            camTransform = Camera.main.transform;
        }

        initialCameraX = camTransform.position.x; 
        initialPositionX = transform.position.x;
        
        initialCameraY = camTransform.position.y;
        initialPositionY = transform.position.y;
    }

    void LateUpdate()
    {
        float deltaCameraX = camTransform.position.x - initialCameraX; 
        float parallaxShiftX = deltaCameraX * parallaxEffectX; 
        float newX = initialPositionX + parallaxShiftX;
        
        float deltaCameraY = camTransform.position.y - initialCameraY; 
        float parallaxShiftY = deltaCameraY * parallaxEffectY; 
        float newY = initialPositionY + parallaxShiftY;
        
        transform.position =  new Vector3(newX, newY, transform.position.z);
    }
} 