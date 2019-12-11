using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mouseH : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Image;
    public GameObject selfImage;
    //public Sprite styleOn;
    //public Sprite styleOff;
    //public Sprite SelfstyleOn;
    //public Sprite SelfstyleOff;
    public Color colorOn;
    public Color colorOff;
    public Color selfcolorOn;
    public Color selfcolorOff;
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
        Image.gameObject.transform.GetComponent<Image>().color = colorOn; //Or however you do your color
        selfImage.gameObject.transform.GetComponent<Image>().color = selfcolorOn;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("onmouse");
        //    Image.gameObject.transform.GetComponent<Image>().sprite = styleOn;
        //    selfImage.gameObject.transform.GetComponent<Image>().sprite = SelfstyleOn;
        //}
        if (Input.GetMouseButtonUp(0))
        {
            //rotSpeed = 0;
            Image.gameObject.transform.GetComponent<Image>().color = colorOn;
            selfImage.gameObject.transform.GetComponent<Image>().color = selfcolorOn;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Image.gameObject.transform.GetComponent<Image>().color = colorOff; //Or however you do your color
        selfImage.gameObject.transform.GetComponent<Image>().color = selfcolorOff;
    }
}
