using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Choice : MonoBehaviour
{
    [SerializeField] private TMP_Text choiceText;
    [SerializeField] private TMP_Text choiceTypeText;
    [SerializeField] private SpriteRenderer choiceTypeSprite;
    [SerializeField] private TMP_Text choiceRateText;
    [SerializeField] private TMP_Text choiceSuccessText;
    [SerializeField] private TMP_Text choiceFailText;
    private ChoiceType _choiceType;
    private ChoiceEvent _subEvent;
    private MainEvent _mainEvent;
    private List<ChoiceEvent> _subChoices;

    public void Init()
    {
        choiceText.text = "";
        choiceTypeText.text = "";
        choiceRateText.text = "";
    }

    public void SetChoice(MainEvent choiceEvent)
    {
        _mainEvent = choiceEvent;

        choiceText.text = _mainEvent.choiceName;
        switch (_mainEvent.choiceEventType)
        {
            case ChoiceType.Battle:
                _choiceType = ChoiceType.Battle;
                choiceTypeText.text = "전투";
                choiceTypeSprite.color = Color.red;
                break;
            case ChoiceType.Event:
                _choiceType = ChoiceType.Event;
                choiceTypeText.text = "이벤트";
                choiceTypeSprite.color = Color.blue;
                break;
            case ChoiceType.Treasure:
                _choiceType = ChoiceType.Treasure;
                choiceTypeText.text = "보물";
                choiceTypeSprite.color = Color.yellow;
                break;
            case ChoiceType.Rest:
                _choiceType = ChoiceType.Rest;
                choiceTypeText.text = "휴식";
                choiceTypeSprite.color = Color.green;
                break;
        }

        choiceRateText.text = _mainEvent.choiceRate.ToString();
        choiceRateText.text += "%";
        choiceRateText.color = _mainEvent.choiceRate switch
        {
            <= 25 => Color.red,
            <= 50 => Color.yellow,
            <= 70 => Color.orange,
            _ => Color.green
        };

        if (_mainEvent.choiceReward != "NONE")
        {
            string[] rewards = _mainEvent.choiceReward.Split('/');
            for (int i = 0; i < rewards.Length; i++)
            {
                string[] reward = rewards[i].Split('_');
                switch (reward[0])
                {
                    case "PC":
                        int cardId = int.TryParse(reward[1], out int id) ? id : 0;
                        if (cardId == 0)
                        {
                            choiceSuccessText.text += "<color=black>랜덤 카드</color> +1";
                        }
                        else
                        {
                            CardDataSO card = CardManager.Instance.FindCardById(cardId);
                            switch (card.Element)
                            {
                                case "Air":
                                    choiceSuccessText.text += $"<color=#2C9E19>바람 속성 카드</color>";
                                    break;
                                case "Fire":
                                    choiceSuccessText.text += $"<color=#F23C16>불 속성 카드</color>";
                                    break;
                                case "Water":
                                    choiceSuccessText.text += $"<color=#153696>물 속성 카드</color>";
                                    break;
                                case "Land":
                                    choiceSuccessText.text += $"<color=#AD8018>땅 속성 카드</color>";
                                    break;
                                case "None":
                                    choiceSuccessText.text += $"<color=#F23C16>스페셜 카드</color>";
                                    break;
                            }
                        }
                        break;
                    case "GOLD":
                        choiceSuccessText.text += $"골드 +{reward[1]}";
                        break;
                    case "HP":
                        choiceSuccessText.text += $"체력 +{reward[1]}";
                        break;
                }

                if (i < rewards.Length - 1)
                {
                    choiceSuccessText.text += ", ";
                }
            }
        }
    }

    public void SetChoice(ChoiceEvent choiceEvent)
        {
            _subEvent = choiceEvent;

            choiceText.text = _subEvent.choiceName;
            switch (_subEvent.choiceEventType)
            {
                case ChoiceType.Battle:
                    _choiceType = ChoiceType.Battle;
                    choiceTypeText.text = "전투";
                    choiceTypeSprite.color = Color.red;
                    break;
                case ChoiceType.Event:
                    _choiceType = ChoiceType.Event;
                    choiceTypeText.text = "이벤트";
                    choiceTypeSprite.color = Color.blue;
                    break;
                case ChoiceType.Treasure:
                    _choiceType = ChoiceType.Treasure;
                    choiceTypeText.text = "보물";
                    choiceTypeSprite.color = Color.yellow;
                    break;
                case ChoiceType.Rest:
                    _choiceType = ChoiceType.Rest;
                    choiceTypeText.text = "휴식";
                    choiceTypeSprite.color = Color.green;
                    break;
            }

            choiceRateText.text = _subEvent.choiceRate.ToString();
            choiceRateText.text += "%";
            choiceRateText.color = _subEvent.choiceRate switch
            {
                <= 25 => Color.red,
                <= 50 => Color.yellow,
                <= 70 => Color.orange,
                _ => Color.green
            };

            if (_subEvent.choiceReward != "NONE")
            {
                string[] rewards = _subEvent.choiceReward.Split('/');
                for (int i = 0; i < rewards.Length; i++)
                {
                    string[] reward = rewards[i].Split('_');
                    switch (reward[0])
                    {
                        case "PC":
                            int cardId = int.TryParse(reward[1], out int id) ? id : 0;
                            if (cardId == 0)
                            {
                                choiceSuccessText.text += "<color=black>랜덤 카드</color> +1";
                            }
                            else
                            {
                                CardDataSO card = CardManager.Instance.FindCardById(cardId);
                                switch (card.Element)
                                {
                                    case "Air":
                                        choiceSuccessText.text += $"<color=#2C9E19>바람 속성 카드</color>";
                                        break;
                                    case "Fire":
                                        choiceSuccessText.text += $"<color=#F23C16>불 속성 카드</color>";
                                        break;
                                    case "Water":
                                        choiceSuccessText.text += $"<color=#153696>물 속성 카드</color>";
                                        break;
                                    case "Land":
                                        choiceSuccessText.text += $"<color=#AD8018>땅 속성 카드</color>";
                                        break;
                                    case "None":
                                        choiceSuccessText.text += $"<color=#F23C16>스페셜 카드</color>";
                                        break;
                                }
                            }
                            break;
                        case "GOLD":
                            choiceSuccessText.text = $"골드 +{reward[1]}";
                            break;
                        case "HP":
                            choiceSuccessText.text = $"체력 +{reward[1]}";
                            break;
                    }

                    if (i < rewards.Length - 1)
                    {
                        choiceSuccessText.text += ", ";
                    }
                }
            }

            if (_subEvent.choiceLoss != "NONE")
            {
                string[] losses = _subEvent.choiceLoss.Split('/');
                for (int i = 0; i < losses.Length; i++)
                {
                    string[] reward = losses[i].Split('_');
                    switch (reward[0])
                    {
                        case "PC":
                            choiceFailText.text += "<color=black>??? 카드</color> -1";
                            break;
                        case "GOLD":
                            choiceFailText.text += $"<color=black>골드</color> {reward[1]}";
                            break;
                        case "HP":
                            choiceFailText.text += $"<color=black>HP</color> {reward[1]}";
                            break;
                    }

                    if (i < losses.Length - 1)
                    {
                        choiceFailText.text += ", ";
                    }
                }
            }
        }

        public void ChoiceClicked()
        {
            ChoiceAction().Forget();
        }

        private async UniTaskVoid ChoiceAction()
        {
            switch (Stage.Chapters[GameManager.Instance.currentChapter][GameManager.Instance.currentStage])
            {
                case EventType.MainStory: //메인 스토리 선택지
                    LogManager.Instance.AddDelayedLog(_mainEvent.choiceName, 2.0f).Forget();

                    GameManager.Instance.HideChoices();

                    await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                    LogManager.Instance.StartLog(_mainEvent.choiceText).Forget();

                    await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                    if (_subChoices is { Count: > 0 })
                    {
                        GameManager.Instance.ShowChoices();
                        ChoiceManager.Instance.SetSubChoiceButtons(_subChoices);
                    }
                    else
                    {
                        if (_choiceType == ChoiceType.Battle) //전투 시
                        {
                            GameManager.Instance.StartBattle("Rank1");

                            await UniTask.WaitUntil(() => !GameManager.Instance.isPlayerinBattle); //전투 끝날 때 까지 대기

                            if (GameManager.Instance.lastBattleWon)
                            {
                                LogManager.Instance
                                    .StartLog(_mainEvent.choiceSuccessText)
                                    .Forget();
                            }
                        }
                        else
                        {
                            int r = Random.Range(1, 101);
                            if (r <= _mainEvent.choiceRate)
                            {
                                LogManager.Instance
                                    .StartLog(_mainEvent.choiceSuccessText)
                                    .Forget();

                                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                                if (_mainEvent.choiceReward != "NONE")
                                {
                                    string[] rewards = _mainEvent.choiceReward.Split('/');
                                    foreach (string t in rewards)
                                    {
                                        string[] reward = t.Split('_');
                                        switch (reward[0])
                                        {
                                            case "PC":
                                                int id = int.Parse(reward[1]);
                                                CardDataSO card = id == 0
                                                    ? CardManager.Instance.GetRandomCard()
                                                    : CardManager.Instance.FindCardById(id);
                                                switch (card.Element)
                                                {
                                                    case "Air":
                                                        LogManager.Instance
                                                            .AddDelayedLog(
                                                                $"<color=#2C9E19>{card.C_Name}</color>을/를 획득했다.", .0f)
                                                            .Forget();
                                                        break;
                                                    case "Fire":
                                                        LogManager.Instance
                                                            .AddDelayedLog($"<color=#F23C16>{card.C_Name}</color>을/를 획득했다.",
                                                                .0f).Forget();
                                                        break;
                                                    case "Water":
                                                        LogManager.Instance
                                                            .AddDelayedLog(
                                                                $"<color=#153696>{card.C_Name}</color>을/를 획득했다.", .0f)
                                                            .Forget();
                                                        break;
                                                    case "Land":
                                                        LogManager.Instance
                                                            .AddDelayedLog($"<color=#AD8018>{card.C_Name}</color>을/를 획득했다.",
                                                                .0f).Forget();
                                                        break;
                                                    case "None":
                                                        LogManager.Instance
                                                            .AddDelayedLog($"<color=#BF05F2>{card.C_Name}</color>을/를 획득했다.",
                                                                .0f).Forget();
                                                        break;
                                                }

                                                PlayerManager.Instance.Player.Cards.Add(card);
                                                break;
                                            case "GOLD":
                                                LogManager.Instance.AddDelayedLog($"골드를 {reward[1]}만큼 얻었다.", 1.0f).Forget();
                                                break;
                                            case "HP":
                                                LogManager.Instance.AddDelayedLog($"체력을 {reward[1]}만큼 회복했다.", 1.0f)
                                                    .Forget();
                                                PlayerManager.Instance.Player.SetCurrentHp(
                                                    PlayerManager.Instance.Player.GetCurrentHp() + int.Parse(reward[1]) >
                                                    100
                                                        ? 100
                                                        : PlayerManager.Instance.Player.GetCurrentHp() +
                                                          int.Parse(reward[1]));
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                LogManager.Instance
                                    .StartLog(_mainEvent.choiceFailText).Forget();
                                await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);
                            }
                        }
                    }

                    break;
                case EventType.SubStory: //서브 스토리 선택지
                    LogManager.Instance.AddDelayedLog(_subEvent.choiceName, 2.0f).Forget();

                    GameManager.Instance.HideChoices();

                    await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                    LogManager.Instance.StartLog(_subEvent.choiceText).Forget();

                    await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

                    if (_subChoices is { Count: > 0 }) //세부 선택지 존재 시
                    {
                        GameManager.Instance.ShowChoices();
                        ChoiceManager.Instance.SetSubChoiceButtons(_subChoices);
                        return;
                    }

                    if (_choiceType == ChoiceType.Battle)
                    {
                        GameManager.Instance.StartBattle("Rank1");

                        await UniTask.WaitUntil(() => !GameManager.Instance.isPlayerinBattle); //전투 끝날 때 까지 대기

                        if (GameManager.Instance.lastBattleWon)
                        {
                            LogManager.Instance
                                .StartLog(_subEvent.choiceSuccessText)
                                .Forget();

                            if (_subEvent.choiceReward != "NONE")
                            {
                                string[] rewards = _subEvent.choiceReward.Split('/');
                                foreach (string t in rewards)
                                {
                                    string[] reward = t.Split('_');
                                    switch (reward[0])
                                    {
                                        case "PC":
                                            int id = int.Parse(reward[1]);
                                            CardDataSO card = id == 0
                                                ? CardManager.Instance.GetRandomCard()
                                                : CardManager.Instance.FindCardById(int.Parse(reward[1]));
                                            switch (card.Element)
                                            {
                                                case "Air":
                                                    LogManager.Instance
                                                        .AddDelayedLog(
                                                            $"<color=#2C9E19>{card.C_Name}</color>을/를 획득했다.", .0f)
                                                        .Forget();
                                                    break;
                                                case "Fire":
                                                    LogManager.Instance
                                                        .AddDelayedLog($"<color=#F23C16>{card.C_Name}</color>을/를 획득했다.",
                                                            .0f).Forget();
                                                    break;
                                                case "Water":
                                                    LogManager.Instance
                                                        .AddDelayedLog(
                                                            $"<color=#153696>{card.C_Name}</color>을/를 획득했다.", .0f)
                                                        .Forget();
                                                    break;
                                                case "Land":
                                                    LogManager.Instance
                                                        .AddDelayedLog($"<color=#AD8018>{card.C_Name}</color>을/를 획득했다.",
                                                            .0f).Forget();
                                                    break;
                                                case "None":
                                                    LogManager.Instance
                                                        .AddDelayedLog($"<color=#BF05F2>{card.C_Name}</color>을/를 획득했다.",
                                                            .0f).Forget();
                                                    break;
                                            }

                                            PlayerManager.Instance.Player.Cards.Add(card);
                                            break;
                                        case "GOLD":
                                            LogManager.Instance.AddDelayedLog($"골드를 {reward[1]}만큼 얻었다.", 1.0f).Forget();
                                            break;
                                        case "HP":
                                            LogManager.Instance.AddDelayedLog($"체력을 {reward[1]}만큼 회복했다.", 1.0f).Forget();
                                            PlayerManager.Instance.Player.SetCurrentHp(
                                                PlayerManager.Instance.Player.GetCurrentHp() + int.Parse(reward[1]) > 100
                                                    ? 100
                                                    : PlayerManager.Instance.Player.GetCurrentHp() + int.Parse(reward[1]));
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int r = Random.Range(1, 101);
                        if (r <= _subEvent.choiceRate)
                        {
                            LogManager.Instance
                                .StartLog(_subEvent.choiceSuccessText)
                                .Forget();
                            if (_subEvent.choiceReward != "NONE")
                            {
                                string[] rewards = _subEvent.choiceReward.Split('/');
                                foreach (string t in rewards)
                                {
                                    string[] reward = t.Split('_');
                                    switch (reward[0])
                                    {
                                        case "PC":
                                            int cardId = int.TryParse(reward[1], out int id) ? id : 0;
                                            CardDataSO card = cardId != 0
                                                ? CardManager.Instance.FindCardById(cardId)
                                                : CardManager.Instance.GetRandomCard();
                                            switch (card.Element)
                                            {
                                                case "Air":
                                                    LogManager.Instance
                                                        .AddDelayedLog(
                                                            $"<color=#2C9E19>{card.C_Name}</color>을/를 획득했다.", .0f)
                                                        .Forget();
                                                    break;
                                                case "Fire":
                                                    LogManager.Instance
                                                        .AddDelayedLog($"<color=#F23C16>{card.C_Name}</color>을/를 획득했다.",
                                                            .0f).Forget();
                                                    break;
                                                case "Water":
                                                    LogManager.Instance
                                                        .AddDelayedLog(
                                                            $"<color=#153696>{card.C_Name}</color>을/를 획득했다.", .0f)
                                                        .Forget();
                                                    break;
                                                case "Land":
                                                    LogManager.Instance
                                                        .AddDelayedLog($"<color=#AD8018>{card.C_Name}</color>을/를 획득했다.",
                                                            .0f).Forget();
                                                    break;
                                                case "None":
                                                    LogManager.Instance
                                                        .AddDelayedLog($"<color=#BF05F2>{card.C_Name}</color>을/를 획득했다.",
                                                            .0f).Forget();
                                                    break;
                                            }

                                            PlayerManager.Instance.Player.Cards.Add(card);
                                            break;
                                        case "GOLD":
                                            LogManager.Instance.AddDelayedLog($"골드를 {reward[1]}만큼 얻었다.", 1.0f).Forget();
                                            break;
                                        case "HP":
                                            LogManager.Instance.AddDelayedLog($"체력을 {reward[1]}만큼 회복했다.", 1.0f).Forget();
                                            PlayerManager.Instance.Player.SetCurrentHp(
                                                PlayerManager.Instance.Player.GetCurrentHp() + int.Parse(reward[1]) > 100
                                                    ? 100
                                                    : PlayerManager.Instance.Player.GetCurrentHp() + int.Parse(reward[1]));
                                            break;
                                    }
                                }
                            }
                        } //서브 이벤트 성공 시
                        else
                        {
                            LogManager.Instance
                                .StartLog(_subEvent.choiceFailText).Forget();
                            if (_subEvent.choiceLoss != "NONE")
                            {
                                string[] losses = _subEvent.choiceLoss.Split('/');
                                foreach (string t in losses)
                                {
                                    string[] loss = t.Split('_');
                                    switch (loss[0])
                                    {
                                        case "PC":
                                            break;
                                        case "GOLD":
                                            LogManager.Instance.AddDelayedLog($"골드를 {loss[1]}개 잃었다...", 1.0f).Forget();
                                            break;
                                        case "HP":
                                            LogManager.Instance.AddDelayedLog($"체력 {int.Parse(loss[1]) * -1}만큼 줄었다...", 1.0f).Forget();
                                            PlayerManager.Instance.Player.SetCurrentHp(
                                                PlayerManager.Instance.Player.GetCurrentHp() - int.Parse(loss[1]) < 0
                                                    ? 0
                                                    : PlayerManager.Instance.Player.GetCurrentHp() - int.Parse(loss[1]));
                                            break;
                                    }
                                }
                            }
                        } //서브 이벤트 실패 시
                    }

                    break;
            }

            await UniTask.WaitUntil(() => !LogManager.Instance.isLogging);

            await UniTask.WaitForSeconds(3.0f);

            LogManager.Instance.AddSpacingLine();

            GameManager.Instance.NextStage().Forget();
        }
}