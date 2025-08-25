using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private List<Choice> choiceButtons = new List<Choice>();
    [SerializeField] private int[] rates;
    private Choice _currentChoice;
    public static ChoiceManager Instance;
    public ChoiceSO choices;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void GetRandomChoice() // 선택지 버튼에 랜덤한 선택지 부여
    {
        List<string> choiceNames = choices.Choices.ToList(); //참조되지 않도록 ToList 사용

        foreach (Choice choice in choiceButtons)
        {
            choice.gameObject.SetActive(true);
            int r = Random.Range(0, choiceNames.Count);
            string choiceName = choiceNames[r];
            if (choices.ChoicesTypes.ContainsKey(choiceName))
            {
                r = Random.Range(0, rates.Length);
                choice.SetChoice(choiceName, choices.ChoicesTypes[choiceName], rates[r]);
                if (choices.SubChoices.ContainsKey(choiceName)) //서브 선택지가 존재한다면 서브 선택지를 추가
                {
                    choice.SetSubChoices(choices.SubChoices[choiceName]);
                }
            }
            choiceNames.Remove(choiceName); //참조된 상태로 Remove 시 SO에 있던 값 사라짐
        }
    }

    public void SetSubChoiceButtons(List<string> str) //서브 선택지가 존재 시, 현재 버튼들을 초기화하고 서브 선택지로 바꿈
    {
        for (int i = 0; i < str.Count; i++)
        {
            choiceButtons[i].Init();
            choiceButtons[i].SetChoice(str[i], ChoiceType.Event, rates[i]);
        }

        for (int i = str.Count; i < choiceButtons.Count; i++)
        {
            choiceButtons[i].Init();
            choiceButtons[i].gameObject.SetActive(false);
        }
    }
}
