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
    private ChoiceSO _subEvents;
    private ChoiceSO _mainEvents;
    public static ChoiceManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _subEvents = Resources.Load<ChoiceSO>("Sub");
        _mainEvents = Resources.Load<ChoiceSO>("Main");
    }
    
    public void GetRandomChoice() // 선택지 버튼에 랜덤한 선택지 부여
    {
        foreach (Choice choice in choiceButtons)
        {
            choice.gameObject.SetActive(true);
            choice.Init();
        }
        switch (Stage.Chapters[GameManager.Instance.currentChapter][GameManager.Instance.currentStage])
        {
            case EventType.MainStory:
                {
                    List<MainEvent> choiceEvents = this._mainEvents.MainEvents.ToList();
                    foreach (Choice button in choiceButtons)
                    {
                        int r = Random.Range(0, choiceEvents.Count);
                        button.SetChoice(choiceEvents[r]);
                        choiceEvents.RemoveAt(r);
                    }

                    break;
                }
            case EventType.SubStory:
                {
                    List<ChoiceEvent> choiceEvents = this._subEvents.ChoiceEvent.ToList();
                    foreach (Choice button in choiceButtons)
                    {
                        int r;
                        while (true)
                        {
                            r =  Random.Range(0, choiceEvents.Count);
                            if (choiceEvents[r].isRootEvent)
                            {
                                break;
                            }
                        }
                        
                        button.SetChoice(choiceEvents[r]);
                        choiceEvents.RemoveAt(r);
                    }

                    break;
                }
            case EventType.Battle:
                break;
            case EventType.Boss:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetSubChoiceButtons(List<ChoiceEvent> subChoices) //서브 선택지가 존재 시, 현재 버튼들을 초기화하고 서브 선택지로 바꿈
    {
        int i;
        for (i = 0; i < subChoices.Count; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].SetChoice(subChoices[i]);
        }

        for (; i < choiceButtons.Count; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }
}
