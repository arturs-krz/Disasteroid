using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mouseH : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Image;
    public GameObject selfImage;
    public Sprite styleOn;
    public Sprite styleOff;
    public Sprite SelfstyleOn;
    public Sprite SelfstyleOff;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Image.gameObject.transform.GetComponent<Image>().sprite = styleOn; //Or however you do your color
        selfImage.gameObject.transform.GetComponent<Image>().sprite = SelfstyleOn;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("onmouse");
        //    Image.gameObject.transform.GetComponent<Image>().sprite = styleOn;
        //    selfImage.gameObject.transform.GetComponent<Image>().sprite = SelfstyleOn;
        //}
        if (Input.GetMouseButtonUp(0))
        {
            //rotSpeed = 0;
            Image.gameObject.transform.GetComponent<Image>().sprite = styleOff;
            selfImage.gameObject.transform.GetComponent<Image>().sprite = SelfstyleOff;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Image.gameObject.transform.GetComponent<Image>().sprite = styleOff; //Or however you do your color
        selfImage.gameObject.transform.GetComponent<Image>().sprite = SelfstyleOff;
    }
}
