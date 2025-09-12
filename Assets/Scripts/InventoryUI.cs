using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Player Trait")]
    [SerializeField] private Character player;        // 플레이어 캐릭터 (Inspector 연결)
    [SerializeField] private TMP_Text mytrait;      // TraitText UI (Inspector 연결)
    [SerializeField] private TMP_Text traitText;    // TraitText UI (Inspector 연결)

    [Header("Card Inventory Panel")]
    [SerializeField] private GameObject cardInvenPanel;  // CardInven 스크롤뷰 Panel (Inspector 연결)

    // 플레이어 특성 출력
    public void ShowPlayerTrait()
    {
        if (player == null || mytrait == null)
        {
            Debug.LogWarning("Player 또는 TraitText가 연결되지 않았습니다.");
            return;
        }
        string PlayerTrait = PlayerManager.Instance.GetTraitPlayer().ToString();
        Debug.Log($"플레이어 특성: {PlayerTrait}");
        mytrait.text = $"{PlayerTrait}";
        traitText.text = $"{GettraitText(PlayerTrait)}";
    }

    public string GettraitText(string playerTrait)
    {
        return playerTrait switch
        {
            "Diceby20" => "주사위 눈이 20까지 늘어납니다. 다만 홀수가 나올 경우 1로 변합니다.",
            "Diceby10" => "주사위 눈이 10까지 늘어납니다. 다만 2, 3이 나올 경우 1로 변합니다.",
            "AddOne" => "주사위 눈이 결정된 후 1을 더합니다.",
            _ => "알 수 없는 특성"
        };
    }

    // 카드 인벤토리 패널 활성화
    public void ShowCardInventory()
    {
        if (cardInvenPanel != null)
            cardInvenPanel.SetActive(true);
    }

    // 카드 인벤토리 패널 비활성화
    public void HideCardInventory()
    {
        if (cardInvenPanel != null)
            cardInvenPanel.SetActive(false);
    }

    // 인벤토리 오픈 (Trait + 카드 인벤토리)
    public void OpenInventory()
    {
        ShowPlayerTrait();
        ShowCardInventory();
    }

    // 인벤토리 닫기
    public void CloseInventory()
    {
        HideCardInventory();
    }
}

