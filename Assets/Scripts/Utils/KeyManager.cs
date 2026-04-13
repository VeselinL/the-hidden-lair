using System;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         Destroy(gameObject);
         if (AudioManager.instance != null)
         {
            AudioManager.instance.PlaySFX(AudioManager.instance.keyPickupSound);
         }
         if (GameManager.instance != null)
         {
            GameManager.instance.hasKey = true;
         }
      }
   }
}
