using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource ballBounceSFX;
    [SerializeField] private AudioSource ballDestroySFX;

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

    public void BallBounceSFX()
    {
        ballBounceSFX?.PlayOneShot(ballBounceSFX.clip);
    }

    public void BallDestroySFX()
    {
        ballDestroySFX?.PlayOneShot(ballDestroySFX.clip);
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
