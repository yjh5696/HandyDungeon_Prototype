using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryItemUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text tierText;
    public TMP_Text elementText;
    public Image elementIcon;

    // 카드 정보 셋팅
    public void Set(string name, string rare, string element, string hexColor)
    {
        nameText.text = name;
        tierText.text = "레어도: " + rare;
        elementText.text = "속성: " + element;
        Color color;
        if (UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            elementIcon.color = color;
        }
    }
}
