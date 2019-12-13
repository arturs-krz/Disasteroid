using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaltyScript : MonoBehaviour
{
    private RectTransform tommyTransform;
    private float startPosition;
    private float endPosition;
    private int direction = 1;
    private float travelTime = 5f;
    private float currentRunTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        tommyTransform = transform.Find("TommyBoi").GetComponent<RectTransform>();
        startPosition = tommyTransform.anchoredPosition.x;
        endPosition = -startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        currentRunTime += Time.deltaTime;

        if (currentRunTime > travelTime)
        {
            currentRunTime = 0f;
            direction = -direction;
        }

        if (direction == 1)
        {
            tommyTransform.anchoredPosition = new Vector2(Mathf.Lerp(startPosition, endPosition, currentRunTime / travelTime), tommyTransform.anchoredPosition.y);
        }
        else
        {
            tommyTransform.anchoredPosition = new Vector2(Mathf.Lerp(endPosition, startPosition, currentRunTime / travelTime), tommyTransform.anchoredPosition.y);
        }
    }

    public void BacktoMain()
    {
        SceneManager.LoadScene("GameStart");
    }
}
