using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{

    GameManager gameManager;
    DifficultyManager difficultyManager;
    private int Score;
    
   
    float BallLifeSpan = 5f;
    float SurvivedTime;
    float BestTime;
    GameObject NewBestWindow;




    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        difficultyManager = FindAnyObjectByType<DifficultyManager>();
        NewBestWindow = gameManager.BestTime;
        Score = gameManager.score;
        StartCoroutine(BallLifeSpanCoroutine());


        // high score
        SurvivedTime = GameManager.Instance.survivalTime;
        PlayerPrefs.SetFloat("SurvivalTime", SurvivedTime);

        SurvivedTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        BestTime = PlayerPrefs.GetFloat("BestTime", 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 hitPoint = collision.GetContact(0).point;
        if (gameManager != null)
        {
            gameManager.BallBounceEffect(gameObject.GetComponent<SpriteRenderer>().color, hitPoint);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.BallBounceSFX();
        }

    }

    private void OnMouseDown()
    {
        gameManager.BallLastPos = transform.position;
        DestroyTheBall();
    }

    public void DestroyTheBall()
    {
        LevelSelect();
        gameManager.ScoreSystem();
        Color BallColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameManager.destroyEffect(BallColor);
        AudioManager.Instance.BallDestroySFX();
        Destroy(gameObject);
    }

    private void LevelSelect()
    {
        if (Score <= 10)
        {
            difficultyManager.LevelOne();
            
        }

        else if (Score > 10 && Score <= 20)
        {
            difficultyManager.LevelTwo(); // adding swing
        }

        else if (Score > 20 && Score <= 100)
        {
            difficultyManager.LevelThree(); //Ball Speed incrase
        }
        else if (Score > 100 && !difficultyManager.is2BallSpawned) //spawn 2 ball
        {
            for (int i = 0; i < 2; i++)
            {
                difficultyManager.LevelFour();
            }
            difficultyManager.HeartBall();
            difficultyManager.is2BallSpawned = true;
        }

        else if (Score > 100 && Score <= 300 && difficultyManager.is2BallSpawned)
        {
            difficultyManager.LevelFour(); //continuing 2 ball
        }

        else if (Score > 300 && !difficultyManager.is3BallSpawned) // spawn 3 ball
        {
            for (int i = 0; i < 2; i++)
            {
                difficultyManager.LevelFive();
   
            }

            for (int j = 0; j < 2; j++)
            {
                difficultyManager.HeartBall();
            }
            difficultyManager.is3BallSpawned = true;
        }


        else if (Score > 300 && Score < 450 && difficultyManager.is3BallSpawned)
        {
            difficultyManager.LevelFive();// continuing 3 ball
        }

        else if (Score >= 450 && Score <= 550) // Ball speed increase
        {
            difficultyManager.LevelSix();
        }

        else if (Score > 550 && !difficultyManager.is4BallSpawned) // 4 ball spawn
        {
            for (int i = 0; i < 3; i++)
            {
                difficultyManager.LevelSix();  
            }
            for (int j = 0; j < 2; j++)
            {
                difficultyManager.HeartBall();
            }
            difficultyManager.is4BallSpawned = true;
        }

        else if (Score > 550 && Score < 700 && difficultyManager.is4BallSpawned)
        {
            difficultyManager.LevelSeven(); // continuing 4 ball
        }

        else if (Score > 700 && Score < 1000)
        {
            difficultyManager.LevelEight(); // Ball speed increase
        }

        else if (Score > 1000 && Score< 2000 && !difficultyManager.is5BallSpawned) // 5 ball spawn
        {
            for (int i = 0; i < 4; i++)
            {
                difficultyManager.LevelNine();
    
            }
            for (int j = 0; j < 3; j++)
            {
                difficultyManager.HeartBall();
            }
            difficultyManager.is5BallSpawned = true;
        }

        else if (Score > 1000 && Score < 2000 && difficultyManager.is4BallSpawned)
        {
            difficultyManager.LevelNine(); // continuing 5 ball
        }
    }


    IEnumerator BallLifeSpanCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < BallLifeSpan)
        {
            float t = elapsed / BallLifeSpan;

            // Smooth transition from white → dark red
            gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.darkRed, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        AudioManager.Instance.BallDestroySFX();
        Destroy(gameObject);
        
        if (gameManager.Health > 0)
        {
            gameManager.HealthSystem();

            // If health reached zero after decrement, trigger game over immediately
            if (gameManager.Health <= 0)
            {
                if (SurvivedTime >= BestTime)
                {
                    PlayerPrefs.SetFloat("BestTime", SurvivedTime);

                    if (NewBestWindow != null)
                        NewBestWindow.SetActive(true);
                }
                else
                {
                    gameManager.GameOver();
                }
            }
        }
        else
        {
            // If health was already zero or below, ensure game over runs
            gameManager.Revive();
        }
        LevelSelect();
    }

  

}


