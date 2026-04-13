using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    [Header("Parallax Speeds (0=static, 1=moves with camera)")]
    public float parallaxEffectX = 0.5f;
    public float parallaxEffectY = 0.3f; // Usually slower than X for platformers

    [Header("Looping")]
    public bool loopHorizontally = true;
    public bool loopVertically = false; // Set true + add vertical tiles only for infinite vertical scroll

    private float startPosX, startPosY;
    private float length, height;
    private Transform camTransform;
    private Vector3 initialCamPos;

    void Start()
    {
        camTransform = Camera.main.transform;
        initialCamPos = camTransform.position;
        startPosX = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void LateUpdate()
    {
        float relCamX = camTransform.position.x - initialCamPos.x;
        float relCamY = camTransform.position.y - initialCamPos.y;
        
        float distX = relCamX * parallaxEffectX;
        transform.position = new Vector3(startPosX + distX, transform.position.y, transform.position.z);
        if (loopHorizontally)
        {
            float tempX = (camTransform.position.x * (1 - parallaxEffectX));
            if (tempX > startPosX + length) { startPosX += length; }
            else if (tempX < startPosX - length) { startPosX -= length; }
        }

        // Vertical parallax (+ looping if enabled)
        float distY = relCamY * parallaxEffectY;
        transform.position = new Vector3(transform.position.x, startPosY + distY, transform.position.z);
        if (loopVertically)
        {
            float tempY = (camTransform.position.y * (1 - parallaxEffectY));
            if (tempY > startPosY + height) { startPosY += height; }
            else if (tempY < startPosY - height) { startPosY -= height; }
        }
    }
}