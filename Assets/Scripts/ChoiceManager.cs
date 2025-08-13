using System.Collections.Generic;
using UnityEngine;


public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private ChoiceSO choices;
    [SerializeField] Choice choice1;
    [SerializeField] Choice choice2;
    [SerializeField] Choice choice3;
    [SerializeField] Choice choice4;
    private List<Choice> _choiceList;
    private Choice _currentChoice;
    public static ChoiceManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _choiceList = new List<Choice>(); //리스트 생성 후 선택지 버튼을 리스트에 추가
        _choiceList.Add(choice1);
        _choiceList.Add(choice2);
        _choiceList.Add(choice3);
        _choiceList.Add(choice4);
    }

    public void GetRandomChoice() // 선택지 버튼에 랜덤한 선택지 부여
    {
        List<string> choiceNames = choices.Choices;
        
        foreach (Choice choice in _choiceList)
        {
            int r = Random.Range(0, choiceNames.Count);
            string choiceName = choiceNames[r];
            if (choices.SubChoices[choiceName] != null) //딕셔너리 키값으로 밸류값 못찾는 경우가 있음
            {
                choice.SetChoice(choiceName, choices.ChoicesTypes[choiceName], choices.SubChoices[choiceName]);
            }
            else
            {
                choice.SetChoice(choiceName, choices.ChoicesTypes[choiceName], null);
            }
            choiceNames.Remove(choiceName);
        }
    }
}
