using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MovementInputField : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem.current.currentSelectedGameObject
                ?.GetComponent<Selectable>()
                .FindSelectableOnDown()
                ?.Select();
        }
    }
}
