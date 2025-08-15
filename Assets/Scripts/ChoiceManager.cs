using System.Collections.Generic;
using System.Linq;
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
        List<string> choiceNames = choices.Choices.ToList(); //참조되지 않도록 ToList 사용
        
        foreach (Choice choice in _choiceList)
        {
            int r = Random.Range(0, choiceNames.Count);
            string choiceName = choiceNames[r];
            if (choices.ChoicesTypes.ContainsKey(choiceName))
            {
                choice.SetChoice(choiceName, choices.ChoicesTypes[choiceName]);
                if (choices.SubChoices.ContainsKey(choiceName)) //서브 선택지가 존재한다면 서브 선택지를 추가
                {
                    choice.SetSubChoices(choices.SubChoices[choiceName]);
                }
            }
            choiceNames.Remove(choiceName); //참조된 상태로 Remove 시 SO에 있던 값 사라짐
        }
    }
}
