using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    public CardDataSO selectedCard;

    [SerializeField] private Character playerCharacter;  // 플레이어 캐릭터 참조 (인스펙터 할당)
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private Card cardUI;  // 카드 UI 컴포넌트

    [SerializeField] private TMP_Text actionCardText;
    [SerializeField] private TMP_Text supportCardText;
    [SerializeField] private TMP_Text specialCardText;

    private CardDataSO _currentCard;

    private CardDataSO _actionCard;
    private CardDataSO _supportCard;
    private CardDataSO _specialCard;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (playerCharacter == null || playerCharacter.Cards == null || playerCharacter.Cards.Count == 0)
            Debug.LogWarning("Player Character 카드 덱이 할당되지 않았거나 비어있습니다.");
    }

    // 플레이어 턴 시작 시 3장 카드 미리 뽑기
    public void DrawCard()
    {
        List<CardDataSO> cards = playerCharacter.Cards;
        if (cards == null || cards.Count < 3)
        {
            Debug.LogError("플레이어의 카드 덱에 카드가 3장 미만입니다.");
            return;
        }
        int actionCount = cards.FindAll(c => c.C_Type == "Action").Count;
        int supportCount = cards.FindAll(c => c.C_Type == "Support").Count;
        if (actionCount == 0)
        {
            Debug.LogError("플레이어 카드 덱에 Action 타입 카드가 없습니다.");
            return;
        }
        if (supportCount == 0)
        {
            Debug.LogError("플레이어 카드 덱에 Support 타입 카드가 없습니다.");
            return;
        }

        _actionCard = DrawRandomFrom(cards, c => c.C_Type == "Action");
        _supportCard = DrawRandomFrom(cards, c => c.C_Type == "Support");

        // _actionCard, _supportCard 제외한 리스트로 필터링
        List<CardDataSO> specialCandidates = new List<CardDataSO>();
        foreach (var card in cards)
        {
            if (card != _actionCard && card != _supportCard)
                specialCandidates.Add(card);
        }

        if (specialCandidates.Count == 0)
        {
            _specialCard = null;
            if (cardNameText != null)
                cardNameText.text = "사용가능한 카드가 없음";
        }
        else
        {
            _specialCard = DrawRandomFrom(specialCandidates, c => true);
            if (cardNameText != null)
                cardNameText.text = "빈 카드";
        }

        // 버튼 텍스트 세팅
        if (actionCardText != null)
            actionCardText.text = _actionCard != null ? _actionCard.C_Name : "빈 카드";
        if (supportCardText != null)
            supportCardText.text = _supportCard != null ? _supportCard.C_Name : "빈 카드";
        if (specialCardText != null)
            specialCardText.text = _specialCard != null ? _specialCard.C_Name : "빈 카드";

        _currentCard = null;
    }

    private CardDataSO DrawRandomFrom(List<CardDataSO> pool, System.Predicate<CardDataSO> pred)
    {
        var filtered = pool.FindAll(pred);
        if (filtered == null || filtered.Count == 0)
            return null;

        return filtered[Random.Range(0, filtered.Count)];
    }

    public void SelectActionCard()
    {
        if (_actionCard != null)
        {
            _currentCard = _actionCard;
            cardNameText.text = _actionCard.C_Name;
            Canvas.ForceUpdateCanvases();
            cardUI.SetCard(_currentCard);
            UseCard();
        }
        else
        {
            cardNameText.text = "카드 없음";
        }
    }

    public void SelectSupportCard()
    {
        if (_actionCard != null)
        {
            _currentCard = _supportCard;
            cardNameText.text = _supportCard.C_Name;
            Canvas.ForceUpdateCanvases();
            cardUI.SetCard(_currentCard);
            UseCard();
        }
        else
        {
            cardNameText.text = "카드 없음";
        }
    }

    public void SelectSpecialCard()
    {
        if (_actionCard != null)
        {
            _currentCard = _specialCard;
            cardNameText.text = _specialCard.C_Name;
            Canvas.ForceUpdateCanvases();
            cardUI.SetCard(_currentCard);
            UseCard();
        }
        else
        {
            cardNameText.text = "카드 없음";
        }
    }

    private void UpdateCardUI()
    {
        if (_currentCard != null)
            cardNameText.text = _currentCard.C_Name;
        else
            cardNameText.text = "카드 없음";

        Canvas.ForceUpdateCanvases();
    }

    // 카드 사용 함수
    public void UseCard()
    {
        if (_currentCard == null)
        {
            Debug.LogWarning("카드가 선택되지 않았습니다.");
            return;
        }
        selectedCard = _currentCard;
        //cardUI.SetCard(_currentCard);
        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog($"플레이어가 {selectedCard.C_Name}을 사용하였습니다!");
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog("주사위를 터치하여 굴려주세요.");
    }

}


