using System.Collections;
using UnityEngine;

public class AddDelay : MonoBehaviour
{
    public static AddDelay instance { get; private set; } = null!;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddDelayToAction(float delay)
    {
        StartCoroutine(AddDelayCoroutine(delay));
    }

    IEnumerator AddDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

}
