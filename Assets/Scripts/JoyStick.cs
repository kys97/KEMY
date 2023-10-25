using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.VisualScripting.Member;

public class JoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    public RectTransform lever;
    RectTransform rectTransform;

    [SerializeField, Range(10f, 150f)]
    private float leverRange;

    private Vector2 InputDir, clampedDir;

    void Start()
    {
        //lever = gameObject.GetComponentInChildren<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        //lever.anchorMin = new Vector2(1, 0);
        //lever.anchorMax = new Vector2(1, 0);

        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InputDir = eventData.position + new Vector2(-1080, 0) - rectTransform.anchoredPosition;
        clampedDir = InputDir.magnitude < leverRange ? InputDir : InputDir.normalized * leverRange;

        lever.anchoredPosition = clampedDir;
    }

    public void OnDrag(PointerEventData eventData)
    {
        InputDir = eventData.position + new Vector2(-1080, 0) - rectTransform.anchoredPosition;
        clampedDir = InputDir.magnitude < leverRange ? InputDir : InputDir.normalized * leverRange;

        lever.anchoredPosition = clampedDir;

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        lever.anchoredPosition = Vector2.zero;
    }
}
