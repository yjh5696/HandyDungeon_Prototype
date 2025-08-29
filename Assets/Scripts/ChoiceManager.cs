using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private List<Choice> choiceButtons = new List<Choice>();
    [SerializeField] private int[] rates;
    private Choice _currentChoice;
    private List<string> _choiceNamesMain; //메인 스테이지 지우는 용
    public static ChoiceManager Instance;
    public ChoiceSO subEvents;
    public ChoiceSO mainEvents;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void GetRandomChoice() // 선택지 버튼에 랜덤한 선택지 부여
    {
        if (Stage.Chapters[GameManager.Instance.currentChapter][GameManager.Instance.currentStage] ==
            EventType.MainStory)
        {
            List<MainEvent> choiceEvents = this.mainEvents.MainEvents.ToList();
            foreach (Choice button in choiceButtons)
            {
                int r = Random.Range(0, choiceEvents.Count);
                button.SetChoice(choiceEvents[r]);
                choiceEvents.RemoveAt(r);
            }
        }
        else if (Stage.Chapters[GameManager.Instance.currentChapter][GameManager.Instance.currentStage] ==
                 EventType.SubStory)
        {
            List<ChoiceEvent> choiceEvents = this.subEvents.ChoiceEvent.ToList();
            foreach (Choice button in choiceButtons)
            {
                int r = Random.Range(0, choiceEvents.Count);
                button.SetChoice(choiceEvents[r]);
                choiceEvents.RemoveAt(r);
            }
        }
    }

    public void SetSubChoiceButtons(List<ChoiceEvent> subChoices) //서브 선택지가 존재 시, 현재 버튼들을 초기화하고 서브 선택지로 바꿈
    {
        
    }
}
