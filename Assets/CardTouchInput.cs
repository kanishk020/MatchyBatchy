using UnityEngine;
using UnityEngine.EventSystems;

public class CardTouchInput : MonoBehaviour, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

}

