using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool IsMove;
    public static Vector2 MoveDir;

    //Joystick
    [SerializeField] public RectTransform Lever;
    [SerializeField, Range(0f, 130f)] private float LeverRange;
    private RectTransform rt;

    private Vector2 InputDir, ClampedDir;


    void Start()
    {
        rt = GetComponent<RectTransform>();

        //조이스틱 우측 하단
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsMove = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //JoyStick
        InputDir = eventData.position - rt.anchoredPosition + new Vector2(-1f * Screen.width, 0);
        ClampedDir = InputDir.magnitude < LeverRange ? InputDir : InputDir.normalized * LeverRange;
        Lever.anchoredPosition = ClampedDir;

        MoveDir = ClampedDir.normalized;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //JoyStick
        Lever.anchoredPosition = Vector2.zero;

        IsMove = false;
    }
}
