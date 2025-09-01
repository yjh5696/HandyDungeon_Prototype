using System.Collections.Generic;
using UnityEngine;

public class CardInventoryUI : MonoBehaviour
{
    [Header("Scroll View Content")]
    public Transform contentParent;       // Content 오브젝트 (Vertical/Grid Layout Group 부착)
    [Header("Card Slot Prefab")]
    public GameObject cardItemPrefab;     // 카드 UI 프리팹 (이름, Tier, 속성 등 표시)

    public List<CardDataSO> PlayerCardList => PlayerManager.Instance.Player.Cards;

    public void OnEnable()
    {
        Refresh(PlayerCardList);
    }

    // 인벤토리 새로고침(카드 리스트 갱신)
    public void Refresh(List<CardDataSO> cardList)
    {
        // 1. 기존 카드 슬롯 모두 제거
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 2. 카드 개수만큼 프리팹 생성 및 세팅
        foreach (var card in cardList)
        {
            var go = Instantiate(cardItemPrefab, contentParent.transform, false);
            var itemUI = go.GetComponent<CardInventoryItemUI>();
            if (itemUI != null)
                if(card.C_Type == "Action")
                    itemUI.Set(card.C_Name, card.Tier, card.Debuff_Type);
                else if(card.C_Type == "Support")
                    itemUI.Set(card.C_Name, card.Tier, card.Buff_Type);
                else if(card.C_Type == "Special")
                    itemUI.Set(card.C_Name, card.Tier, "Special");
        }
    }
}

