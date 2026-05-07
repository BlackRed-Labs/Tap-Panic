using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource _BGM;
    public AudioSource _BallBounceSFX;
    public AudioSource _BallDestroySFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

        public void PlayBGM()
        {
            if (_BGM != null && !_BGM.isPlaying)
            {
                _BGM.Play();
            }
        }

    public void BallBounceSFX() { 
        if (_BallBounceSFX != null)
        {
            _BallBounceSFX.PlayOneShot(_BallBounceSFX.clip);
        }
    }

    public void BallDestroySFX()
    {
        if (_BallDestroySFX != null)
        {
            _BallDestroySFX.PlayOneShot(_BallDestroySFX.clip);
        }
    }
}
