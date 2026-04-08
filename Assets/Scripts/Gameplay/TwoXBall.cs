using System.Collections;
using UnityEngine;

public class TwoXBall : MonoBehaviour
{
     private GameManager GameManager;
    

    private void Start()
    {
        GameManager = FindAnyObjectByType<GameManager>();
        StartCoroutine(BallDestry());
    }
    private void OnMouseDown()
    {
        GameManager.DoubleScore();
        Destroy(gameObject);
    }

    private IEnumerator BallDestry()
    {
        if (gameObject != null && gameObject.activeSelf)
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }
      
        
    }
}
