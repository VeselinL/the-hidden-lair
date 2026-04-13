using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for coroutines

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip level1Music; 
    public AudioClip level2Music;
    public AudioClip bossLevelMusic;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip playerAttack;
    public AudioClip playerGrunt;
    public AudioClip coinPickup;
    public AudioClip playerDeath;
    public AudioClip jump;
    public AudioClip hurt;
    public AudioClip dash;
    public AudioClip enemyAttack;
    public AudioClip enemyDeath;
    public AudioClip fallingPlatform;
    public AudioClip clothesSound;
    public AudioClip stompSound;
    public AudioClip respawn;
    public AudioClip caveEntranceSFX;
    public AudioClip keyPickupSound;
    public AudioClip doorUnlockSound;
    public AudioClip dizzySound;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = 0.5f;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = 0.7f; 
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetMusicPitch();
        AudioClip nextClip = null;
        string sceneName = scene.name;
        if (sceneName == "Level 1") 
        {
            nextClip = level1Music;
        }
        else if (sceneName == "Level 2") 
        {
            nextClip = level2Music;
        }
        else if (sceneName == "Boss Level")
        {
            nextClip = bossLevelMusic;
        }
    
        if (nextClip != null)
        {
            bool shouldPlayMusic = (musicSource.clip != nextClip || !musicSource.isPlaying);

            if (shouldPlayMusic) 
            {
                if (sceneName == "Level 2" && caveEntranceSFX != null)
                {
                    StopMusic();
                    StartCoroutine(HandleMusicTransition(nextClip));
                }
                else
                {
                    PlayMusic(nextClip);
                }
            }
        }
        else if (sceneName == "MainMenu")
        {
            StopMusic();
        }
    }
    private IEnumerator HandleMusicTransition(AudioClip nextClip)
    {
        PlaySFX(caveEntranceSFX);
        
        yield return new WaitForSeconds(caveEntranceSFX.length - 3);
        PlayMusic(nextClip);
    }
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    public void PlaySFX(AudioClip clip, float volume = 0.7f)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }
    public void SetMusicPitch(float pitch)
    {
        if (musicSource != null)
        {
            musicSource.pitch = pitch;
        }
    }
    // used aftter boss level
    public void ResetMusicPitch()
    {
        if (musicSource != null)
        {
            musicSource.pitch = 1.0f;
        }
    }
    // used when game is paused
    public void MuteAllAudio(bool isMuted)
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }
        if (sfxSource != null)
        {
            sfxSource.mute = isMuted;
        }
    }

}