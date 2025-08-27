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

    private CardDataSO _currentCard;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (playerCharacter == null || playerCharacter.Cards == null || playerCharacter.Cards.Count == 0)
            Debug.LogWarning("Player Character 카드 덱이 할당되지 않았거나 비어있습니다.");
    }

    public void DrawCard()
    {
        if (playerCharacter == null)
        {
            Debug.LogError("playerCharacter가 할당되지 않았습니다.");
            return;
        }

        var cards = playerCharacter.Cards;
        if (cards == null || cards.Count == 0)
        {
            Debug.LogWarning("playerCharacter의 카드 덱이 비어있습니다.");
            cardNameText.text = "카드 없음";
            return;
        }

        int idx = Random.Range(0, cards.Count);
        _currentCard = cards[idx];

        if (_currentCard == null)
        {
            Debug.LogWarning("선택된 카드가 null입니다.");
            cardNameText.text = "카드 없음";
            return;
        }

        if (cardNameText == null)
        {
            Debug.LogError("cardNameText UI 컴포넌트가 할당되지 않았습니다.");
            return;
        }

        cardNameText.text = _currentCard.C_Name;
    }

    public void UseCard()
    {
        if (_currentCard == null)
        {
            Debug.LogWarning("카드가 선택되지 않았습니다.");
            return;
        }

        selectedCard = _currentCard;
        cardUI.SetCard(_currentCard);

        LogManager.Instance.AddSpacingLine();
        LogManager.Instance.AddLog($"플레이어가 {selectedCard.C_Name}을 사용하였습니다!");
        LogManager.Instance.AddLog("");
        LogManager.Instance.AddLog("주사위를 터치하여 굴려주세요.");
    }
}

