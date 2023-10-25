using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.VisualScripting.Member;
using static UnityEngine.Rendering.DebugUI;

public class JoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Joystick
    [SerializeField] public RectTransform Lever;
    [SerializeField, Range(10f, 150f)] private float LeverRange;
    private RectTransform RectTransform;
    private Vector2 InputDir, ClampedDir;

    
    //Player
    private Move CharacterMove;


    void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        CharacterMove = GameObject.FindWithTag("Player").GetComponent<Move>();

        RectTransform.anchorMin = new Vector2(1, 0);
        RectTransform.anchorMax = new Vector2(1, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //JoyStick
        InputDir = eventData.position + new Vector2(-1080, 0) - RectTransform.anchoredPosition;
        ClampedDir = InputDir.magnitude < LeverRange ? InputDir : InputDir.normalized * LeverRange;
        Lever.anchoredPosition = ClampedDir;

        CharacterMove.IsMove = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //JoyStick
        InputDir = eventData.position + new Vector2(-1080, 0) - RectTransform.anchoredPosition;
        ClampedDir = InputDir.magnitude < LeverRange ? InputDir : InputDir.normalized * LeverRange;
        Lever.anchoredPosition = ClampedDir;

        CharacterMove.SetMove(InputDir.normalized);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //JoyStick
        Lever.anchoredPosition = Vector2.zero;

        CharacterMove.IsMove = false;
    }
}
