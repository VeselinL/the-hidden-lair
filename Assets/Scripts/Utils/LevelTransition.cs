using System;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && CoinManager.Instance.coinCount == 7)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
        }
    }
}
