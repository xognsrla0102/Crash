using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySND(SSfxName.BUTTON_OVER_SFX);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySND(SSfxName.BUTTON_CLICK_SFX);
    }
}
