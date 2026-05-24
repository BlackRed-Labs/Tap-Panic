using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } = null!;

    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource ballBounceSFX;
    [SerializeField] private AudioSource ballDestroySFX;
    [SerializeField] private AudioSource healthCollectedSFX;

    private void Awake()
    {
        // Singleton: keep first instance, destroy duplicates
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM()
    {
        if (bgm != null && !bgm.isPlaying)
        {
            bgm.Play();
        }
    }

    public void StopBGM()
    {
        if (bgm != null && bgm.isPlaying)
        {
            bgm.Stop();
        }
    }

    public void PauseBGM()
    {
        if (bgm != null && bgm.isPlaying)
        {
            bgm.Pause();
        }
    }

    public void UnpauseBGM()
    {
        if (bgm != null && !bgm.isPlaying)
        {
            bgm.UnPause();
        }
    }

    public void BallBounceSFX()
    {
        ballBounceSFX?.PlayOneShot(ballBounceSFX.clip);
    }

    public void BallDestroySFX()
    {
        ballDestroySFX?.PlayOneShot(ballDestroySFX.clip);
    }

    public void PlayHealthCollected() 
    {
        PauseBGM();
        healthCollectedSFX.Play();
        AddDelay.instance.AddDelayToAction(healthCollectedSFX.clip.length); // Wait for the health collected sound to finish
        PlayBGM();

    }

    /// <summary>
    /// Play any clip on a specified AudioSource (safe wrapper).
    /// </summary>
    public void PlaySFX(AudioSource source, AudioClip clip)
    {
        if (source != null && clip != null)
        {
            source.PlayOneShot(clip);
        }
    }
}
