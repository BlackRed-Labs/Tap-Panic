using UnityEngine;

public class MissedTap : MonoBehaviour
{
    public GameManager GameManager;
    [SerializeField] private GameObject MouseclickEffectPrefab;

    private void OnMouseDown()
    {
        if (!gameObject.CompareTag("Tap Missed")) return;

        if (GameManager.Health > 0 && GameManager != null && gameObject.CompareTag("Tap Missed"))
        {

            GameManager.HealthSystem();
            mouseClickEffect();


            // If health reached zero after decrement, trigger Revive immediately
            if (GameManager.Health <= 0)
            {
                GameManager.Revive();
            }
        }
        
    }

    void mouseClickEffect() {
        #region Mouse click effect
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.timeScale > 0)
            {
                SpawnEffect(Input.mousePosition);
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Time.timeScale > 0)
            {
                SpawnEffect(Input.GetTouch(0).position);
            }
        }
    }

    void SpawnEffect(Vector3 screenPos)
    {

        screenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        GameObject effect = Instantiate(MouseclickEffectPrefab, worldPos, Quaternion.identity);
        #endregion
    }



}

