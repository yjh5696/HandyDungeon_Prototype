using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInventoryItemUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text tierText;
    public TMP_Text elementText;       // 또는 Image elementIcon 등

    // 카드 정보 셋팅
    public void Set(string name, string tier, string element)
    {
        nameText.text = name;
        tierText.text = tier;
        elementText.text = element;
    }
}
