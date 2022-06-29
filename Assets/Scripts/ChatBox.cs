using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatBox : MonoBehaviour
{
    private const float CHAT_DURATION_TIME = 4f;

    [SerializeField] private Text chatText;
    [SerializeField] private bool isUserSlotChatBox;

    private CanvasGroup canvasGroup;
    private Coroutine chatEffectCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (isUserSlotChatBox)
        {
            canvasGroup.alpha = 0f;
        }
    }

    public void SetText(string msg) => chatText.text = msg;

    public void ShowChatEffect(string msg)
    {
        SetText(msg);

        if (chatEffectCoroutine != null)
        {
            StopCoroutine(chatEffectCoroutine);
        }
        chatEffectCoroutine = StartCoroutine(ChatEffectCoroutine());
    }

    private IEnumerator ChatEffectCoroutine()
    {
        // 2초 뒤에 사라지기 시작
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(CHAT_DURATION_TIME);

        while (true)
        {
            if (Mathf.Approximately(canvasGroup.alpha, 0f))
            {
                break;
            }
            canvasGroup.alpha -= Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
