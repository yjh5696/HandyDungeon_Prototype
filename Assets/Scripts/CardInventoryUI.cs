using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryUI : MonoBehaviour
{
    [Header("Scroll View Content")]
    private ScrollRect scrollRect;
    [Header("Card Slot Prefab")]
    public GameObject cardItemPrefab;     // 카드 UI 프리팹 (이름, Tier, 속성 등 표시)

    public List<CardDataSO> PlayerCardList => PlayerManager.Instance.Player.Cards;
    public List<RectTransform> cardSlots = new List<RectTransform>();

    [SerializeField] GameObject log;
    [SerializeField] GameObject btns;
    [SerializeField] GameObject playerbuff;
    [SerializeField] GameObject enemybuff;
    [SerializeField] GameObject playerdebuff;
    [SerializeField] GameObject enemydebuff;

    public float space = 50;


    public void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void OnEnable()
    {
        Refresh(PlayerCardList);
    }

    // 인벤토리 새로고침(카드 리스트 갱신)
    public void Refresh(List<CardDataSO> cardList)
    {
        // 1. 기존 카드 슬롯 모두 제거
        foreach (Transform child in scrollRect.content)
            Destroy(child.gameObject);
        cardSlots.Clear();

        // 2. 카드 개수만큼 프리팹 생성 및 세팅
        foreach (var card in cardList)
        {
            var go = Instantiate(cardItemPrefab, scrollRect.content).GetComponent<RectTransform>();
            var itemUI = go.GetComponent<CardInventoryItemUI>();
            string colorString = "#FFFFFF";
            if (itemUI != null)
                if (card.C_Type == "Action")
                {
                    colorString = "#A82B3B";
                    itemUI.Set(card.C_Name, card.Rare, card.Debuff_Type, colorString);
                }
                else if(card.C_Type == "Support")
                {
                    colorString = "#41924B";
                    itemUI.Set(card.C_Name, card.Rare, card.Buff_Type, colorString);
                }
                else if(card.C_Type == "Special")
                {
                    colorString = "#DFB54F";
                    itemUI.Set(card.C_Name, card.Rare, "Special", colorString);
                }
            cardSlots.Add(go);

            float y = 0f;
            for(int i = 0; i < cardSlots.Count; i++)
            {
                cardSlots[i].anchoredPosition = new Vector2(0, -y);
                y += (cardSlots[i].sizeDelta.y + space);
            }

            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
        }
    }

    public void Close()
    {
        log.SetActive(false);
        btns.SetActive(false);
        playerbuff.SetActive(false);
        enemybuff.SetActive(false);
        playerdebuff.SetActive(false);
        enemydebuff.SetActive(false);
    }
}

