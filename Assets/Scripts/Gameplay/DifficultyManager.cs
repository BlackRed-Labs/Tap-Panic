using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
   
    public  GameManager gameManager;
    private GameObject SpawnedBall;
    Rigidbody2D Ballrb;
    public float BallForce = 10f;
    public float CurrectScaleOfBall = 1.3f;
    public GameObject _BombPreFab;
    public GameObject _2XBallPrefab;
    public GameObject _HeartBall;
    [HideInInspector]
    public bool isBombSpawned = false;
    public int BombDelayTime;
    public bool is2BallSpawned = false;  
    public bool is3BallSpawned = false;
    public bool is4BallSpawned = false;
    public bool is5BallSpawned = false;


    public void BombBallSpawn()
    {
        isBombSpawned=true;
        GameObject Bomb = Instantiate(_BombPreFab, new Vector2(UnityEngine.Random.Range(-1.3f, 1.3f), UnityEngine.Random.Range(0f, 3f)), Quaternion.identity);
        Bomb.transform.localScale = Vector3.one *CurrectScaleOfBall;
        Rigidbody2D BombRB = Bomb.GetComponent<Rigidbody2D>();
        int randomnumber = UnityEngine.Random.Range(-2, 1);

        //Debug.Log(randomnumber);
        if (randomnumber <= 0)
        {
            BombRB.AddForce(new Vector2(-10f, 0), ForceMode2D.Impulse);
        }
        else if (randomnumber > 0)
        {
            BombRB.AddForce(new Vector2(10f, 0), ForceMode2D.Impulse);

        }

        StartCoroutine(Bombelay(BombDelayTime));
    }

    IEnumerator Bombelay(int Delay)
    {
        yield return new WaitForSeconds(Delay);
        isBombSpawned = false;

    }

  

    public void TwoXBall()
    {
        GameObject TwoXBall = Instantiate(_2XBallPrefab, new Vector2(UnityEngine.Random.Range(-1.3f, 1.3f), UnityEngine.Random.Range(0f, 3f)), Quaternion.identity);
        TwoXBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Rigidbody2D BombRB = TwoXBall.GetComponent<Rigidbody2D>();
        int randomnumber = UnityEngine.Random.Range(-2, 1);

        //Debug.Log(randomnumber);
        if (randomnumber <= 0)
        {
            BombRB.AddForce(new Vector2(-10, 0), ForceMode2D.Impulse);
        }
        else if (randomnumber > 0)
        {
            BombRB.AddForce(new Vector2(10, 0), ForceMode2D.Impulse);

        }

    }

    public void HeartBall()
    {
        GameObject HeartBall = Instantiate(_HeartBall, new Vector2(UnityEngine.Random.Range(-1.3f, 1.3f), UnityEngine.Random.Range(0f, 3f)), Quaternion.identity);
        HeartBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Rigidbody2D BombRB = HeartBall.GetComponent<Rigidbody2D>();
        int randomnumber = UnityEngine.Random.Range(-2, 1);

        //Debug.Log(randomnumber);
        if (randomnumber <= 0)
        {
            BombRB.AddForce(new Vector2(-10, 0), ForceMode2D.Impulse);
        }
        else if (randomnumber > 0)
        {
            BombRB.AddForce(new Vector2(10, 0), ForceMode2D.Impulse);

        }

    }

    #region Levels

    public void LevelOne() //Spawn ball
    {
        gameManager.SpawnBall();

    }

    public void LevelTwo() // adding Swing to ball
    {
          
        SpawnedBall = gameManager.SpawnBall();
        
        Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();

        if (gameManager.isBonusScoreActive)
        {
            Ballrb.bodyType = RigidbodyType2D.Kinematic;
        }

        else
        {

            int randomnumber = UnityEngine.Random.Range(-2, 1);
            //Debug.Log(randomnumber);
            if (randomnumber <= 0)
            {
                Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
            }
            else if (randomnumber > 0)
            {
                Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

            }
        }
    }
    
    public void LevelThree() //Ball Speed increase
    {
        CurrectScaleOfBall = CurrectScaleOfBall - 0.02f;
        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall, 1f);
        BallForce += 0.3f;
        BallForce = Mathf.Clamp(BallForce, 10f, 18f);
        SpawnedBall = gameManager.SpawnBall();
        SpawnedBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();

        if (gameManager.isBonusScoreActive)
        {
            Ballrb.bodyType = RigidbodyType2D.Kinematic;
        }

        else
        {

            int randomnumber = UnityEngine.Random.Range(-2, 1);
            //Debug.Log(randomnumber);
            if (randomnumber <= 0)
            {
                Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
            }
            else if (randomnumber > 0)
            {
                Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

            }
        }

    }

    public void LevelFour() //2 ball Spawn
    {
            int Score = gameManager.score;
             if (Score > 140 && Score <300) 
             {
              BallForce += 0.4f;
              BallForce = Mathf.Clamp(BallForce, 10f, 30f);

             }
            
            CurrectScaleOfBall = CurrectScaleOfBall - 0.02f;
            CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall, 0.8f);
            SpawnedBall = gameManager.SpawnBall();
            SpawnedBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
            Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();
            Ballrb.mass = 3;
            Ballrb.linearDamping = 0.5f;
            if (gameManager.isBonusScoreActive)
            {
                Ballrb.bodyType = RigidbodyType2D.Kinematic;
            }

            else
            {

                int randomnumber = UnityEngine.Random.Range(-2, 1);
                //Debug.Log(randomnumber);
                if (randomnumber <= 0)
                {
                    Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
                }
                else if (randomnumber > 0)
                {
                    Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

                }
            }

    }

    public void LevelFive() // 3 Ball spawn
    {
        int Score = gameManager.score;
        if (Score > 300 && Score < 450)
        {
            BallForce += 0.6f;
            BallForce = Mathf.Clamp(BallForce, 10f, 40f);

        }

        CurrectScaleOfBall = CurrectScaleOfBall - 0.01f;
        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall, 0.7f);
        SpawnedBall = gameManager.SpawnBall();
        SpawnedBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();
        Ballrb.mass = 3.2f;
        Ballrb.linearDamping = 0.6f;
        if (gameManager.isBonusScoreActive)
        {
            Ballrb.bodyType = RigidbodyType2D.Kinematic;
        }

        else
        {

            int randomnumber = UnityEngine.Random.Range(-2, 1);
            //Debug.Log(randomnumber);
            if (randomnumber <= 0)
            {
                Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
            }
            else if (randomnumber > 0)
            {
                Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

            }
        }

    }

    public void LevelSix() // Ball Speed increase
    {
        int Score = gameManager.score;
        if (Score > 450 && Score < 550)
        {
            BallForce += 0.6f;
            BallForce = Mathf.Clamp(BallForce, 10f, 50f);

        }

        CurrectScaleOfBall = CurrectScaleOfBall - 0.01f;
        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall, 0.65f);
        SpawnedBall = gameManager.SpawnBall();
        SpawnedBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();
        Ballrb.mass = 3.2f;
        Ballrb.linearDamping = 0.6f;
        if (gameManager.isBonusScoreActive)
        {
            Ballrb.bodyType = RigidbodyType2D.Kinematic;
        }

        else
        {

            int randomnumber = UnityEngine.Random.Range(-2, 1);
            //Debug.Log(randomnumber);
            if (randomnumber <= 0)
            {
                Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
            }
            else if (randomnumber > 0)
            {
                Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

            }
        }

    }

    public void LevelSeven() // 4 ball spawn
    {
        int Score = gameManager.score;
        if (Score > 550 && Score < 700)
        {
            BallForce += 0.6f;
            BallForce = Mathf.Clamp(BallForce, 10f, 55f);

        }

        CurrectScaleOfBall = CurrectScaleOfBall - 0.01f;
        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall, 0.6f);
        SpawnedBall = gameManager.SpawnBall();
        SpawnedBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();
        Ballrb.mass = 3.8f;
        Ballrb.linearDamping = 0.8f;
        if (gameManager.isBonusScoreActive)
        {
            Ballrb.bodyType = RigidbodyType2D.Kinematic;
        }

        else
        {

            int randomnumber = UnityEngine.Random.Range(-2, 1);
            //Debug.Log(randomnumber);
            if (randomnumber <= 0)
            {
                Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
            }
            else if (randomnumber > 0)
            {
                Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

            }
        }

    }

    public void LevelEight() // ball Speed increase
    {
        int Score = gameManager.score;
        if (Score > 700 && Score < 1000)
        {
            BallForce += 0.6f;
            BallForce = Mathf.Clamp(BallForce, 10f, 60f);

        }

        CurrectScaleOfBall = CurrectScaleOfBall - 0.01f;
        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall, 0.6f);
        SpawnedBall = gameManager.SpawnBall();
        SpawnedBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();
        Ballrb.mass = 3.8f;
        Ballrb.linearDamping = 0.8f;
        if (gameManager.isBonusScoreActive)
        {
            Ballrb.bodyType = RigidbodyType2D.Kinematic;
        }

        else
        {

            int randomnumber = UnityEngine.Random.Range(-2, 1);
            //Debug.Log(randomnumber);
            if (randomnumber <= 0)
            {
                Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
            }
            else if (randomnumber > 0)
            {
                Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

            }
        }

    }

    public void LevelNine() // 5 ball spawn
    {
        int Score = gameManager.score;
        if (Score > 1000 && Score < 2000)
        {
            BallForce += 0.8f;
            BallForce = Mathf.Clamp(BallForce, 10f, 65f);

        }

        CurrectScaleOfBall = CurrectScaleOfBall - 0.01f;
        CurrectScaleOfBall = Mathf.Max(CurrectScaleOfBall, 0.5f);
        SpawnedBall = gameManager.SpawnBall();
        SpawnedBall.transform.localScale = Vector3.one * CurrectScaleOfBall;
        Ballrb = SpawnedBall.GetComponent<Rigidbody2D>();
        Ballrb.mass = 4.2f;
        Ballrb.linearDamping = 1f;
        if (gameManager.isBonusScoreActive)
        {
            Ballrb.bodyType = RigidbodyType2D.Kinematic;
        }

        else
        {

            int randomnumber = UnityEngine.Random.Range(-2, 1);
            //Debug.Log(randomnumber);
            if (randomnumber <= 0)
            {
                Ballrb.AddForce(new Vector2(-BallForce, 0), ForceMode2D.Impulse);
            }
            else if (randomnumber > 0)
            {
                Ballrb.AddForce(new Vector2(BallForce, 0), ForceMode2D.Impulse);

            }
        }

    }


    #endregion






}
