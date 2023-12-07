using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonController : MonoBehaviour
{
    public GameObject uiElement; // ����� UI ��Ҹ� Inspector���� ����

    // ��� ��ư Ŭ�� �̺�Ʈ �ڵ鷯
    public void ToggleButtonClick()
    {
        // UI ����� Ȱ��/��Ȱ�� ���¸� ���
        uiElement.SetActive(!uiElement.activeSelf);
    }
}
