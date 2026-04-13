using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    private bool isOpen;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            if (GameManager.instance.hasKey)
            {
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        Collider2D doorTrigger = GetComponent<Collider2D>();
        if (doorTrigger != null)
            doorTrigger.enabled = false;

        StartCoroutine(OpenDoorSequence());
    }

    private IEnumerator OpenDoorSequence()
    {
        AudioManager.instance?.PlaySFX(AudioManager.instance.doorUnlockSound);

        yield return new WaitForSeconds(0.15f);

        SceneFader fader = FindFirstObjectByType<SceneFader>();
        if (fader != null)
            yield return fader.FadeAndLoad("Boss Level");
        else
            SceneManager.LoadScene("Boss Level");
    }

}
