using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoinScrollEventTrigger : UIEventTrigger
{
    [SerializeField] ScrollRect parentScrollRect;
    [SerializeField] TMP_InputField inputField; 
    Vector2 pressPosition;
    bool forwarding;

    void Awake()
    {
        if (inputField == null) inputField = GetComponent<TMP_InputField>();
        init();
    }

    public void Initialize(ScrollRect scrollRect)
    {
        if (scrollRect == null)
        {
            Debug.LogError("JoinScrollEventTrigger에 ScrollRect가 전달되지 않았습니다.", this);
            return;
        }

        parentScrollRect = scrollRect;
        if (inputField == null) inputField = GetComponent<TMP_InputField>();
        init();
    }

    protected override void OnInitializePotentialDrag(PointerEventData data)
    {
        pressPosition = data.position;
        forwarding = false;
        parentScrollRect?.OnInitializePotentialDrag(data);
    }

    protected override void OnPointerDown(PointerEventData data)
    {
        pressPosition = data.position;
        forwarding = false;
    }

    protected override void OnBeginDrag(PointerEventData data)
    {
        TryBeginVerticalForwarding(data);
    }

    protected override void OnDrag(PointerEventData data)
    {
        if (!forwarding) TryBeginVerticalForwarding(data);
        if (!forwarding) return;
        data.eligibleForClick = false;
        parentScrollRect.OnDrag(data);
    }

    protected override void OnEndDrag(PointerEventData data)
    {
        if (forwarding && parentScrollRect != null) parentScrollRect.OnEndDrag(data);
        forwarding = false;
    }

    protected override void OnPointerUp(PointerEventData data)
    {
        if (forwarding) data.eligibleForClick = false;
    }

    protected override void OnScroll(PointerEventData data)
    {
        parentScrollRect?.OnScroll(data);
    }

    void TryBeginVerticalForwarding(PointerEventData data)
    {
        if (parentScrollRect == null) return;
        Vector2 delta = data.position - pressPosition;
        int dragThreshold = EventSystem.current != null ? EventSystem.current.pixelDragThreshold : 5;
        if (delta.magnitude < dragThreshold || Mathf.Abs(delta.y) <= Mathf.Abs(delta.x)) return;
        forwarding = true;
        data.eligibleForClick = false;
        if (inputField != null) inputField.DeactivateInputField(false);
        parentScrollRect.OnBeginDrag(data);
    }
}
