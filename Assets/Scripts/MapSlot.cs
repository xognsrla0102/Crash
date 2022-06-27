using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private RawImage mapImg;

    public void InitSlot(string mapName)
    {
        mapNameText.text = mapName;
        switch (mapName)
        {
            case SMapName.STADIUM:
                mapImg.texture = Resources.Load<Texture2D>($"{SResourceLoadPath.IMAGE}Stadium");
                break;
            default:
                Debug.Assert(false);
                mapImg = null;
                break;
        }
    }
}
