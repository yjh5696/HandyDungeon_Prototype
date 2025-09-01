using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private UnityEngine.UI.Image specialButtonImage;
    [SerializeField] private UnityEngine.UI.Image CardEffectButton;

    private CardPackSO _cardPacks;

    private CardDataSO _currentCard;

    private CardDataSO _actionCard;
    private CardDataSO _supportCard;
    private CardDataSO _specialCard;

    private CardDataSO[] _allCards;

    private Dictionary<int, int> _cardEnhanceCounts = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _allCards = Resources.LoadAll<CardDataSO>("CardDataSOs").ToArray();
        _cardPacks = Resources.Load<CardPackSO>("CardPacks");

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
            if (cardNameText)
                cardNameText.text = "사용가능한 카드가 없음";
        }
        else
        {
            _specialCard = DrawRandomFrom(specialCandidates, c => true);
            UpdateButtonColor(_specialCard);
            if (cardNameText)
                cardNameText.text = "빈 카드";
        }

        // 버튼 텍스트 세팅
        if (actionCardText)
            actionCardText.text = _actionCard ? _actionCard.C_Name : "빈 카드";
        if (supportCardText)
            supportCardText.text = _supportCard ? _supportCard.C_Name : "빈 카드";
        if (specialCardText)
            specialCardText.text = _specialCard ? _specialCard.C_Name : "빈 카드";

        _currentCard = null;
    }

    private void UpdateButtonColor(CardDataSO ChangeColorCard)
    {
        if (!specialButtonImage)
            return;

        if (!ChangeColorCard)
        {
            specialButtonImage.color = new Color32(255, 251, 157, 255); // 기본 색상
        }
        else if (ChangeColorCard.C_Type == "Action")
        {
            specialButtonImage.color = new Color32(248, 134, 134, 255); // 빨강
        }
        else if (ChangeColorCard.C_Type == "Support")
        {
            specialButtonImage.color = new Color32(83, 255, 59, 255); // 초록
        }
        else
        {
            specialButtonImage.color = new Color32(255, 251, 157, 255); // 기본 색상(스페셜)
        }
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

    // 카드 사용 함수
    public void SelectCard()
    {
        if (selectedCard.Enhanceable == "Yes")
        {
            int id = selectedCard.C_Id;
            if (!_cardEnhanceCounts.ContainsKey(id)) // 처음 사용하는 카드라면 딕셔너리에 추가
            {
                _cardEnhanceCounts[id] = selectedCard.Enhance_Count;
            }

            if (_cardEnhanceCounts[id] > 0)
            {
                _cardEnhanceCounts[id]--;
                Debug.Log($"[Card Enhance] 카드: {selectedCard.C_Name} (ID: {id}), 남은 강화 횟수: {_cardEnhanceCounts[id]}");
            }
        }
    }

    public string GetEnhanceCountString(int cardId)
    {
        if (_cardEnhanceCounts.TryGetValue(cardId, out int count))
        {
            return count.ToString();
        }

        CardDataSO card = FindCardById(cardId);
        if (card != null)
        {
            if (card.Enhanceable == "Yes")
            {
                return card.Enhance_Count.ToString();
            }
            else if (card.Enhanceable == "No")
            {
                return "최종형태";
            }
        }

        // 딕셔너리에도 없고 카드 정보가 없으면 기본
        return "정보 없음";
    }

    public void CardEffectON()
    {
        if(selectedCard.C_Type == "Special")
        {
            CardEffectButton.gameObject.SetActive(false);
        }
        else if(selectedCard.C_Type == "Action" || selectedCard.C_Type == "Support")
        {
            CardEffectButton.gameObject.SetActive(true);
        }
    }

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

    public void UpgradeEnhanceableCardsOnTurnEnd()
    {
        List<CardDataSO> cards = playerCharacter.Cards;
        List<CardDataSO> cardsToRemove = new List<CardDataSO>();
        List<CardDataSO> cardsToAdd = new List<CardDataSO>();

        Debug.Log($"[Upgrade] 현재 카드 풀 상태:");
        foreach (var c in cards)
        {
            Debug.Log($"  카드: {c.C_Name} (ID: {c.C_Id})");
        }
        Debug.Log($"[Upgrade] cardEnhanceCounts 상태:");
        foreach (var kvp in _cardEnhanceCounts)
        {
            Debug.Log($"  카드 ID: {kvp.Key}, 강화 카운트: {kvp.Value}");
        }

        foreach (var card in cards)
        {
            if (card.Enhanceable == "Yes")
            {
                int id = card.C_Id;

                if (!_cardEnhanceCounts.ContainsKey(id))
                {
                    _cardEnhanceCounts[id] = card.Enhance_Count;
                    Debug.Log($"[Upgrade] 딕셔너리 초기화: {card.C_Name} (ID: {id}) Enhance_Count = {card.Enhance_Count}");
                }

                if (_cardEnhanceCounts[id] <= 0)
                {
                    cardsToRemove.Add(card);

                    CardDataSO nextCard = FindCardById(id + 1);
                    Debug.Log($"[Upgrade] 카드 {card.C_Name} (ID: {id}) 강화 완료, 다음 카드 ID: {id + 1}");

                    if (nextCard != null)
                    {
                        cardsToAdd.Add(nextCard);
                        LogManager.Instance.AddLog($"{card.C_Name} 강화 완료, {nextCard.C_Name}로 교체되었습니다.");

                        if (!_cardEnhanceCounts.ContainsKey(nextCard.C_Id))
                        {
                            _cardEnhanceCounts[nextCard.C_Id] = nextCard.Enhance_Count;
                            Debug.Log($"[Upgrade] 딕셔너리 초기화: {nextCard.C_Name} (ID: {nextCard.C_Id}) Enhance_Count = {nextCard.Enhance_Count}");
                        }
                    }
                    else
                    {
                        LogManager.Instance.AddLog($"{card.C_Name}의 강화 카드({id + 1})를 찾을 수 없습니다.");
                        Debug.LogWarning($"[Upgrade] 강화 카드 없음: {card.C_Name} (ID: {id}), 다음 카드 ID: {id + 1}");
                    }
                }
                else
                {
                    Debug.Log($"[Upgrade] 강화 중: {card.C_Name} (ID: {id}), 남은 강화 카운트: {_cardEnhanceCounts[id]}");
                }
            }
        }

        foreach (var removeCard in cardsToRemove)
        {
            cards.Remove(removeCard);
            _cardEnhanceCounts.Remove(removeCard.C_Id);
            Debug.Log($"[Upgrade] 카드 제거: {removeCard.C_Name} (ID: {removeCard.C_Id})");
        }
        foreach (var addCard in cardsToAdd)
        {
            cards.Add(addCard);
            Debug.Log($"[Upgrade] 카드 추가: {addCard.C_Name} (ID: {addCard.C_Id})");
        }
    }



    // 카드 ID로 카드 데이터를 찾는 함수 
    public CardDataSO FindCardById(int id)
    {
        CardDataSO card = _allCards.FirstOrDefault(card => card.C_Id == id);
        return !card ? null : card;
    }

    public CardDataSO GetRandomCard()
    {
        int r = Random.Range(0, _allCards.Length);
        while (true)
        {
            if (_allCards[r].Tier == "tier1")
            {
                return _allCards[r];
            }

            r = Random.Range(0, _allCards.Length);
        }
    }

    public CardPack FindCardPackById(int id)
    {
        if(id > _cardPacks.CardPacks.Count || id <= 0) return null;
        return _cardPacks.CardPacks.Find(x => x.cardPackID == id);
    }

}


