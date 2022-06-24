using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldUtility : MonoBehaviour
{
    [HideInInspector] public Action EnterAction;

    [HideInInspector] public bool canTab = true;
    [HideInInspector] public bool canEnter = true;

    public void Init(bool canTab, bool canEnter)
    {
        this.canTab = canTab;
        this.canEnter = canEnter;
    }

    private void Update()
    {
        // Tab 키로 현재 인풋필드에서 다음 인풋필드로 넘어가도록 설정
        if (canTab && Input.GetKeyDown(KeyCode.Tab))
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                Selectable nowSelectedObj = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
                if (nowSelectedObj != null)
                {
                    Selectable nextSelectedObj = nowSelectedObj.FindSelectableOnDown();
                    if (nextSelectedObj != null)
                    {
                        nextSelectedObj.Select();
                    }
                }
            }
        }

        // Enter 키로 특정 기능 작동
        if (canEnter && Input.GetKeyDown(KeyCode.Return))
        {
            Popup normalPopup = FindObjectOfType<Popup>();

            // 이미 일반 팝업이 활성화 되어있다면 무시
            if (normalPopup != null && normalPopup.isNormalPopup)
            {
                print($"일반 팝업 [{normalPopup.name}]이 이미 화면에 있습니다.");
                return;
            }

            EnterAction();
        }
    }
}
