using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum MessageState
{
    MovingIn,
    Showing,
    MovingOut,
}

public class UIMessage : MonoBehaviour
{
    private static UIMessage _instance;

    private GameObject snackBar;
    private RectTransform snackBarTransform;
    private Text textContainer;

    private bool showingMessage = false;
    private MessageState state;
    private float currentMessageDuration;
    private float messageTimer;

    private float visiblePosition;
    private float hiddenPosition;

    private static float EASING_TIME = 0.5f;
    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        snackBar = transform.Find("SnackBar").gameObject;
        textContainer = snackBar.transform.GetChild(0).gameObject.GetComponent<Text>();

        snackBarTransform = snackBar.GetComponent<RectTransform>();
        visiblePosition = snackBarTransform.anchoredPosition.y;
        hiddenPosition = visiblePosition - 70f;
    }

    public static void ShowMessage(string message, float duration = 3f)
    {
        _instance.textContainer.text = message;
        _instance.snackBarTransform.anchoredPosition = new Vector2(_instance.snackBarTransform.anchoredPosition.x, _instance.hiddenPosition);

        _instance.currentMessageDuration = duration;
        _instance.messageTimer = 0f;
        _instance.showingMessage = true;
        _instance.state = MessageState.MovingIn;

        _instance.snackBar.SetActive(true);
    }    

    // Update is called once per frame
    void Update()
    {
        if (showingMessage)
        {
            messageTimer += Time.deltaTime;

            switch (state)
            {
                case MessageState.MovingIn:
                {
                    float yPos = Mathf.Lerp(hiddenPosition, visiblePosition, messageTimer / EASING_TIME);
                    snackBarTransform.anchoredPosition = new Vector2(snackBarTransform.anchoredPosition.x, yPos);

                    if (messageTimer >= EASING_TIME)
                    {
                        snackBarTransform.anchoredPosition = new Vector2(snackBarTransform.anchoredPosition.x, visiblePosition);
                        state = MessageState.Showing;
                    }
                    break;
                }
                case MessageState.Showing:
                {
                    if (messageTimer >= EASING_TIME + currentMessageDuration)
                    {
                        state = MessageState.MovingOut;
                    }
                    break;
                }
                case MessageState.MovingOut:
                {
                    float yPos = Mathf.Lerp(visiblePosition, hiddenPosition, (messageTimer - currentMessageDuration - EASING_TIME) / EASING_TIME);
                    snackBarTransform.anchoredPosition = new Vector2(snackBarTransform.anchoredPosition.x, yPos);

                    if (messageTimer >= (2 * EASING_TIME) + currentMessageDuration)
                    {
                        snackBarTransform.anchoredPosition = new Vector2(snackBarTransform.anchoredPosition.x, hiddenPosition);
                        showingMessage = false;
                        snackBar.SetActive(false);
                    }
                    break;
                }
            }
        }
    }
}
