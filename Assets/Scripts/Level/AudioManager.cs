using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Efectos de Sonido")]
    public AudioClip shootSFX;
    public AudioClip playerHitSFX;
    public AudioClip enemyHitSFX;
    public AudioClip rewindSFX;
    public AudioClip glitchSFX;
    [Header("Música")]
    public AudioClip backgroundMusic;

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource rewindSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }
        AudioSource[] sources = GetComponents<AudioSource>();
        sfxSource = sources[0];
        musicSource = sources[1];
        rewindSource = sources[2];

        SetupMusic();
    }

    void SetupMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 0.2f;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = 1.0f;
    }

    public void PlayRewindLoop(bool play)
    {
        if (play)
        {
            if (!rewindSource.isPlaying)
            {
                rewindSource.clip = rewindSFX;
                rewindSource.loop = true;
                rewindSource.Play();
            }
        }
        else { rewindSource.Stop(); }
    }
    public void PlayButtonClick()
    {
        if (glitchSFX != null) sfxSource.PlayOneShot(glitchSFX);
    }
}