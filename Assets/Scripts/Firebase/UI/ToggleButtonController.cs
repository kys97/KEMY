using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonController : MonoBehaviour
{
    public GameObject uiElement; // 토글할 UI 요소를 Inspector에서 설정

    // 토글 버튼 클릭 이벤트 핸들러
    public void ToggleButtonClick()
    {
        // UI 요소의 활성/비활성 상태를 토글
        uiElement.SetActive(!uiElement.activeSelf);
    }
}
