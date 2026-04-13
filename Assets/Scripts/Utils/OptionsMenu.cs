using UnityEngine;
using UnityEngine.Audio;
public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mixer;
    void SetVolume(float volume)
    {
        mixer.SetFloat("volume", volume);
    }
}
