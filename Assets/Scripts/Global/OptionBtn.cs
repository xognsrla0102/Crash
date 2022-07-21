using UnityEngine;
using UnityEngine.UI;

public class OptionBtn : MonoBehaviour
{
    private Button optionBtn;

    private void Start()
    {
        optionBtn = GetComponent<Button>();
        optionBtn.onClick.AddListener(OptionManager.Instance.OnClickOptionBtn);
    }

    private void OnDestroy()
    {
        optionBtn.onClick.RemoveAllListeners();
    }
}
